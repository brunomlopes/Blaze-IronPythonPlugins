using System;
using System.Linq;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPythonPlugins;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Xunit;

namespace Tests
{
    public class IronPythonCommands
    {
        [Fact]
        public void Class_With_Docstring_Has_Description_And_No_Description_Method_Uses_Docstring_As_Description()
        {
            var commandFile = new IronPythonCommandFile(_engine,
                                                        @"
class CommandName(BaseIronPythonCommand):
  """"""This is the description""""""
  def Execute(self, args):
    return ""None""
");
            var command = commandFile.First();
            Assert.Equal("This is the description", command.GetDescription(string.Empty));
            Assert.Equal("This is the description", command.Description);
        }
        
        [Fact]
        public void Output_Line_And_File_On_Syntax_Exception()
        {
            var code = @"
class CommandName(BaseIronPythonCommand):
  """"""This is the description""""""
  def Execute(self, args):
    return ""None""
";
            try
            {
                var commandFile = new IronPythonCommandFile(_engine,code);
            }
            catch (SyntaxErrorException e)
            {
                Console.WriteLine(e);
            }
        }
        
        [Fact]
        public void Syntax_Exception_Shows_Error_Line_And_Column()
        {
            var code = @"
class CommandName(BaseIronPythonCommand):
  %invalid)
";
            try
            {
                var commandFile = new IronPythonCommandFile(_engine,code);
            }
            catch (SyntaxErrorExceptionPrettyWrapper e)
            {
                var lines = e.Message.Split('\n');
                Assert.Equal("Line 3", lines[1]);
                Assert.Equal("  %invalid)", lines[2]);
                Assert.Equal("   ^---", lines[3].Substring(0,7));
            }
        }
        
        [Fact]
        public void ValueException_Shows_Error_Line_And_Column()
        {
            var code = @"
class CommandName(BaseIronPythonCommand):
  (invalid)
";
            try
            {
                var commandFile = new IronPythonCommandFile(_engine,code);
            }
            catch (PythonException e)
            {
                Assert.IsType<UnboundNameException>(e.InnerException);
            }
        }

        private ScriptEngine _engine;

        public IronPythonCommands()
        {
            _engine = Python.CreateEngine();
        }
    }
}