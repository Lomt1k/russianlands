using System;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData
{
    public abstract class DatabaseSerializableData
    {
        public GameSession session { get; protected set; }
        protected bool isDeserializationCompleted { get; private set; } = false;

        public abstract FieldInfo[] fieldsInfo { get; }

        public DatabaseSerializableData(DataRow data)
        {
            Deserialize(data);
            isDeserializationCompleted = true;
        }

        protected virtual void Deserialize(DataRow data)
        {
            foreach (var field in fieldsInfo)
            {
                var dbValue = data[field.Name];
                if (dbValue != null)
                {
                    string unparsed = dbValue.ToString();
                    var parsedValue = Parse(field.FieldType, unparsed);
                    field.SetValue(this, parsedValue);
                }
                else
                {
                    Program.logger.Error($"{GetType()} deserialization error: not found field {field.Name} in database");
                }
            }
        }

        public abstract Task<bool> UpdateInDatabase();

        private object Parse(Type type, string unparsed)
        {
            object parsed = null;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    unparsed.TryParse<bool>(out var boolResult);
                    parsed = boolResult;
                    break;
                case TypeCode.Int32:
                    unparsed.TryParse<int>(out var intResult);
                    parsed = intResult;
                    break;
                case TypeCode.Int64:
                    unparsed.TryParse<long>(out var longResult);
                    parsed = longResult;
                    break;
                case TypeCode.SByte:
                    unparsed.TryParse<sbyte>(out var sbyteResult);
                    parsed = sbyteResult;
                    break;
                case TypeCode.UInt16:
                    unparsed.TryParse<ushort>(out var ushortResult);
                    parsed = ushortResult;
                    break;
                case TypeCode.UInt32:
                    unparsed.TryParse<uint>(out var uintResult);
                    parsed = uintResult;
                    break;
                case TypeCode.UInt64:
                    unparsed.TryParse<ulong>(out var ulongResult);
                    parsed = ulongResult;
                    break;
                case TypeCode.Int16:
                    unparsed.TryParse<short>(out var shortResult);
                    parsed = shortResult;
                    break;
                case TypeCode.Byte:
                    unparsed.TryParse<byte>(out var byteResult);
                    parsed = byteResult;
                    break;
                case TypeCode.Double:
                    unparsed.TryParse<double>(out var doubleResult);
                    parsed = doubleResult;
                    break;
                case TypeCode.DateTime:
                    unparsed.TryParse<DateTime>(out var dateTimeResult);
                    parsed = dateTimeResult;
                    break;
                case TypeCode.Char:
                    unparsed.TryParse<char>(out var charResult);
                    parsed = charResult;
                    break;
                case TypeCode.String:
                    parsed = unparsed;
                    break;
            }

            return parsed;
        }

        public void SetupSession(GameSession _session)
        {
            session = _session;
        }

    }
}
