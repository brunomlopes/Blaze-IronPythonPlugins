using System;
using SystemCore.CommonTypes;

namespace IronPythonPlugins
{
    public abstract class BaseIronPythonCommand : IIronPythonCommand
    {
        public abstract string Name { get; }

        public virtual CommandUsage Usage(string parameters)
        {
            return new CommandUsage(Name);
        }

        public virtual string GetName(string parameters)
        {
            return Name;
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
                   split[0].Trim().ToLowerInvariant() == Name.ToLowerInvariant();
        }

        public abstract void Execute(string command);
    }
}
