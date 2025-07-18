using ScriptRuntime.Core;
using ScriptRuntime.FFI;
using ScriptRuntime.Utils;

namespace ScriptRuntime.Runtime
{
    public class ObjectManager
    {
        public static Dictionary<string, ScriptFunction> ObjectFunctions = new Dictionary<string, ScriptFunction>()
        {
            {"addMember",new ScriptFunction("addMember",
                    new List<Core.ValueType>(){Core.ValueType.STRING,Core.ValueType.ANY},
                    Core.ValueType.ANY,ObjectAddMember) },
            {"removeMember",new ScriptFunction("removeMember",
                    new List<Core.ValueType>(){Core.ValueType.STRING},
                    Core.ValueType.NULL,ObjectRemoveMember) },
            {"values",new ScriptFunction("values",
                    new List<Core.ValueType>(),
                    Core.ValueType.ARRAY,Values) },
            {"keys",new ScriptFunction("keys",
                    new List<Core.ValueType>(),
                    Core.ValueType.ARRAY,Keys) },
            {"toPointer",new ScriptFunction("toPointer",
                    new List<Core.ValueType>(){Core.ValueType.STRING},
                    Core.ValueType.PTR,ObjectToPtr) },
        };
        public static VariableValue Values(List<VariableValue> args, VariableValue thisValue)
        {
            var objContainer = (Dictionary<string, VariableValue>)thisValue.Value;
            List<VariableValue> values = new List<VariableValue>();
            foreach (var arg in objContainer)
            {
                values.Add(arg.Value);
            }
            return new VariableValue(Core.ValueType.ARRAY,values);
        }
        public static VariableValue Keys(List<VariableValue> args, VariableValue thisValue)
        {
            var objContainer = (Dictionary<string, VariableValue>)thisValue.Value;
            List<VariableValue> keys = new List<VariableValue>();
            foreach (var arg in objContainer)
            {
                keys.Add(new VariableValue(Core.ValueType.STRING,arg.Key));
            }
            return new VariableValue(Core.ValueType.ARRAY, keys);
        }
        public static VariableValue ObjectToPtr(List<VariableValue> args, VariableValue thisValue)
        {
            return FFIManager.FFIObjectToPtr(thisValue, (string)args[0].Value);
        }
        public static VariableValue ObjectAddMember(List<VariableValue> args,VariableValue thisValue)
        {
            var name = args[0].Value.ToString();
            var value = args[1];
            var objContainer = (Dictionary<string, VariableValue>)thisValue.Value;
            objContainer.Add(name, value);
            return value;
        }
        public static VariableValue ObjectRemoveMember(List<VariableValue> args, VariableValue thisValue)
        {
            var name = args[0].Value.ToString();
            var objContainer = (Dictionary<string, VariableValue>)thisValue.Value;
            if (!objContainer.Remove(name))
            {
                throw new ScriptException(name + " is not found in the object.");
            }
            return FunctionManager.EmptyVariable;
        }
    }
}