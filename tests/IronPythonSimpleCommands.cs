using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using IronPython.Hosting;
using IronPythonPlugins;
using Microsoft.Scripting.Hosting;
using Xunit;

namespace Tests
{
    public class IronPythonSimpleCommands
    {
        

        [Fact]
        public void Name_For_Class_With_No_GetName_Is_ClassName()
        {
            var commandFile = ForCode(@"class Command(BaseIronPythonCommand):
  def Execute(self, args): return 1");

            Assert.Equal(1, commandFile.Count());
            Assert.Equal("Command", commandFile.First().Name);
        }
        
        [Fact]
        public void Name_For_Class_With_GetName_Is_Result_Of_GetName()
        {
            var commandFile = new IronPythonCommandFile(_engine,
                                      @"class Command(BaseIronPythonCommand):
  def GetName(self): return ""Name""
  def Execute(self, args): return 1");

            Assert.Equal(1, commandFile.Count());
            Assert.Equal("Name", commandFile.First().Name);
        }

        [Fact]
        public void Name_For_Class_With_CamelCase_Name_Has_Spaces_Separating_Words()
        {
            var commandFile = new IronPythonCommandFile(_engine,
                                      @"class CommandName(BaseIronPythonCommand):
  def Execute(self, args): return 1");

            Assert.Equal("Command Name", commandFile.First().Name);
        }
        
        [Fact]
        public void Can_Call_GetName_With_Two_Arguments()
        {
            var commandFile = new IronPythonCommandFile(_engine,
                                      @"class CommandName(BaseIronPythonCommand):
  def GetName(self): return ""Name""
  def Execute(self, args): return");

            var command = commandFile.First();
            Assert.Equal("Name",command.GetName(""));
        } 
        
        [Fact]
        public void Can_Create_Command_From_Method()
        {
            var commandFile = new IronPythonCommandFile(_engine,
                                      @"def CommandName(args):
  return ""result""");

            Assert.True(commandFile.Any());
            var command = commandFile.First();
            Assert.Equal("Command Name",command.Name);
        }
        
        [Fact]
        public void Ignores_Methods_Starting_With_Underscore()
        {
            var commandFile = new IronPythonCommandFile(_engine,
                                      @"def _CommandName(args):
  return ""result""");

            Assert.False(commandFile.Any());
        }

        public IronPythonCommandFile ForCode(string code)
        {
            return new IronPythonCommandFile(_engine, code);
        }

        private ScriptEngine _engine;

        public IronPythonSimpleCommands()
        {
            _engine = Python.CreateEngine();
        }
    }

    
}
