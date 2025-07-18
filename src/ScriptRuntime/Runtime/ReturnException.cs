using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptRuntime.Core;

namespace ScriptRuntime.Runtime
{
    public class ReturnException : Exception
    {
        public VariableValue ReturnValue;
        public ReturnException(VariableValue value)
        {
            ReturnValue = value;
        }
    }

    public class BreakException : Exception
    {
        public object Tag;
    }
    public class ContuineException : Exception
    {
        public object Tag;
    }
}