using System.Data;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData
{
    public class QuestProgressData : DatabaseSerializableData
    {
        public long dbid;
        // --- название полей должно быть идентичным QuestType (из QuestsEnum.cs)
        public int MainQuest;

        public static TableColumn[] GetTableColumns()
        {
            return new TableColumn[]
            {
                new TableColumn("dbid", "INTEGER PRIMARY KEY AUTOINCREMENT", "0"),
                new TableColumn("MainQuest", "INTEGER", "100"), //основной квест запущен изначально
            };
        }

        public QuestProgressData(DataRow data) : base(data)
        {
        }

        public async override Task<bool> UpdateInDatabase()
        {
            if (!isDeserializationCompleted)
                return true;

            var questsTable = TelegramBot.instance.dataBase[Table.QuestProgress] as QuestProgressTable;
            var success = await questsTable.UpdateDataInDatabase(this);
            return success;
        }




    }
}
