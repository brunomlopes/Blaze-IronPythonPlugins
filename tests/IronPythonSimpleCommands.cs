using System.Linq;
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
        public void Autocomplete_for_Command_Returns_Command_Name()
        {
            var commandFile = ForCode(@"class CommandName(BaseIronPythonCommand):
  def Execute(self, args): return 1");

            Assert.Equal(1, commandFile.Count());
            var command = commandFile.First();
            Assert.Equal(command.Name, command.GetAutoComplete("Com"));
            Assert.Equal("Command Name", command.Name);
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
        
        [Fact]
        public void DocString_Serves_As_Description_For()
        {
            var commandFile = new IronPythonCommandFile(_engine,
                                      @"def CommandName(args):
  """"""This is the description""""""
  return ""result""");
            Assert.Equal("This is the description", commandFile.First().GetDescription(string.Empty)); 
            Assert.Equal("This is the description", commandFile.First().Description); 
        }
        
        [Fact]
        public void Default_Description_Is_The_Name()
        {
            var commandFile = new IronPythonCommandFile(_engine,
                                      @"def CommandName(args):
  return ""result""");
            Assert.Equal("Command Name", commandFile.First().Description);
            Assert.Equal("Command Name", commandFile.First().GetDescription(string.Empty)); 

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
