using System;
using System.Diagnostics;
using SystemCore.CommonTypes;

namespace IronPythonPlugins
{
    public class IronPythonPluginCommand : Command
    {
        public IronPythonPluginCommand(string fileFullName, IIronPythonCommand plugin, string defaultDescription)
            : base(plugin.GetName(), GetDescriptionFromCommand(plugin, defaultDescription, string.Empty))
        {
            var pythonFileFullName = fileFullName;


            SetIsOwnerDelegate(plugin.IsOwner);
            SetNameDelegate(plugin.GetNameForParameters);
            SetDescriptionDelegate(parameters =>
                                       {
                                           return GetDescriptionFromCommand(plugin, defaultDescription, parameters);
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
                                           Debug.Write(string.Format("Error executing {0}:{1}", pythonFileFullName,
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

        private static string GetDescriptionFromCommand(IIronPythonCommand plugin, string defaultDescription, string parameters)
        {
            var currentDescription = plugin.GetDescription(parameters);
            return string.IsNullOrEmpty(currentDescription)
                       ? defaultDescription
                       : currentDescription;
        }
    }
}