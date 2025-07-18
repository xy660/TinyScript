using ScriptRuntime.FFI;
using ScriptRuntime.Core;
using System.Text;
using static StringUtils;
using ValueType = ScriptRuntime.Core.ValueType;
namespace ScriptRuntime.Runtime
{
    public static class FunctionManager
    {
        
        public static readonly VariableValue EmptyVariable = new VariableValue(ValueType.NULL, null) {ReadOnly = true };
        public static Dictionary<string, ScriptFunction> FunctionTable = new Dictionary<string, ScriptFunction>();

        //参数格式：funcName(args...){...}
        public static ScriptFunction RegisterFunction(string name, List<string> argNames, ASTNode code)
        {
            if (FunctionTable.ContainsKey(name))
            {
                throw new ScriptException("函数重定义");
            }
            var argTypes = new List<ValueType>();
            for (int i = 0; i < argNames.Count; i++)
            {
                argTypes.Add(ValueType.ANY);
            }
            var func = new ScriptFunction(name, argTypes, argNames, ValueType.ANY, code);
            FunctionTable.Add(name, func);
            return func;
        }

        public static VariableValue CallFunction(string functionName, List<VariableValue> args)
        {
            if (FunctionTable.ContainsKey(functionName)) //参数检查，确保匹配
            {
                var func = FunctionTable[functionName];
                       
                try
                {
                    return func.Invoke(args);
                }
                catch (Exception ex) //转换系统错误为脚本异常
                {
                    throw new ScriptException(ex.Message);
                }
            }
            else
            {
                throw new ScriptException("找不到函数：" + functionName);
            }
        }

        public static List<string> SplitArgSyntax(string raw)
        {
            Stack<char> bracketStack = new Stack<char>(); //括号平衡栈
            bool inString = false;
            StringBuilder sb = new StringBuilder();
            var ret = new List<string>();
            for (int i = 0; i < raw.Length; i++)
            {
                if (raw[i] == '"')
                {
                    inString = !inString;
                }
                else if (bracket.ContainsKey(raw[i]) && !inString)
                {
                    bracketStack.Push(raw[i]);
                }
                else if (bracket.ContainsValue(raw[i]) && !inString)
                {
                    if (bracket[bracketStack.Pop()] != raw[i])
                    {
                        throw new ScriptException("解析语法错误：括号错误");
                    }
                }
                if (bracketStack.Count == 0 && raw[i] == ',' && !inString) //寻找到末尾
                {
                    ret.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    //if(inString || (!inString && raw[i] != ' '))
                    if (true)
                    {
                        sb.Append(raw[i]);
                    }
                }
            }
            if (sb.Length > 0)
                ret.Add(sb.ToString());

            return ret;
        }
    }
    public enum FunctionType
    {
        Native, //原生函数（系统原生调用）
        Local,  //脚本定义函数
        System  //Runtime自带函数
    }
    
    public class ScriptFunction
    {
        public FunctionType FuncType;
        public string Name;
        public List<ValueType> FunctionArgumentTypes; //方法参数签名
        public List<string> FunctionArgumentNames; //方法参数名列表
        public ValueType ReturnType; //方法返回值类型
        public ASTNode CodeBlock; //脚本代码块
        public Func<List<VariableValue>, VariableValue,VariableValue> SystemFunctionCaller; //系统函数调用
        public string[] NativeFunctionArgdefines;
        public string NativeReturnType;
        public nint NativePointer;
        public VariableValue ThisObject = new VariableValue(ValueType.NULL,"undefined");

        //复制一个本地函数对象，全部都是复制引用
        public ScriptFunction LocalFunctionShallowClone()
        {
            return new ScriptFunction(Name,FunctionArgumentTypes,FunctionArgumentNames,ReturnType,CodeBlock);
        }
        public VariableValue Invoke(List<VariableValue> args, VariableValue thisValue = null)
        {
            if (FunctionArgumentTypes.Count != args.Count)
            {
                throw new ScriptException("参数数量不匹配，函数名称：" + this.Name);
            }
            for (int i = 0; i < args.Count; i++)
            {
                if (FunctionArgumentTypes[i] != ValueType.ANY && args[i].VarType != FunctionArgumentTypes[i])
                {
                    throw new ScriptException("参数签名不匹配，函数名称：" + this.Name);
                }
            }
            if (FuncType == FunctionType.System)
            {
                return SystemFunctionCaller.Invoke(args,thisValue);
            }
            else if (FuncType == FunctionType.Local)
            {
                var variables = new Dictionary<string, VariableValue>();
                if (thisValue != null) variables.Add("this", thisValue);
                for (int i = 0; i < FunctionArgumentNames.Count; i++)
                {
                    variables.Add(FunctionArgumentNames[i], args[i]);
                }
                try
                {
                    Interpreter.ExecuteBlock(CodeBlock, variables);
                }
                catch (ReturnException ex)
                {
                    return ex.ReturnValue;
                }
                return FunctionManager.EmptyVariable; //如果函数内没有return就返回默认空值
            }
            else if (FuncType == FunctionType.Native)
            {
                return FFIManager.CallNativeFunction(this, args);
            }
            else
            {
                throw new ScriptException("非法枚举");
            }
        }
        public ScriptFunction(string name, List<ValueType> functionArgumentTypes, ValueType returnType, Func<List<VariableValue>, VariableValue, VariableValue> systemFunctionCaller)
        {
            FuncType = FunctionType.System;
            Name = name;
            FunctionArgumentTypes = functionArgumentTypes;
            ReturnType = returnType;
            CodeBlock = null;
            FunctionArgumentNames = new List<string>();
            SystemFunctionCaller = systemFunctionCaller;
        }
        public ScriptFunction(string name, List<ValueType> functionArgumentTypes, List<string> functionArgumentNames, ValueType returnType, ASTNode codeBlock)
        {
            FuncType = FunctionType.Local;
            Name = name;
            FunctionArgumentTypes = functionArgumentTypes;
            FunctionArgumentNames = functionArgumentNames;
            ReturnType = returnType;
            CodeBlock = codeBlock;
            SystemFunctionCaller = null;
        }
        public ScriptFunction(string name, ValueType returnType, string argDefines, string returnNativeType, nint functionPtr)
        {
            FuncType = FunctionType.Native;
            Name = name;
            (FunctionArgumentTypes, NativeFunctionArgdefines) = FFIManager.NativeDefToScriptDef(argDefines);
            ReturnType = returnType;
            NativeReturnType = returnNativeType;
            NativePointer = functionPtr;
        }
    }

}