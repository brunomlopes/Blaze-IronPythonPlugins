using System;
using System.Linq;
using Microsoft.Scripting;

namespace IronPythonPlugins
{
    public class PythonException : Exception
    {
        public PythonException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class SyntaxErrorExceptionPrettyWrapper: PythonException
    {
        private readonly SyntaxErrorException _innerException;
        public SyntaxErrorExceptionPrettyWrapper(string message, SyntaxErrorException innerException)
            : base(message, innerException)
        {
            _innerException = innerException;
        }

        public override string Message
        {
            get
            {
                return string.Format("{4}\nLine {0}\n{1}\n{2}^---{3}", _innerException.Line, 
                    _innerException.GetCodeLine(),
                                     string.Join("", Enumerable.Repeat(" ", _innerException.Column).ToArray()),
                                     _innerException.Message,
                                     base.Message);
            }
        }
    }
}