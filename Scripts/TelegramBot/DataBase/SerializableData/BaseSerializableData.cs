using System.Data;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData
{
    public abstract class BaseSerializableData
    {
        public BaseSerializableData(DataRow data)
        {
            Deserialize(data);
        }

        private void Deserialize(DataRow data)
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

    }
}
