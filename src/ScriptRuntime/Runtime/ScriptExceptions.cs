using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptRuntime.Runtime
{
    //脚本异常基类
    public class ScriptBaseException : Exception
    {
        public object Tag = 0;
        public ScriptBaseException(string message) : base(message)
        {

        }
    }
    //脚本执行过程中的异常
    public class ScriptException : ScriptBaseException
    {
        public object Tag = 0;
        public ScriptException(string message) : base(message)
        {

        }
    }
    //脚本语法解析过程中的异常
    public class SyntaxException : ScriptBaseException
    {
        public object Tag = 0;
        public SyntaxException(string message,string context) : base(message + "    上下文：" + context)
        {

        }
    }

}