using System;

namespace IronPythonPlugins
{
    public class IronPythonCommandFromMethod : BaseIronPythonCommand
    {
        private readonly Func<string, string> _pythonMethodCall;

        public IronPythonCommandFromMethod(string name, Func<string, string> pythonMethodCall)
        {
            _pythonMethodCall = pythonMethodCall;
            SetDefaultName(name);
        }

        public override string Execute(string command)
        {
            return _pythonMethodCall(command);
        }
    }
}