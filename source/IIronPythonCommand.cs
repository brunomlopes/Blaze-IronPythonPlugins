using SystemCore.CommonTypes;

namespace IronPythonPlugins
{
    public interface IIronPythonCommand
    {
        void Execute(string command);
        string GetName(string parameters);
        string GetDescription(string parameters);
        string AutoComplete(string parameters);
        bool IsOwner(string parameters);
        string Name { get; }
        CommandUsage Usage(string parameters);
    }
}