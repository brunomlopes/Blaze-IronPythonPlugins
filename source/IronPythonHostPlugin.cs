using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Xml.Serialization;
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
        private Dictionary<string, IronPythonCommandFile> _pythonCommands;
        private List<FileSystemWatcher> _pluginFolderWatchers;

        private readonly object _syncRoot;
        private readonly Queue<string> _pathsToReload;
        private readonly Timer _reloadTimer;

        public IronPythonHostPlugin()
            : base("Host for IronPython plugins")
        {
            _configurable = true;
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
            catch (IOException)
            {
                Debug.Write(string.Format("File {0} was locked for read. Will retry again.", path));
                lock (_syncRoot)
                {
                    _pathsToReload.Enqueue(path);
                }
            }
            catch (Exception e)
            {
                Debug.Write(string.Format("Error with file {1}: {0}.\nWill not reload it again",
                                            e, path));
            }
        }
        public override void Configure()
        {
            _configuration = new Configuration(_configuration).ShowConfigurationDialog();
            SaveSettings();
            SetupCommands();
        }

        private const string _Name = "IronPythonPlugins";

        private Configuration.ConfigurationObject _configuration = new Configuration.ConfigurationObject();
        private readonly string _configurationFileName = CommonInfo.UserFolder + _Name + ".xml";

        private void SaveSettings()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Configuration.ConfigurationObject));
            using (TextWriter writer = new StreamWriter(_configurationFileName, false, Encoding.Default))
            {
                serializer.Serialize(writer, _configuration);
            }
        }

        private void LoadSettings()
        {
            _configuration = new Configuration.ConfigurationObject();
            if (File.Exists(_configurationFileName))
            {
                var serializer = new XmlSerializer(typeof(Configuration.ConfigurationObject));
                _configuration = (Configuration.ConfigurationObject)serializer.Deserialize(new StreamReader(_configurationFileName, Encoding.Default));
            }
        }

        protected override void SetupCommands()
        {
            LoadSettings();

            _engine = Python.CreateEngine();
            _pythonCommands = new Dictionary<string, IronPythonCommandFile>();
            _pluginFolderWatchers = new List<FileSystemWatcher>();

            foreach (var folder in
                new[] {Path.Combine(CommonInfo.PluginsFolder, "IronPythonPlugins")}
                    .Concat(_configuration.IronPythonScriptPaths)
                    .Where(Directory.Exists))
            {
                Debug.Write(string.Format("Loading scripts from {0}", folder));
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
            var localCommands = new IronPythonCommandFile(_engine, new FileInfo(pythonFile));
            
            Commands.AddRange(localCommands);
            _pythonCommands[pythonFile] = localCommands;
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