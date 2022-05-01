using System.Data;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData
{
    public abstract class DatabaseSerializableData
    {
        protected bool isDeserializationCompleted { get; private set; } = false;

        public DatabaseSerializableData(DataRow data)
        {
            Deserialize(data);
            isDeserializationCompleted = true;
        }

        protected virtual void Deserialize(DataRow data)
        {
            var fields = GetType().GetFields();
            foreach (var field in fields)
            {
                var dbValue = data[field.Name];
                if (dbValue != null)
                {
                    field.SetValue(this, dbValue);
                }
                else
                {
                    Program.logger.Error($"{GetType()} deserialization error: not found field {field.Name} in database");
                }
            }
        }

        public abstract Task<bool> UpdateInDatabase();

    }
}
