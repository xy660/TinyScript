using ScriptRuntime.Core;
using ScriptRuntime.Runtime;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using ValueType = ScriptRuntime.Core.ValueType;

namespace ScriptRuntime.FFI
{
    public static class CallbackManager
    {

        public const int ArgumentHeapSize = 1024;
        public record CallbackInfo(nint addr,nint heap,int argc,ScriptFunction targetFunc,string[] nativeArgDef,string nativeRetDef,int id);

        //id寻找地址纪录类
        static Dictionary<int, CallbackInfo> Callbacks = new Dictionary<int, CallbackInfo>();

        //地址寻找回调纪录类
        public static Dictionary<nint,CallbackInfo> Ptr2Callback = new Dictionary<nint, CallbackInfo>();

        static int LastestFuncId = 0;

        static readonly Dictionary<string, TrampolineCode.ABI> ABIMapper = new Dictionary<string, TrampolineCode.ABI>()
        {
            {"stdcall",TrampolineCode.ABI.Stdcall },
            {"cdecl",TrampolineCode.ABI.Cdecl },
            {"win64",TrampolineCode.ABI.Microsoft },
            {"systemv",TrampolineCode.ABI.SystemV },
            {string.Empty,TrampolineCode.ABI.Undefined }
        };

        

        public unsafe static nint CallbackEntry(int funcId,nint* args)
        {
            CallbackInfo info = Callbacks[funcId];
            List<VariableValue> scriptVariables = new List<VariableValue>();
            for(int i = 0;i < info.argc;i++)
            {
                scriptVariables.Add(FFIManager.Native2ScriptVariable(args[i], info.nativeArgDef[i]));
            }
            var result = info.targetFunc.Invoke(scriptVariables);
            nint ret = 0;
            FFIManager.WriteValueToMemory(&ret, info.nativeRetDef, result);
            return ret;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        public unsafe static nint CallbackEntryStdcall(int funcId, nint* args)
        {
            return CallbackEntry(funcId,args);
        }
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public unsafe static nint CallbackEntryCdecl(int funcId, nint* args)
        {
            return CallbackEntry(funcId, args);
        }

        [UnmanagedCallersOnly]
        public unsafe static nint CallbackEntryDefault(int funcId, nint* args)
        {
            return CallbackEntry(funcId, args);
        }

        //ABI参数为空字符串时就是让系统自动选择
        //nativeArgDefines需要传入描述，比如"string,int,byte,ptr"
        public unsafe static CallbackInfo RegisterCallback(ScriptFunction func,string abi,string nativeArgDefines,string retNativeType)
        {
            if(func.FuncType != FunctionType.Local)
            {
                throw new ScriptException("不允许将非脚本函数注册为回调 FuncName=" + func.Name);
            }
            var id = ++LastestFuncId; //自增id
            
            if(!ABIMapper.ContainsKey(abi))
            {
                throw new ScriptException("ABI描述语句错误 输入：" + abi);
            }
            var immabi = ABIMapper[abi];
            nint pTarget = 0;

            if (immabi == TrampolineCode.ABI.Stdcall) pTarget = (nint)(delegate* unmanaged[Stdcall]<int, nint*, nint>)&CallbackEntryStdcall;
            else if(immabi == TrampolineCode.ABI.Cdecl) pTarget = (nint)(delegate* unmanaged[Cdecl]<int, nint*, nint>)&CallbackEntryCdecl;
            else pTarget = (nint)(delegate* unmanaged<int, nint*, nint>)&CallbackEntryDefault;

            var heap = (nint)NativeMemory.Alloc(ArgumentHeapSize);

            var code = TrampolineCode.GenerateTrampoline(id, func.FunctionArgumentNames.Count, heap, pTarget, immabi);

            byte* pStubMemory = (byte*)NativeMemoryManager.Alloc((nuint)code.Length, (nuint)Environment.SystemPageSize);
            for(int i = 0;i < code.Length;i++)
                pStubMemory[i] = code[i];


            var info = new CallbackInfo((nint)pStubMemory, heap, func.FunctionArgumentNames.Count, func,nativeArgDefines.Split(','),retNativeType,id);
            
            Callbacks.Add(id, info);
            Ptr2Callback.Add(info.addr, info);

            return info;
        }

        //删除回调桩，释放内存
        public unsafe static void UnregisterCallback(nint ptr)
        {
            var info = Ptr2Callback[ptr];
            NativeMemory.Free((void*)info.heap);
            NativeMemoryManager.Free((void*)info.addr);
            Ptr2Callback.Remove(ptr);
            Callbacks.Remove(info.id); 
        }
    }
}
