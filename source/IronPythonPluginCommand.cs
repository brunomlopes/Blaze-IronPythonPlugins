using System;
using System.Diagnostics;
using System.IO;
using SystemCore.CommonTypes;

namespace IronPythonPlugins
{
    class IronPythonPluginCommand : Command
    {
        public IronPythonPluginCommand(FileInfo pythonFile, IIronPythonCommand plugin)
            : base(plugin.GetName(), "Python script " + plugin.GetName())
        {
            SetIsOwnerDelegate(plugin.IsOwner);
            SetNameDelegate(plugin.GetNameForParameters);
            SetDescriptionDelegate(parameters =>
                                       {
                                           var description = plugin.GetDescription(parameters);
                                           return string.IsNullOrEmpty(description)
                                                      ? pythonFile.FullName + ":" + plugin.GetName()
                                                      : description;
                                       });
            SetIconDelegate(str => Resources.python_clear.ToBitmap());
            SetAutoCompleteDelegate(plugin.AutoComplete);
            SetUsageDelegate(plugin.Usage);
            SetExecuteDelegate((parameters, modifiers) =>
                                   {
                                       try
                                       {
                                           var result = plugin.Execute(parameters);
                                           if(!string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(result.Trim()))
                                           {
                                               var command = result.Split(new[] {' '}, 2);
                                               if(command.Length == 1)
                                               {
                                                   Process.Start(command[0]);
                                               }
                                               else
                                               {
                                                   Process.Start(command[0], command[1]);
                                               }
                                           }
                                       }
                                       catch (Exception e)
                                       {
                                           Debug.Write(string.Format("Error executing {0}:{1}", pythonFile.FullName,
                                                                     e.Message));
                                           Debug.Write(e.StackTrace);
                                           if(e.InnerException != null)
                                           {
                                               Debug.Write(e.InnerException);
                                           }
                                           SetDescriptionDelegate(s => string.Format("Error in past execution: {0}", e.Message));
                                       }
                                   });
        }
    }
}