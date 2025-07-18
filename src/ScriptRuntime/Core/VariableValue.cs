using ScriptRuntime.Runtime;

namespace ScriptRuntime.Core
{
    //此类用于表示脚本中的变量
    public class VariableValue : IComparable<VariableValue>
    {
        public VariableValue Parent; //父对象
        public bool ReadOnly = false;
        public ValueType VarType;
        //public object Value;
        private object _value;
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!ReadOnly)
                {
                    _value = value;
                }
            }
        }
        public VariableValue(ValueType type, object value)
        {
            VarType = type;
            _value = value;
            Parent = FunctionManager.EmptyVariable;
        }
        //取大小用于排序，数字直接是面值，字符串是长度，布尔是0或1
        public int GetSize()
        {
            var curSzie = 0;
            switch (this.VarType)
            {
                case ValueType.NUM: curSzie = (int)(double)Value; break;
                case ValueType.STRING: curSzie = ((string)Value).Length; break;
                case ValueType.PTR: curSzie = (int)(nint)Value; break;
                case ValueType.BOOL: curSzie = ((bool)Value) ? 1 : 0; break;
                default:
                    curSzie = -1;
                    break;
            }
            return curSzie;
        }
        public int CompareTo(VariableValue other)
        {
            return GetSize() - other.GetSize();
        }
    }

    public enum ValueType
    {
        NULL,
        ANY,
        NUM,
        STRING,
        BOOL,
        ARRAY,
        OBJECT,
        FUNCTION,
        PTR,
    }
}