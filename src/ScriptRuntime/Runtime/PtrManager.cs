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
            {"move",new ScriptFunction("move",
                    new List<ValueType>(){ValueType.NUM},
                    ValueType.PTR,PtrMove) },
            {"copy",new ScriptFunction("copy",
                    new List<ValueType>(),
                    ValueType.PTR,PtrCopy) },
        };

        static unsafe VariableValue PtrCopy(List<VariableValue> args, VariableValue thisValue)
        {
            nint dest = 0;
            var p = &dest;
            *p = (nint)thisValue.Value;
            return new VariableValue(ValueType.PTR,dest);
        }
        static VariableValue PtrMove(List<VariableValue> args, VariableValue thisValue)
        {
            thisValue.Value = (nint)thisValue.Value + (int)(double)args[0].Value;
            return thisValue;
        }
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
