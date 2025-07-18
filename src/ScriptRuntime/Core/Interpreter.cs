using ScriptRuntime.Runtime;
using ScriptRuntime.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScriptRuntime.Core
{
    public class Interpreter
    {
        public static bool VariableEquals(VariableValue left, VariableValue right)
        {
            // 类型不同立即返回false
            if (left.VarType != right.VarType)
                return false;

            // 根据类型执行严格比较
            switch (left.VarType)
            {
                case ValueType.NUM:
                    return (double)left.Value == (double)right.Value;

                case ValueType.BOOL:
                    return (bool)left.Value == (bool)right.Value;

                case ValueType.STRING:
                    return (string)left.Value == (string)right.Value;

                case ValueType.ARRAY:
                    return object.ReferenceEquals(left.Value, right.Value); // 数组比较引用
                case ValueType.ANY:
                    return object.ReferenceEquals(left.Value, right.Value);
                default:
                    return false; // 未知类型
            }
        }
        public static Dictionary<string, VariableValue> ConstValue = new Dictionary<string, VariableValue>()
        {
            {"nullptr",new VariableValue(ValueType.PTR,(nint)0){ReadOnly = true } },
            {"null",FunctionManager.EmptyVariable },
            {"PI",new VariableValue(ValueType.NUM,(double)3.14159265){ReadOnly = true } }
        };
        static VariableValue GetVariable(string name, Dictionary<string, VariableValue> localVariable)
        {
            if(ConstValue.ContainsKey(name))
            {
                return ConstValue[name];
            }
            else if (localVariable.ContainsKey(name))
            {
                return localVariable[name];
            }
            else
            {
                throw new ScriptException("未找到变量：" + name);
            }
        }
        static void SetVariable(string name,VariableValue value, Dictionary<string, VariableValue> localVariable)
        {
            if (localVariable.ContainsKey(name))
            {
                localVariable[name] = value;
            }
            else
            {
                localVariable.Add(name, value);
            }
        }
        static VariableValue ExecCodeBlock(ASTNode root, Dictionary<string, VariableValue> localVariable, bool clearLocalVar = true)
        {
            var retn = FunctionManager.EmptyVariable;
            List<string> backup = new List<string>(localVariable.Keys);
            //拆解AST
            //Parser.PrintAST(root);
            //开始执行
            foreach (var node in root.Childrens)
            {
                ExecuteAST(node,localVariable);
            }
            
            //处理执行出来的变量
            if (clearLocalVar)
            {
                foreach (var variable in localVariable.ToList()) //销毁局部变量
                {
                    if (!backup.Contains(variable.Key))
                    {
                        localVariable.Remove(variable.Key);
                    }
                }
            }
            return FunctionManager.EmptyVariable;
        }
        static VariableValue ExecUnaryOperator(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            if(root.Raw == "!")
            {
                var val = ExecuteAST(root.Childrens[0], localVariable);
                if (val.VarType != ValueType.BOOL)
                    throw new ScriptBaseException("运算符!类型不匹配，实际类型：" + val.VarType);
                val.Value = !(bool)val.Value;
                return val;
            }
            else if (root.Raw == "+")
            {
                var val = ExecuteAST(root.Childrens[0], localVariable);
                if (val.VarType != ValueType.NUM) 
                    throw new ScriptBaseException("运算符+类型不匹配，实际类型：" + val.VarType);
                val.Value = +(double)val.Value;
                return val;
            }
            else if(root.Raw == "-")
            {
                var val = ExecuteAST(root.Childrens[0],localVariable);
                if (val.VarType != ValueType.NUM)
                    throw new ScriptBaseException("运算符+类型不匹配，实际类型：" + val.VarType);
                val.Value = -(double)val.Value;
                return val;
            }
            else if(root.Raw == "++")
            {
                VariableValue variable;
                if (root.Childrens[0].NodeType == ASTNode.ASTNodeType.ArrayLabel)
                {
                    variable = ExecArrayLabel(root.Childrens[0],localVariable);
                }
                else
                {
                    variable = GetVariable(root.Childrens[0].Raw, localVariable);   
                }
                if (variable.VarType != ValueType.NUM)
                    throw new ScriptBaseException("运算符++类型不匹配，实际类型：" + variable.VarType);
                variable.Value = (double)variable.Value + 1;
                return variable;
            }
            else if (root.Raw == "--")
            {
                VariableValue variable;
                if (root.Childrens[0].NodeType == ASTNode.ASTNodeType.ArrayLabel)
                {
                    variable = ExecArrayLabel(root.Childrens[0], localVariable);
                }
                else
                {
                    variable = GetVariable(root.Childrens[0].Raw, localVariable);
                }
                if (variable.VarType != ValueType.NUM)
                    throw new ScriptBaseException("运算符--类型不匹配，实际类型：" + variable.VarType);
                variable.Value = (double)variable.Value - 1;
                return variable;
            }
            else
            {
                throw new ScriptException("不支持的一元运算符");
            }
        }
        static VariableValue ExecVariableDefination(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            if(localVariable.ContainsKey(root.Raw))
            {
                throw new ScriptException("变量重复定义：" + root);
            }
            else
            {
                localVariable.Add(root.Raw, new VariableValue(ValueType.NULL,null));
            }
            return FunctionManager.EmptyVariable;
        }
       
        static void ExecReturnStatement(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            throw new ReturnException(ExecuteAST(root.Childrens[0], localVariable));
        }
        static VariableValue ExecAssignment(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            if (root.Raw == "=")
            {
                var target = ExecuteAST(root.Childrens[0],localVariable);
                var val = ExecuteAST(root.Childrens[1], localVariable);
                target.Value = val.Value;
                target.VarType = val.VarType;
                return target;
            }
            else if (root.Raw == "+=")
            {
                var target = ExecuteAST(root.Childrens[0], localVariable);
                var val = ExecuteAST(root.Childrens[1], localVariable);

                if (target.VarType == ValueType.NUM && val.VarType == ValueType.NUM)
                {
                    target.Value = (double)target.Value + (double)val.Value;
                }
                else
                {
                    target.Value = target.Value.ToString() + val.Value.ToString();
                    target.VarType = ValueType.STRING;
                }
                return target;
            }
            else if (root.Raw == "-=")
            {
                var target = ExecuteAST(root.Childrens[0], localVariable);
                var val = ExecuteAST(root.Childrens[1], localVariable);

                if (target.VarType != ValueType.NUM || val.VarType != ValueType.NUM)
                {
                    throw new ScriptException("-=类型不匹配，实际类型：" + val.VarType);
                }
                target.Value = (double)target.Value - (double)val.Value;
                return target;
            }
            else if (root.Raw == "*=")
            {
                var target = ExecuteAST(root.Childrens[0], localVariable);
                var val = ExecuteAST(root.Childrens[1], localVariable);

                if (target.VarType != ValueType.NUM || val.VarType != ValueType.NUM)
                {
                    throw new ScriptException("*=类型不匹配，实际类型：" + val.VarType);
                }
                target.Value = (double)target.Value * (double)val.Value;
                return target;
            }
            else if (root.Raw == "/=")
            {
                var target = ExecuteAST(root.Childrens[0], localVariable);
                var val = ExecuteAST(root.Childrens[1], localVariable);

                if (target.VarType != ValueType.NUM || val.VarType != ValueType.NUM)
                {
                    throw new ScriptException("/=类型不匹配，实际类型：" + val.VarType);
                }
                target.Value = (double)target.Value / (double)val.Value;
                return target;
            }
            else if (root.Raw == "%=")
            {
                var target = ExecuteAST(root.Childrens[0], localVariable);
                var val = ExecuteAST(root.Childrens[1], localVariable);

                if (target.VarType != ValueType.NUM || val.VarType != ValueType.NUM)
                {
                    throw new ScriptException("%=类型不匹配，实际类型：" + val.VarType);
                }
                target.Value = (double)target.Value % (double)val.Value;
                return target;
            }
            else if (root.Raw == "&=")
            {
                var target = ExecuteAST(root.Childrens[0], localVariable);
                var val = ExecuteAST(root.Childrens[1], localVariable);

                if (target.VarType != ValueType.NUM || val.VarType != ValueType.NUM)
                {
                    throw new ScriptException("&=类型不匹配，实际类型：" + val.VarType);
                }
                target.Value = (int)(double)target.Value & (int)(double)val.Value;
                return target;
            }
            else if (root.Raw == "|=")
            {
                var target = ExecuteAST(root.Childrens[0], localVariable);
                var val = ExecuteAST(root.Childrens[1], localVariable);

                if (target.VarType != ValueType.NUM || val.VarType != ValueType.NUM)
                {
                    throw new ScriptException("|=类型不匹配，实际类型：" + val.VarType);
                }
                target.Value = (int)(double)target.Value | (int)(double)val.Value;
                return target;
            }
            else if (root.Raw == "^=")
            {
                var target = ExecuteAST(root.Childrens[0], localVariable);
                var val = ExecuteAST(root.Childrens[1], localVariable);

                if (target.VarType != ValueType.NUM || val.VarType != ValueType.NUM)
                {
                    throw new ScriptException("^=类型不匹配，实际类型：" + val.VarType);
                }
                target.Value = (int)(double)target.Value ^ (int)(double)val.Value;
                return target;
            }
            else if (root.Raw == "<<=")
            {
                var target = ExecuteAST(root.Childrens[0], localVariable);
                var val = ExecuteAST(root.Childrens[1], localVariable);

                if (target.VarType != ValueType.NUM || val.VarType != ValueType.NUM)
                {
                    throw new ScriptException("<<=类型不匹配，实际类型：" + val.VarType);
                }
                target.Value = (int)(double)target.Value << (int)(double)val.Value;
                return target;
            }
            else if (root.Raw == ">>=")
            {
                var target = ExecuteAST(root.Childrens[0], localVariable);
                var val = ExecuteAST(root.Childrens[1], localVariable);

                if (target.VarType != ValueType.NUM || val.VarType != ValueType.NUM)
                {
                    throw new ScriptException(">>=类型不匹配，实际类型：" + val.VarType);
                }
                target.Value = (int)(double)target.Value >> (int)(double)val.Value;
                return target;
            }
            else
            {
                throw new ScriptException($"未知的赋值运算符: {root.Raw}");
            }
        }
        static VariableValue ExecIfStatement(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            var condResult = ExecuteAST(root.Childrens[0], localVariable);
            if (condResult.VarType != ValueType.BOOL)
                throw new ScriptException("if语句条件类型不为BOOL");
            if ((bool)condResult.Value)
            {
                ExecuteAST(root.Childrens[1], localVariable);
            }
            else
            {
                if(root.Childrens.Count > 2)
                {
                    ExecuteAST(root.Childrens[2], localVariable);
                }
            }
            
            return FunctionManager.EmptyVariable;
        }
        static VariableValue ExecWhileStatement(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            var condResult = ExecuteAST(root.Childrens[0], localVariable);
            while(condResult.VarType == ValueType.BOOL && (bool)condResult.Value)
            {
                try
                {
                    ExecuteAST(root.Childrens[1], localVariable);
                }
                catch(BreakException ex)
                {
                    break;
                }
                catch(ContuineException ex)
                {
                    continue;
                }
                condResult = ExecuteAST(root.Childrens[0], localVariable);
            }
            return FunctionManager.EmptyVariable;
        }
        static VariableValue ExecForEachStatement(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            var forEachVar = root.Childrens[0].Raw;
            var array = ExecuteAST(root.Childrens[1],localVariable);
            if (array.VarType != ValueType.ARRAY)
                throw new ScriptException("for-each循环表达式结果不为集合");
            localVariable.Add(forEachVar,FunctionManager.EmptyVariable);
            foreach(var child in (List<VariableValue>)array.Value)
            {
                SetVariable(forEachVar,child,localVariable);
                try
                {
                    ExecuteAST(root.Childrens[2], localVariable);
                }
                catch(BreakException ex)
                {
                    break;
                }
                catch(ContuineException ex)
                {
                    break;
                }
            }
            localVariable.Remove(forEachVar);
            return FunctionManager.EmptyVariable;
        }
        static VariableValue ExecTryCatchStatement(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            try
            {
                ExecuteAST(root.Childrens[0],localVariable);
            }
            catch(ScriptException ex)
            {
                if (localVariable.ContainsKey(root.Childrens[1].Raw))
                {
                    throw new ScriptException($"变量{root.Childrens[1].Raw}已存在");
                }
                localVariable.Add(root.Childrens[1].Raw, new VariableValue(ValueType.STRING, ex.Message));
                ExecuteAST(root.Childrens[2], localVariable);
                localVariable.Remove(root.Childrens[1].Raw);
            }
            return FunctionManager.EmptyVariable;
        }
        static VariableValue ExecFuncDefStatement(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            List<string> args = new List<string>();
            for(int i = 0;i < root.Childrens.Count - 1;i++)
            {
                args.Add(root.Childrens[i].Raw);
            }
            if (root.Raw != string.Empty)
            {
                return new VariableValue(ValueType.FUNCTION, FunctionManager.RegisterFunction(root.Raw, args, root.Childrens.Last()));
            }
            else
            {
                var argTypes = new List<ValueType>();
                for (int i = 0; i < args.Count; i++)
                {
                    argTypes.Add(ValueType.ANY);
                }
                return new VariableValue(ValueType.FUNCTION, new ScriptFunction(string.Empty, argTypes, args, ValueType.ANY, root.Childrens.Last()));
            }
        }
        static VariableValue ExecFunctionCall(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {   
            List<VariableValue> args = new List<VariableValue>();
            var targetFunctionIdentifier = root.Childrens[0];
            for (int i = 1; i < root.Childrens.Count; i++)
            {
                var ast = root.Childrens[i];
                args.Add(ExecuteAST(ast, localVariable));
            }
            if (targetFunctionIdentifier.NodeType == ASTNode.ASTNodeType.Identifier 
                && FunctionManager.FunctionTable.ContainsKey(targetFunctionIdentifier.Raw))
            {
                //如果是直接的标识符就说明是全局函数，直接从全局函数表寻找函数调用
                return FunctionManager.CallFunction(targetFunctionIdentifier.Raw, args);
            }
            var result = ExecuteAST(targetFunctionIdentifier, localVariable);
            if(result.VarType != ValueType.FUNCTION)
            {
                throw new ScriptException("非函数类型进行函数调用");
            }
            ScriptFunction targetFunction = (ScriptFunction)result.Value;
            var retn = targetFunction.Invoke(args, result.Parent);
            return retn;
        }
        static VariableValue ExecLeftUnaryOperator(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            if (root.Raw == "++")
            {
                VariableValue variable;
                if (root.Childrens[0].NodeType == ASTNode.ASTNodeType.ArrayLabel)
                {
                    variable = ExecArrayLabel(root.Childrens[0],localVariable);
                }
                else 
                {
                    variable = GetVariable(root.Childrens[0].Raw, localVariable);
                }
                if (variable.VarType != ValueType.NUM)
                    throw new ScriptBaseException("运算符++类型不匹配，实际类型：" + variable.VarType);
                double ret = (double)variable.Value;
                variable.Value = ret + 1;
                return new VariableValue(ValueType.NUM, ret);
            }
            else if (root.Raw == "--")
            {
                VariableValue variable;
                if (root.Childrens[0].NodeType == ASTNode.ASTNodeType.ArrayLabel)
                {
                    variable = ExecArrayLabel(root.Childrens[0], localVariable);
                }
                else
                {
                    variable = GetVariable(root.Childrens[0].Raw, localVariable);
                }
                if (variable.VarType != ValueType.NUM)
                    throw new ScriptBaseException("运算符--类型不匹配，实际类型：" + variable.VarType);
                double ret = (double)variable.Value;
                variable.Value = ret - 1;
                return new VariableValue(ValueType.NUM,ret);
            }
            else
            {
                throw new ScriptException("不支持的一元运算符");
            }
        }
        static VariableValue ExecBinaryOperator(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            var left = ExecuteAST(root.Childrens[0], localVariable);
            var right = ExecuteAST(root.Childrens[1], localVariable);

            if (root.Raw == "+")
            {
                if (left.VarType == ValueType.NUM && right.VarType == ValueType.NUM)
                {
                    return new VariableValue(ValueType.NUM, (double)left.Value + (double)right.Value);
                }
                else
                {
                    return new VariableValue(ValueType.STRING, left.Value.ToString() + right.Value.ToString());
                }
            }
            else if (root.Raw == "-")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($"-类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.NUM, (double)left.Value - (double)right.Value);
            }
            else if (root.Raw == "*")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($"*类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.NUM, (double)left.Value * (double)right.Value);
            }
            else if (root.Raw == "/")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($"/类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.NUM, (double)left.Value / (double)right.Value);
            }
            else if (root.Raw == "%")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($"%类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.NUM, (double)left.Value % (double)right.Value);
            }
            else if (root.Raw == "|")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($"|类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.NUM, (double)((long)(double)left.Value | (long)(double)right.Value));
            }
            else if (root.Raw == "^")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($"^类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.NUM, (double)((long)(double)left.Value ^ (long)(double)right.Value));
            }
            else if (root.Raw == "&")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($"&类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.NUM, (double)((long)(double)left.Value & (long)(double)right.Value));
            }
            else if (root.Raw == ">>")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($">>类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.NUM, (double)((int)(double)left.Value >> (int)(double)right.Value));
            }
            else if (root.Raw == "<<")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($"<<类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.NUM, (double)((int)(double)left.Value << (int)(double)right.Value));
            }
            else if (root.Raw == "<")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($"<类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.BOOL, (double)left.Value < (double)right.Value);
            }
            else if (root.Raw == ">")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($">类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.BOOL, (double)left.Value > (double)right.Value);
            }
            else if (root.Raw == ">=")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($">=类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.BOOL, (double)left.Value >= (double)right.Value);
            }
            else if (root.Raw == "<=")
            {
                if (left.VarType != ValueType.NUM || right.VarType != ValueType.NUM)
                    throw new ScriptException($"<=类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.BOOL, (double)left.Value <= (double)right.Value);
            }
            else if (root.Raw == "==")
            {
                return new VariableValue(ValueType.BOOL, VariableEquals(left, right));
            }
            else if (root.Raw == "!=")
            {
                return new VariableValue(ValueType.BOOL, !VariableEquals(left, right));
            }
            else if (root.Raw == "&&")
            {
                if (left.VarType != ValueType.BOOL || right.VarType != ValueType.BOOL)
                    throw new ScriptException($"&&类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.BOOL, (bool)left.Value && (bool)right.Value);
            }
            else if (root.Raw == "||")
            {
                if (left.VarType != ValueType.BOOL || right.VarType != ValueType.BOOL)
                    throw new ScriptException($"||类型不匹配，type(left)={left.VarType} type(right)={right.VarType}");
                return new VariableValue(ValueType.BOOL, (bool)left.Value || (bool)right.Value);
            }
            else
            {
                throw new ScriptException("不支持的二元运算符: " + root.Raw);
            }
        }
        static VariableValue ExecArray(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            var list = new List<VariableValue>();
            foreach(var child in root.Childrens)
            {
                list.Add(ExecuteAST(child,localVariable));
            }
            return new VariableValue(ValueType.ARRAY,list);
        }
        static VariableValue ExecArrayLabel(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            var left = ExecuteAST(root.Childrens[0], localVariable);
            //var left = GetVariable(leftResult.Value.ToString(),localVariable);
            if(left.VarType != ValueType.ARRAY && left.VarType != ValueType.OBJECT)
            {
                throw new ScriptException($"[]下标操作无法应用于{AOTEnumMap.ValueTypeString[left.VarType]}类型");
            }
            var index = ExecuteAST(root.Childrens[1],localVariable);
            if(left.VarType == ValueType.ARRAY)
            {
                if (index.VarType != ValueType.NUM)
                {
                    throw new ScriptException("无法使用非数字类型作为集合索引");
                }
                return ((List<VariableValue>)left.Value)[(int)(double)index.Value];
            }
            else
            {
                if(index.VarType != ValueType.STRING)
                {
                    throw new ScriptException("无法使用非数字类型作为对象索引");
                }
                return ((Dictionary<string, VariableValue>)left.Value)[(string)index.Value];
            } 
        }
        static VariableValue ExecObjectParse(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            Dictionary<string, VariableValue> ObjContainer = new Dictionary<string, VariableValue>();
            VariableValue retn = new VariableValue(ValueType.OBJECT, ObjContainer);
            foreach (var item in root.Childrens)
            {
                if(item.NodeType != ASTNode.ASTNodeType.KeyValuePair)
                {
                    throw new ScriptException("对象定义解析出错");
                }
                var key = item.Childrens[0].Raw;
                var value = ExecuteAST(item.Childrens[1],localVariable);
                value.Parent = retn;
                ObjContainer.Add(key, value);
            }
            return retn;
        }
        static VariableValue ExecMemberAccess(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            var parent = ExecuteAST(root.Childrens[0],localVariable);
            if (parent.VarType == ValueType.OBJECT)
            {
                Dictionary<string, VariableValue> objContainer = (Dictionary<string, VariableValue>)parent.Value;
                VariableValue? result;
                if (objContainer.TryGetValue(root.Childrens[1].Raw, out result))
                {
                    return result;
                }
                else if (ObjectManager.ObjectFunctions.ContainsKey(root.Childrens[1].Raw))
                {
                    var retn = new VariableValue(ValueType.FUNCTION, ObjectManager.ObjectFunctions[root.Childrens[1].Raw]);
                    retn.Parent = parent;
                    return retn;
                }
                throw new ScriptException($"不存在成员 {root.Childrens[1].Raw}");
            }
            else if(parent.VarType == ValueType.ARRAY)
            {
                if (ArrayManager.ArrayFunctions.ContainsKey(root.Childrens[1].Raw))
                {
                    var retn = new VariableValue(ValueType.FUNCTION, ArrayManager.ArrayFunctions[root.Childrens[1].Raw]);
                    retn.Parent = parent;
                    return retn;
                }
                throw new ScriptException($"不存在成员 {root.Childrens[1].Raw}");
            }
            else if(parent.VarType == ValueType.STRING)
            {
                if (StringManager.StringFunctions.ContainsKey(root.Childrens[1].Raw))
                {
                    var retn = new VariableValue(ValueType.FUNCTION, StringManager.StringFunctions[root.Childrens[1].Raw]);
                    retn.Parent = parent;
                    return retn;
                }
                throw new ScriptException($"不存在成员 {root.Childrens[1].Raw}");
            }
            else if(parent.VarType == ValueType.PTR)
            {
                if (PtrManager.PtrFunctions.ContainsKey(root.Childrens[1].Raw))
                {
                    var retn = new VariableValue(ValueType.FUNCTION, PtrManager.PtrFunctions[root.Childrens[1].Raw]);
                    retn.Parent = parent;
                    return retn;
                }
                throw new ScriptException($"不存在成员 {root.Childrens[1].Raw}");
            }
            else
            {
                throw new ScriptException($"{AOTEnumMap.ValueTypeString[parent.VarType]}类型无法进行成员访问");
            }
            
        }
        public static VariableValue ExecuteAST(ASTNode root, Dictionary<string, VariableValue> localVariable)
        {
            switch (root.NodeType)
            {
                case ASTNode.ASTNodeType.BlockCode: return ExecCodeBlock(root,localVariable);
                case ASTNode.ASTNodeType.UnaryOperation: return ExecUnaryOperator(root,localVariable);
                case ASTNode.ASTNodeType.LeftUnaryOperation: return ExecLeftUnaryOperator(root,localVariable);
                case ASTNode.ASTNodeType.BinaryOperation: return ExecBinaryOperator(root,localVariable);
                case ASTNode.ASTNodeType.ArrayLabel: return ExecArrayLabel(root,localVariable);
                case ASTNode.ASTNodeType.VariableDefination: return ExecVariableDefination(root,localVariable);
                case ASTNode.ASTNodeType.Assignment: return ExecAssignment(root,localVariable);
                case ASTNode.ASTNodeType.Number: return new VariableValue(ValueType.NUM, double.Parse(root.Raw));
                case ASTNode.ASTNodeType.StringValue:return new VariableValue(ValueType.STRING, root.Raw);
                case ASTNode.ASTNodeType.Array: return ExecArray(root,localVariable);
                case ASTNode.ASTNodeType.Boolean: return new VariableValue(ValueType.BOOL, bool.Parse(root.Raw));
                case ASTNode.ASTNodeType.BreakStatement: throw new BreakException();
                case ASTNode.ASTNodeType.ContinueStatement: throw new ContuineException();
                case ASTNode.ASTNodeType.ReturnStatement: ExecReturnStatement(root,localVariable);break;
                case ASTNode.ASTNodeType.Identifier: return GetVariable(root.Raw,localVariable);
                case ASTNode.ASTNodeType.IfStatement:return ExecIfStatement(root,localVariable);
                case ASTNode.ASTNodeType.WhileStatement: return ExecWhileStatement(root,localVariable);
                case ASTNode.ASTNodeType.ForEachStatement: return ExecForEachStatement(root,localVariable);
                case ASTNode.ASTNodeType.FunctionDefinition:return ExecFuncDefStatement(root,localVariable);
                case ASTNode.ASTNodeType.CallFunction:return ExecFunctionCall(root,localVariable);
                case ASTNode.ASTNodeType.TryCatchStatement:return ExecTryCatchStatement(root,localVariable);
                case ASTNode.ASTNodeType.Object: return ExecObjectParse(root,localVariable);
                case ASTNode.ASTNodeType.MemberAccess: return ExecMemberAccess(root,localVariable);
            }
            throw new SyntaxException("不支持的ASTNode类型", $"{AOTEnumMap.ASTNodeTypeString[root.NodeType]} {root.Raw}");
        }

        //执行代码块记得try一下break和contuine异常
        public static VariableValue ExecuteBlock(ASTNode root, Dictionary<string, VariableValue> localVariable, bool clearLocalVar = true)
        {
            return ExecCodeBlock(root,localVariable,clearLocalVar);
        }
    }
}
