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
        public string username;
        public string language;
        public string nickname;
        public string regDate;
        public string regVersion;
        public string lastDate;
        public string lastVersion;

        public int adminStatus;
        public byte level;
        public byte freeNickChanges;
        public long endPremiumTime;


        // resources
        public int resourceGold;
        public int resourceFood;
        public int resourceDiamond;
        public int resourceHerbs;
        public int resourceWood;

        // craft resources
        public int resourceCraftPiecesCommon;
        public int resourceCraftPiecesRare;
        public int resourceCraftPiecesEpic;
        public int resourceCraftPiecesLegendary;

        // skill resources
        public int resourceFruitApple;
        public int resourceFruitPear;
        public int resourceFruitMandarin;
        public int resourceFruitCoconut;
        public int resourceFruitPineapple;
        public int resourceFruitBanana;
        public int resourceFruitWatermelon;
        public int resourceFruitStrawberry;
        public int resourceFruitBlueberry;
        public int resourceFruitKiwi;
        public int resourceFruitCherry;
        public int resourceFruitGrape;

        // skills
        public byte skillSword;
        public byte skillBow;
        public byte skillStick;
        public byte skillScroll;
        public byte skillArmor;
        public byte skillHelmet;
        public byte skillBoots;
        public byte skillShield;

        public static TableColumn[] GetTableColumns()
        {
            return new TableColumn[]
            {
                new TableColumn("dbid", "INTEGER PRIMARY KEY AUTOINCREMENT", "0"),
                new TableColumn("telegram_id", "INTEGER", "0"),
                new TableColumn("username", "TEXT", "na"),
                new TableColumn("language", "TEXT", "RU"),
                new TableColumn("nickname", "TEXT", "na"),
                new TableColumn("regDate", "TEXT", "na"),
                new TableColumn("regVersion", "TEXT", "na"),
                new TableColumn("lastDate", "TEXT", "na"),
                new TableColumn("lastVersion", "TEXT", "na"),

                new TableColumn("adminStatus", "INTEGER", "0"),
                new TableColumn("level", "INTEGER", "1"),
                new TableColumn("freeNickChanges", "INTEGER", "1"),
                new TableColumn("endPremiumTime", "INTEGER", "0"),

                new TableColumn("resourceGold", "INTEGER", "3500"),
                new TableColumn("resourceFood", "INTEGER", "1000"),
                new TableColumn("resourceDiamond", "INTEGER", "100"),
                new TableColumn("resourceHerbs", "INTEGER", "0"),
                new TableColumn("resourceWood", "INTEGER", "0"),

                new TableColumn("resourceCraftPiecesCommon", "INTEGER", "0"),
                new TableColumn("resourceCraftPiecesRare", "INTEGER", "0"),
                new TableColumn("resourceCraftPiecesEpic", "INTEGER", "0"),
                new TableColumn("resourceCraftPiecesLegendary", "INTEGER", "0"),

                new TableColumn("resourceFruitApple", "INTEGER", "0"),
                new TableColumn("resourceFruitPear", "INTEGER", "0"),
                new TableColumn("resourceFruitMandarin", "INTEGER", "0"),
                new TableColumn("resourceFruitCoconut", "INTEGER", "0"),
                new TableColumn("resourceFruitPineapple", "INTEGER", "0"),
                new TableColumn("resourceFruitBanana", "INTEGER", "0"),
                new TableColumn("resourceFruitWatermelon", "INTEGER", "0"),
                new TableColumn("resourceFruitStrawberry", "INTEGER", "0"),
                new TableColumn("resourceFruitBlueberry", "INTEGER", "0"),
                new TableColumn("resourceFruitKiwi", "INTEGER", "0"),
                new TableColumn("resourceFruitCherry", "INTEGER", "0"),
                new TableColumn("resourceFruitGrape", "INTEGER", "0"),

                new TableColumn("skillSword", "INTEGER", "0"),
                new TableColumn("skillBow", "INTEGER", "0"),
                new TableColumn("skillStick", "INTEGER", "0"),
                new TableColumn("skillScroll", "INTEGER", "0"),
                new TableColumn("skillArmor", "INTEGER", "0"),
                new TableColumn("skillHelmet", "INTEGER", "0"),
                new TableColumn("skillBoots", "INTEGER", "0"),
                new TableColumn("skillShield", "INTEGER", "0"),
            };
        }

        public ProfileData(DataRow data) : base(data) 
        {
        }

        public async override Task<bool> UpdateInDatabase()
        {
            if (!isDeserializationCompleted)
                return true;

            var profilesTable = BotController.dataBase[Table.Profiles] as ProfilesDataTable;
            var success = await profilesTable.UpdateDataInDatabase(this).FastAwait();
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
