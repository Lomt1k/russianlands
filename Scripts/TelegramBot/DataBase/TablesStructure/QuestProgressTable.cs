using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure
{
    public class QuestProgressTable : DataTableBase
    {
        public override string tableName => "QuestProgress";

        public override TableColumn[] columns => QuestProgressData.GetTableColumns();

        public QuestProgressTable(BotDataBase _database) : base(_database)
        {
        }

        public async Task<QuestProgressData?> GetOrCreateQuestsData(long dbid)
        {
            try
            {
                var sqlQuery = $"SELECT * FROM {tableName} WHERE dbid='{dbid}' LIMIT 1";
                var command = await database.ExecuteQueryAsync(sqlQuery);
                var reader = await command.ExecuteReaderAsync();

                bool hasProfile = reader.HasRows;
                if (hasProfile)
                {
                    var table = new DataTable();
                    table.Load(reader);
                    var profileRow = table.Rows[0];
                    return new QuestProgressData(profileRow);
                }
                else
                {
                    var insertQuery = $"INSERT INTO {tableName} " +
                        $"(dbid) " +
                        $"VALUES " +
                        $"('{dbid}')";
                    var insertCommand = await database.ExecuteQueryAsync(insertQuery);
                    if (insertCommand != null)
                    {
                        var createdQuestsProgress = await GetOrCreateQuestsData(dbid);
                        return createdQuestsProgress;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Program.logger.Error(ex);
                return null;
            }
        }

        public async Task<bool> UpdateDataInDatabase(QuestProgressData questProgress)
        {
            var sb = new StringBuilder();
            sb.Append($"UPDATE {tableName} SET ");
            var fields = questProgress.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                sb.Append($"{field.Name} = '{field.GetValue(questProgress)}'");
                sb.Append(i < fields.Length - 1 ? ", " : " ");
            }
            sb.Append($"WHERE dbid='{questProgress.dbid}' LIMIT 1");
            string query = sb.ToString();
            var command = await database.ExecuteQueryAsync(query);
            return command != null;
        }

    }
}
