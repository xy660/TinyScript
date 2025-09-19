/*
 * Copyright 2025 xy660
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Text;
using static StringUtils;
using static SyntaxUtils;
using static ScriptRuntime.Core.Lexer;
using static ScriptRuntime.Core.Parser;
using ScriptRuntime.Core;
using ScriptRuntime.Runtime;
using ScriptRuntime.Utils;
public class Program
{
    public static bool DEBUG = false;

    
    public static void DbgPrint(string s)
    {
        if (DEBUG)
            Console.WriteLine("[DEBUG]  " + s);
    }
    public static void REPLMain(string[] args)
    {
        Dictionary<string, VariableValue> localVariable = new Dictionary<string, VariableValue>();

        SystemFunctions.InitSystemFunction(); //启动后初始化系统函数

        //将当前主线程注册到线程上下文
        TaskContext.ThreadContext.Add(TaskContext.GetCurrentThreadId(), new TaskContext());

        if (args.Length > 0) //如果从文件加载
        {
            try
            {
                string path = args[0];
                var codeLines = CleanCode(ClearMultiSpace(File.ReadAllText(path)));
                var ast = BuildASTByTokens(SplitTokens(codeLines));
                var mainFunc = FunctionManager.RegisterFunction("main", new List<string>(), ast);
                FunctionManager.CallFunction("main", new List<VariableValue>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Stack:\r\n" + GenerateStackTrace(TaskContext.ThreadContext[TaskContext.GetCurrentThreadId()].StackTrace));
                Console.ReadLine();
            }
            Console.ReadKey();
            return;
        }
        Console.WriteLine("TinyScript REPL CLI Version V" + Interpreter.VersionString);

        Console.WriteLine();
        while (true)
        {
            Console.Write("TinyScript> ");
            string script = Console.ReadLine();
            try
            {
                if (script == "\\ast")
                {
                    while (true)
                    {
                        Console.Write("TinyScript.ASTView> ");
                        string inputCode = Console.ReadLine();
                        if (inputCode == "\\exit")
                        {
                            break;
                        }
                        try
                        {
                            PrintAST(BuildASTByTokens(SplitTokens(inputCode)));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("无法解析AST：" + ex.Message);
                        }
                    }
                }
                else if (script == "\\vars")
                {
                    Console.WriteLine("---Variables---");
                    foreach (var val in localVariable)
                    {
                        Console.Write($"Name={val.Key}  Value=");
                        SystemFunctions.PrintLine(new List<VariableValue>() { val.Value }, null);
                    }
                    Console.WriteLine("---Variables End---");
                }
                else if (script == "\\envclean")
                {
                    var cpy = FunctionManager.FunctionTable.ToList();
                    foreach (var func in cpy)
                    {
                        if (func.Value.FuncType == FunctionType.Native || func.Value.FuncType == FunctionType.Local)
                        {
                            FunctionManager.FunctionTable.Remove(func.Key);
                        }
                    }
                    localVariable.Clear();
                    Console.WriteLine("Environment clean now");
                }
                else if (script == "\\funcs")
                {
                    foreach (var func in FunctionManager.FunctionTable)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("(");
                        for (int i = 0; i < func.Value.FunctionArgumentTypes.Count; i++)
                        {
                            sb.Append($"[{AOTEnumMap.ValueTypeString[func.Value.FunctionArgumentTypes[i]]}]");
                            sb.Append(func.Value.FuncType == FunctionType.Local ? func.Value.FunctionArgumentNames[i] : $"arg{i + 1}");
                            if (i != func.Value.FunctionArgumentTypes.Count - 1)
                            {
                                sb.Append(',');
                            }
                        }

                        Console.WriteLine($"[{AOTEnumMap.FunctionEnumString[func.Value.FuncType]}] {func.Key}{sb.ToString()})");
                    }
                }
                else if (script == "\\help")
                {
                    Console.WriteLine("\\ast  进入抽象语法树视图解析模式");
                    Console.WriteLine("\\exit  退出抽象语法树视图解析模式");
                    Console.WriteLine("\\vars  列出环境中所有变量");
                    Console.WriteLine("\\funcs  列出环境中所有可访问的全局函数");
                    Console.WriteLine("\\envclean  清理环境所有变量和Local/Native函数");
                }
                else
                {
                    TaskContext.ThreadContext[TaskContext.GetCurrentThreadId()].StackTrace.Clear(); //执行前清除之前的堆栈

                    Interpreter.ExecuteBlock(BuildASTByTokens(SplitTokens(script)), localVariable, false);
                }
            }
            catch (ReturnException ex)
            {
                Console.WriteLine("Return " + ex.ReturnValue.VarType + " => " + ex.ReturnValue.Value.ToString());
            }
            catch (SyntaxException ex)
            {

                Console.WriteLine("语法错误：" + ex.Message);
            }
            catch (ScriptException ex)
            {
                Console.WriteLine("执行错误：" + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("系统错误：" + ex.Message);
            }
        }
    }
    public static void Main(string[] args)
    {
        REPLMain(args);

    }
}

