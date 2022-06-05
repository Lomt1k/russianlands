using System;
using System.Reflection;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Models.Editor.ItemsEditor
{
    internal class FieldModel
    {
        private string _unparsedValue;

        public string name { get; }
        public Type type { get;  }
        public string typeView { get; }
        public object? value { get; private set; }
        public string unparsedValue
        {
            get => _unparsedValue;
            set 
            {
                _unparsedValue = value;
                TryParse();
            }
        }
        public bool isValidValue { get; private set; }

        public FieldModel(FieldInfo _info, object? _value)
        {
            name = _info.Name;
            type = _info.FieldType;
            typeView = type.ToString().TrimStart("System.".ToCharArray());
            value = _value;
            _unparsedValue = _value.ToString();
            isValidValue = true;
        }

        private void TryParse()
        {
            bool success = false;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    success = _unparsedValue.TryParse<bool>(out var boolResult);
                    if (success) value = boolResult;
                    break;
                case TypeCode.Int32:
                    success = _unparsedValue.TryParse<int>(out var intResult);
                    if (success) value = intResult;
                    break;
                case TypeCode.Int64:
                    success = _unparsedValue.TryParse<long>(out var longResult);
                    if (success) value = longResult;
                    break;
                case TypeCode.SByte:
                    success = _unparsedValue.TryParse<sbyte>(out var sbyteResult);
                    if (success) value = sbyteResult;
                    break;
                case TypeCode.UInt16:
                    success = _unparsedValue.TryParse<ushort>(out var ushortResult);
                    if (success) value = ushortResult;
                    break;
                case TypeCode.UInt32:
                    success = _unparsedValue.TryParse<uint>(out var uintResult);
                    if (success) value = uintResult;
                    break;
                case TypeCode.UInt64:
                    success = _unparsedValue.TryParse<ulong>(out var ulongResult);
                    if (success) value = ulongResult;
                    break;
                case TypeCode.Int16:
                    success = _unparsedValue.TryParse<short>(out var shortResult);
                    if (success) value = shortResult;
                    break;
                case TypeCode.Byte:
                    success = _unparsedValue.TryParse<byte>(out var byteResult);
                    if (success) value = byteResult;
                    break;
                case TypeCode.Double:
                    success = _unparsedValue.TryParse<double>(out var doubleResult);
                    if (success) value = doubleResult;
                    break;
                case TypeCode.DateTime:
                    success = _unparsedValue.TryParse<DateTime>(out var dateTimeResult);
                    if (success) value = dateTimeResult;
                    break;
                case TypeCode.Char:
                    success = _unparsedValue.TryParse<char>(out var charResult);
                    if (success) value = charResult;
                    break;
                case TypeCode.Single:
                    success = _unparsedValue.TryParse<float>(out var floatResult);
                    if (success) value = floatResult;
                    break;
            }

            isValidValue = success;
        }

    }
}
