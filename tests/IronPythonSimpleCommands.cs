using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
            var commandFile = ForCode(@"class CommandName(BaseIronPythonCommand):
  def Execute(self): return 1");

            Assert.Equal(1, commandFile.Count());
            Assert.Equal("CommandName", commandFile.First().Name);
        }
        
        [Fact]
        public void Name_For_Class_With_GetName_Is_Result_Of_GetName()
        {
            var commandFile = new IronPythonCommandFile(_engine,
                                      @"class CommandName(BaseIronPythonCommand):
  def GetName(self): return ""Name""
  def Execute(self): return 1");

            Assert.Equal(1, commandFile.Count());
            Assert.Equal("Name", commandFile.First().Name);
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
