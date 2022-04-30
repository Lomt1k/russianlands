namespace TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure
{
    public class ProfilesTable : TableStructureBase
    {
        public override string tableName => "Profiles";

        public override TableColumn[] columns => new TableColumn[] 
        {
            new TableColumn("dbid", "INTEGER PRIMARY KEY AUTOINCREMENT", "0"),
            new TableColumn("telegram_id", "INTEGER", "na"),
            new TableColumn("username", "TEXT", "na")
        };
    }
}
