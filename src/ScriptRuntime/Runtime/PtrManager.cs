using ScriptRuntime.Core;
using ScriptRuntime.FFI;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ValueType = ScriptRuntime.Core.ValueType;

namespace ScriptRuntime.Runtime
{
    public class PtrManager
    {
        public static Dictionary<string, ScriptFunction> PtrFunctions = new Dictionary<string, ScriptFunction>()
        {
            {"asUniString",new ScriptFunction("asUniString",
                    new List<ValueType>(),
                    ValueType.STRING,PtrToStringW) },
            {"asAnsiString",new ScriptFunction("asAnsiString",
                    new List<ValueType>(),
                    ValueType.STRING,PtrToStringW) },
            {"asObject",new ScriptFunction("asObject",
                    new List<ValueType>(){ValueType.STRING},
                    ValueType.STRING,PtrToObject) },
        };
        static VariableValue PtrToObject(List<VariableValue> args, VariableValue thisValue)
        {
            var objContainer = FFIManager.PtrToObject((nint)thisValue.Value, (string)args[0].Value); 
            return new VariableValue(ValueType.OBJECT, objContainer);
        }
        static VariableValue PtrToStringW(List<VariableValue> args, VariableValue thisValue)
        {
            var str = Marshal.PtrToStringUni((nint)thisValue.Value);
            return new VariableValue(ValueType.STRING, str is null ? "null" : str);
        }
        static VariableValue PtrToStringA(List<VariableValue> args, VariableValue thisValue)
        {
            var str = Marshal.PtrToStringAnsi((nint)thisValue.Value);
            return new VariableValue(ValueType.STRING, str is null ? "null" : str);
        }

    }
}
