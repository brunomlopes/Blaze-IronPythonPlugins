using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using SystemCore.CommonTypes;
using SystemCore.SystemAbstraction.WindowManagement;
using Configurator;
using ContextLib;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace IronPythonPlugins
{
    [AutomatorPluginAttribute("IronPythonHostPlugin for plugins")]
    public class IronPythonHostPlugin : InterpreterPlugin
    {
        private ScriptEngine _engine;
        private Dictionary<string, IList<Command>> _pythonCommands;
        private string _ironpythonPluginsFolder;
        private List<FileSystemWatcher> _pluginFolderWatchers;

        private readonly object _syncRoot;
        private readonly Queue<string> _pathsToReload;
        private readonly Timer _reloadTimer;

        public IronPythonHostPlugin()
            : base("Host for IronPython plugins")
        {
            _pathsToReload = new Queue<string>();
            _reloadTimer = new Timer();
            _syncRoot = new object();

            _reloadTimer.AutoReset = true;
            _reloadTimer.Interval = 0.5;
            _reloadTimer.Elapsed += OnReloadFilesTimerElapsed;

            _reloadTimer.Start();
        }

        public void OnReloadFilesTimerElapsed(object sender, EventArgs evt)
        {
            string path;
            lock (_syncRoot)
            {
                if (_pathsToReload.Count == 0)
                {
                    return;
                }
                path = _pathsToReload.Dequeue();
                RemoveCommandsForFile(path);
            }
            try
            {
                using (File.OpenRead(path))
                {
                    ; // just try to open the file to read.
                    // if we can't, we'll try again later
                }
                LoadPythonCommandsForFile(path);
                Debug.Write("Reloaded " + path);
            }
            catch (SyntaxErrorException e)
            {
                Debug.Write(string.Format("Error with file {1}: {0}",
                                            e, path));
            }
            catch (IOException e)
            {
                Debug.Write(string.Format("File {0} was locked for read. Will retry again.", path));
                lock (_syncRoot)
                {
                    _pathsToReload.Enqueue(path);
                }
            }
            catch (Exception e)
            {
                Debug.Write(string.Format("Error with file {1}: {0}",
                                            e, path));
                lock (_syncRoot)
                {
                    _pathsToReload.Enqueue(path);
                }
            }
        }

        protected override void SetupCommands()
        {
            _engine = Python.CreateEngine();
            _pythonCommands = new Dictionary<string, IList<Command>>();
            _pluginFolderWatchers = new List<FileSystemWatcher>();

            foreach (var folder in new[] { Path.Combine(CommonInfo.PluginsFolder, "IronPythonPlugins")})
            {    
                SetupPythonCommands(folder);

                var pluginFolderWatcher = new FileSystemWatcher(folder, "*.ipy");
                pluginFolderWatcher.IncludeSubdirectories = true;
                pluginFolderWatcher.Changed += _pluginFolderWatcher_Changed;
                pluginFolderWatcher.Created += _pluginFolderWatcher_Changed;
                pluginFolderWatcher.Deleted += (sender, e) =>
                                                    {
                                                        lock (_syncRoot)
                                                        {
                                                            RemoveCommandsForFile(e.FullPath);
                                                        }
                                                    };
                pluginFolderWatcher.EnableRaisingEvents = true;
                _pluginFolderWatchers.Add(pluginFolderWatcher);
                }
        }

        void _pluginFolderWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (_syncRoot)
            {
                if (_pathsToReload.Contains(e.FullPath)) return;
                _pathsToReload.Enqueue(e.FullPath);
            }
        }

        private void RemoveCommandsForFile(string pythonFile)
        {
            if (_pythonCommands.ContainsKey(pythonFile))
            {
                foreach (var command in _pythonCommands[pythonFile])
                {
                    _commands.Remove(command);
                }
                _pythonCommands.Remove(pythonFile);
            }
        }

        private void SetupPythonCommands(string folder)
        {
            var pythonFiles =
                Directory
                    .GetFiles(folder)
                    .Where(f => Path.GetExtension(f).ToLowerInvariant() == ".ipy");

            foreach (var pythonFile in pythonFiles)
            {
                try
                {
                    LoadPythonCommandsForFile(pythonFile);
                }
                catch (Exception e)
                {
                    Debug.Write(string.Format("Error with file {1}: {0}",
                                              e, pythonFile));
                }
            }
        }

        private void LoadPythonCommandsForFile(string pythonFile)
        {
            ScriptSource script = _engine.CreateScriptSourceFromFile(pythonFile);
            CompiledCode code = script.Compile();
            ScriptScope scope = _engine.CreateScope();

            scope.SetVariable("IIronPythonCommand", ClrModule.GetPythonType(typeof(IIronPythonCommand)));
            scope.SetVariable("BaseIronPythonCommand", ClrModule.GetPythonType(typeof(BaseIronPythonCommand)));
            scope.SetVariable("UserContext", UserContext.Instance);
            scope.SetVariable("WindowUtility", WindowUtility.Instance);
            scope.SetVariable("clr", _engine.GetClrModule());
            code.Execute(scope);

            _pythonCommands[pythonFile] = new List<Command>();


            var pluginClasses = scope.GetItems()
                .Where(kvp => kvp.Value is IronPython.Runtime.Types.PythonType)
                .Where(
                    kvp =>
                    typeof(IIronPythonCommand).IsAssignableFrom(((IronPython.Runtime.Types.PythonType)kvp.Value).__clrtype__()))
                .Where(kvp => kvp.Key != "BaseIronPythonCommand" && kvp.Key != "IIronPythonCommand")

                .Select(kvp => kvp.Value);


            foreach (var p in pluginClasses)
            {
                var plugin = (IIronPythonCommand)_engine.Operations.Invoke(p, new object[] { });

                var command = new IronPythonPluginCommand(new FileInfo(pythonFile), plugin);

                _commands.Add(command);
                _pythonCommands[pythonFile].Add(command);
            }
        }

        protected override string GetAssembyName()
        {
            return "IronPythonHostPlugin";
        }

        protected override string GetAssemblyVersion()
        {
            return "1.0.0.0";
        }
    }
}