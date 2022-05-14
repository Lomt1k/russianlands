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
        public string language;
        public string nickname;
        public short tutorialStage;
        public byte level;
        public short attributeStrength;
        public short attributeVitality;
        public short attributeSorcery;
        public short attributeLuck;
        public int resourceGold;
        public int resourceFood;
        public int resourceDiamonds;

        public static TableColumn[] GetTableColumns()
        {
            return new TableColumn[]
            {
                new TableColumn("dbid", "INTEGER PRIMARY KEY AUTOINCREMENT", "0"),
                new TableColumn("telegram_id", "INTEGER", "na"),
                new TableColumn("username", "TEXT", "na"),
                new TableColumn("language", "TEXT", "ru"),
                new TableColumn("nickname", "TEXT", "na"),
                new TableColumn("tutorialStage", "INTEGER", "0"),
                new TableColumn("level", "INTEGER", "1"),
                new TableColumn("attributeStrength", "INTEGER", "1"),
                new TableColumn("attributeVitality", "INTEGER", "1"),
                new TableColumn("attributeSorcery", "INTEGER", "1"),
                new TableColumn("attributeLuck", "INTEGER", "1"),
                new TableColumn("resourceGold", "INTEGER", "0"),
                new TableColumn("resourceFood", "INTEGER", "0"),
                new TableColumn("resourceDiamonds", "INTEGER", "0"),                
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
