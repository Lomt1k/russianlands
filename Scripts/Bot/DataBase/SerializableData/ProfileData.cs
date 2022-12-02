using System;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.DataBase.TablesStructure;

namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData
{
    public class ProfileData : DatabaseSerializableData
    {
        static FieldInfo[] staticFieldsInfo = typeof(ProfileData).GetFields();
        public override FieldInfo[] fieldsInfo => staticFieldsInfo;

        public long dbid;
        public long telegram_id;
        public long endPremiumTime;
        public string username;
        public string language;
        public string nickname;
        public int adminStatus;
        public byte level;

        // resources
        public int resourceGold;
        public int resourceFood;
        public int resourceDiamond;
        public int resourceHerbs;
        public int resourceWood;

        public static TableColumn[] GetTableColumns()
        {
            return new TableColumn[]
            {
                new TableColumn("dbid", "INTEGER PRIMARY KEY AUTOINCREMENT", "0"),
                new TableColumn("telegram_id", "INTEGER", "0"),
                new TableColumn("endPremiumTime", "INTEGER", "0"),
                new TableColumn("username", "TEXT", "na"),
                new TableColumn("language", "TEXT", "ru"),
                new TableColumn("nickname", "TEXT", "na"),
                new TableColumn("adminStatus", "INTEGER", "0"),
                new TableColumn("level", "INTEGER", "1"),
                new TableColumn("resourceGold", "INTEGER", "3500"),
                new TableColumn("resourceFood", "INTEGER", "1000"),
                new TableColumn("resourceDiamond", "INTEGER", "150"),
                new TableColumn("resourceHerbs", "INTEGER", "0"),
                new TableColumn("resourceWood", "INTEGER", "0"),
            };
        }

        public ProfileData(DataRow data) : base(data) 
        {
        }

        public async override Task<bool> UpdateInDatabase()
        {
            if (!isDeserializationCompleted)
                return true;

            var profilesTable = TelegramBot.instance.dataBase[Table.Profiles] as ProfilesDataTable;
            var success = await profilesTable.UpdateDataInDatabase(this).ConfigureAwait(false);
            return success;
        }
        public bool IsPremiumActive()
        {
            return endPremiumTime > DateTime.UtcNow.Ticks;
        }

        public bool IsPremiumExpired()
        {
            return !IsPremiumActive() && endPremiumTime > 0;
        }


    }
}
