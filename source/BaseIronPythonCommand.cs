using System;
using SystemCore.CommonTypes;

namespace IronPythonPlugins
{
    public abstract class BaseIronPythonCommand : IIronPythonCommand
    {
        private string _name;

        public virtual CommandUsage Usage(string parameters)
        {
            return new CommandUsage(GetName());
        }

        public virtual string GetName()
        {
            return _name;
        }

        public virtual string GetNameForParameters(string parameters)
        {
            return GetName();
        }

        public virtual string GetDescription(string parameters)
        {
            return string.Empty;
        }

        public virtual string AutoComplete(string parameters)
        {
            return parameters;
        }

        public bool IsOwner(string str)
        {
            var split = str.Split(new[] {' '}, 1);
            return split.Length > 0 &&
                   split[0].Trim().ToLowerInvariant() == GetName().ToLowerInvariant();
        }

        public abstract string Execute(string command);

        public void SetDefaultName(string name)
        {
            _name = name;
        }
    }
}
