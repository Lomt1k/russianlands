using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TextGameRPG.ViewModels.UserControls;

namespace TextGameRPG.Models.Editor.ItemsEditor
{
    public class ObjectPropertyModel
    {
        private string _unparsedValue;
        private string _selectedEnumValue;

        public object editableObject { get; }
        public PropertyInfo propertyInfo { get; }
        public string name => propertyInfo.Name;
        public Type type => propertyInfo.PropertyType;
        public string typeView { get; }
        public object? value { get; private set; }
        public string unparsedValue
        {
            get => _unparsedValue;
            set 
            {
                _unparsedValue = value;
                TryParse();
                SaveValueIfValid();
            }
        }
        public bool isValidValue { get; private set; }

        //enum field
        public bool isEnumValue => type.IsEnum;
        public ObservableCollection<string> enumNames { get; }
        public string selectedEnumValue
        {
            get => _selectedEnumValue;
            set
            {
                _selectedEnumValue = value;
                var enumValues = Enum.GetValues(this.value.GetType());
                foreach (var element in enumValues)
                {
                    if (element.ToString().Equals(_selectedEnumValue))
                    {
                        this.value = element;
                        break;
                    }
                }
            }
        }

        //boolean field
        public bool isBooleanValue => Type.GetTypeCode(type) == TypeCode.Boolean;

        public bool isDefaultType => !isBooleanValue && !isEnumValue;


        public ObjectPropertyModel(object _editableObject, PropertyInfo _info, object? _value)
        {
            editableObject = _editableObject;
            propertyInfo = _info;
            typeView = type.ToString().TrimStart("System.".ToCharArray());
            value = _value;
            _unparsedValue = _value.ToString();
            isValidValue = true;

            if (type.IsEnum)
            {
                _selectedEnumValue = _value.ToString();
                enumNames = new ObservableCollection<string>();
                var names = Enum.GetNames(_value.GetType());
                foreach (var name in names)
                {
                    enumNames.Add(name);
                }
            }
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
                case TypeCode.String:
                    value = _unparsedValue;
                    success = true;
                    break;
            }

            isValidValue = success;
        }

        private void SaveValueIfValid()
        {
            if (isValidValue)
            {
                propertyInfo.SetValue(editableObject, value);
            }
        }

        public static void FillObservableCollection<T>(ref ObservableCollection<ObjectPropertyModel> collection, T obj, bool includeClasses = false)
        {
            if (obj == null)
                return;

            var properties = obj.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.CustomAttributes.Any(x => x.AttributeType == typeof(IgnoreInEditorAttribute)))
                {
                    continue;
                }

                var value = propertyInfo.GetValue(obj);
                if (propertyInfo.PropertyType.IsClass && !propertyInfo.PropertyType.Name.Equals("String") )
                {
                    if (includeClasses)
                    {
                        FillObservableCollection(ref collection, value);
                    }
                    continue;
                }

                var objectPropertyModel = new ObjectPropertyModel(obj, propertyInfo, value);
                collection.Add(objectPropertyModel);
            }
        }


    }
}
