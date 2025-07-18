using ScriptRuntime.Core;
using ScriptRuntime.FFI;
using ValueType = ScriptRuntime.Core.ValueType; 

namespace ScriptRuntime.Runtime
{
    public class ArrayManager
    {
        public static Dictionary<string, ScriptFunction> ArrayFunctions = new Dictionary<string, ScriptFunction>()
        {
            {"add",new ScriptFunction("add",
                    new List<ValueType>(){ValueType.ANY},
                    ValueType.ARRAY,ArrayAdd) },
            {"length",new ScriptFunction("length",
                    new List<ValueType>(),
                    ValueType.NUM,ArrayLen) },
            {"insert",new ScriptFunction("insert",
                    new List<ValueType>(){ ValueType.NUM,ValueType.ANY},
                    ValueType.ARRAY,ArrayInsert) },
            {"remove",new ScriptFunction("remove",
                    new List<ValueType>(){ ValueType.NUM},
                    ValueType.ARRAY,ArrayRemove) },
            {"sort",new ScriptFunction("sort",
                    new List<ValueType>(),
                    ValueType.ARRAY,ArraySort) },
        };
        public static VariableValue ArraySort(List<VariableValue> args, VariableValue thisValue)
        {
            var list = (List<VariableValue>)thisValue.Value;
            list.Sort();
            return thisValue;
        }
        public static VariableValue ArrayLen(List<VariableValue> args, VariableValue thisValue) => new(ValueType.NUM, (double)((List<VariableValue>)thisValue.Value).Count);
        public static VariableValue ArrayAdd(List<VariableValue> args, VariableValue thisValue)
        {
            ((List<VariableValue>)thisValue.Value).Add(args[0]);
            return thisValue;
        }
        public static VariableValue ArrayInsert(List<VariableValue> args, VariableValue thisValue)
        {
            if ((int)(double)args[0].Value >= ((List<VariableValue>)thisValue.Value).Count ||
                (int)(double)args[0].Value < 0)
            {
                throw new ScriptException($"数组越界 len={((List<VariableValue>)thisValue.Value).Count} index={(int)(double)args[0].Value}");
            }
            ((List<VariableValue>)thisValue.Value).Insert((int)(double)args[0].Value, args[1]);
            return thisValue;
        }
        public static VariableValue ArrayRemove(List<VariableValue> args, VariableValue thisValue)
        {
            if ((int)(double)args[0].Value >= ((List<VariableValue>)thisValue.Value).Count ||
                (int)(double)args[0].Value < 0)
            {
                throw new ScriptException($"数组越界 len={((List<VariableValue>)thisValue.Value).Count} index={(int)(double)args[0].Value}");
            }
            ((List<VariableValue>)thisValue.Value).RemoveAt((int)(double)args[0].Value);
            return thisValue;
        }
    }
}