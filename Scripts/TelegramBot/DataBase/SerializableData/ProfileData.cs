using System.Data;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData
{
    public class ProfileData : DatabaseSerializableData
    {
        public long dbid;
        public long telegram_id;
        public string username;
        public string nickname;
        public short tutorialStage;

        public static TableColumn[] GetTableColumns()
        {
            return new TableColumn[]
            {
                new TableColumn("dbid", "INTEGER PRIMARY KEY AUTOINCREMENT", "0"),
                new TableColumn("telegram_id", "INTEGER", "na"),
                new TableColumn("username", "TEXT", "na"),
                new TableColumn("nickname", "TEXT", "na"),
                new TableColumn("tutorialStage", "INTEGER", "0"),
            };
        }

        public bool isTutorialCompleted => tutorialStage == -1;

        public ProfileData(DataRow data) : base(data) 
        {
        }

        public async override Task<bool> UpdateInDatabase()
        {
            if (!isDeserializationCompleted)
                return true;

            var profilesTable = TelegramBot.instance.dataBase[Table.Profiles] as ProfilesDataTable;
            var success = await profilesTable.UpdateDataInDatabase(this);
            return success;
        }




    }
}
