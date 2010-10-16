using System;

namespace IronPythonPlugins
{
    public class IronPythonCommandFromMethod : BaseIronPythonCommand
    {
        private string _description;
        private readonly Func<string, string> _pythonMethodCall;

        public IronPythonCommandFromMethod(string name, Func<string, string> pythonMethodCall)
        {
            _description = name;
            _pythonMethodCall = pythonMethodCall;
            SetDefaultName(name);
        }

        public override string Execute(string command)
        {
            return _pythonMethodCall(command);
        }

        public override string GetDescription(string parameters)
        {
            return _description;
        }

        public void SetDescription(string description)
        {
            _description = description;
        }
    }
}