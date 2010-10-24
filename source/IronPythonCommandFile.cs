using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemCore.CommonTypes;
using SystemCore.SystemAbstraction.WindowManagement;
using ContextLib;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace IronPythonPlugins
{
    public class IronPythonCommandFile : IEnumerable<Command>
    {
        private readonly ScriptEngine _engine;
        private readonly List<Command> _localCommands;
        private string _fileFullName;

        public IronPythonCommandFile(ScriptEngine engine, FileInfo pythonFile)
            : this(engine)
        {
            _fileFullName = pythonFile.FullName;

            var script = engine.CreateScriptSourceFromFile(_fileFullName);

            FillCommandsFromScriptSource(script);
        }

        public IronPythonCommandFile(ScriptEngine engine, string sourceCode)
            :this(engine)
        {
            _fileFullName = "::string::";
            _engine = engine;
            var script = engine.CreateScriptSourceFromString(sourceCode, SourceCodeKind.File);
            FillCommandsFromScriptSource(script);
        }

        private IronPythonCommandFile(ScriptEngine engine)
        {
            _engine = engine;
            _localCommands = new List<Command>();
        }

        private void FillCommandsFromScriptSource(ScriptSource script)
        {
            CompiledCode code;
            try
            {
                code = script.Compile();
            }
            catch (SyntaxErrorException e)
            {
                throw new SyntaxErrorExceptionPrettyWrapper(string.Format("Error compiling '{0}", _fileFullName), e);
            }
           
            ScriptScope scope = _engine.CreateScope();

            scope.SetVariable("IIronPythonCommand", ClrModule.GetPythonType(typeof(IIronPythonCommand)));
            scope.SetVariable("BaseIronPythonCommand", ClrModule.GetPythonType(typeof(BaseIronPythonCommand)));
            scope.SetVariable("UserContext", UserContext.Instance);
            scope.SetVariable("WindowUtility", WindowUtility.Instance);
            scope.SetVariable("clr", _engine.GetClrModule());
            try
            {
                code.Execute(scope);
            }
            catch (UnboundNameException e)
            {
                throw new PythonException(string.Format("Error compiling '{0}'", _fileFullName), e);
            }


            var pluginClasses = scope.GetItems()
                .Where(kvp => kvp.Value is IronPython.Runtime.Types.PythonType)
                .Where(
                    kvp =>
                    typeof(IIronPythonCommand).IsAssignableFrom(((IronPython.Runtime.Types.PythonType)kvp.Value).__clrtype__()))
                .Where(kvp => kvp.Key != "BaseIronPythonCommand" && kvp.Key != "IIronPythonCommand");


            var pluginMethods = scope.GetItems()
                .Where(kvp => _engine.Operations.IsCallable(kvp.Value) && kvp.Value is PythonFunction)
                .Where(kvp => !kvp.Key.StartsWith("_"));

            foreach (var nameAndClass in pluginClasses)
            {
                var plugin = (IIronPythonCommand)_engine.Operations.Invoke(nameAndClass.Value, new object[] { });
                var commandName = CamelToSpaced(nameAndClass.Key);

                var description = _engine.Operations.GetDocumentation(nameAndClass.Value);
                if (plugin as BaseIronPythonCommand != null)
                {
                    // retrieving the class name from the python time is a bit trickier without access to the engine
                    // so we pass this here
                    ((BaseIronPythonCommand) plugin).SetDefaultName(commandName);
                }
                var command = new IronPythonPluginCommand(_fileFullName, plugin, description);

                _localCommands.Add(command);
            }

            foreach (var pluginMethod in pluginMethods)
            {
                var commandName = CamelToSpaced(pluginMethod.Key);
                var method = pluginMethod.Value;
                var commandFromMethod = new IronPythonCommandFromMethod(commandName, arguments => _engine.Operations.Invoke(method, arguments).ToString());

                var description = _engine.Operations.GetDocumentation(pluginMethod.Value);
                if (!string.IsNullOrEmpty(description))
                {
                    commandFromMethod.SetDescription(description);
                }

                var ironPythonPluginCommand = new IronPythonPluginCommand(_fileFullName, commandFromMethod, description);
                
                _localCommands.Add(ironPythonPluginCommand);
            }
        }

        private string CamelToSpaced(string name)
        {
            return System.Text.RegularExpressions.Regex.Replace(name, "(?<l>[A-Z])", " ${l}").Trim();
        }

        public IEnumerator<Command> GetEnumerator()
        {
            return _localCommands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}