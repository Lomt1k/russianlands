using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure
{
    public class ProfileBuildingsDataTable : DataTableBase
    {
        public override string tableName => "Buildings";

        public override TableColumn[] columns => ProfileBuildingsData.GetTableColumns();

        public ProfileBuildingsDataTable(BotDataBase _database) : base(_database)
        {
        }

        public async Task<ProfileBuildingsData?> GetOrCreateData(long dbid)
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
                    return new ProfileBuildingsData(profileRow);
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
                        var createdProfile = await GetOrCreateData(dbid);
                        return createdProfile;
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

        public async Task<bool> UpdateDataInDatabase(ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.Append($"UPDATE {tableName} SET ");
            var fields = data.fieldsInfo;
            for (int i = 1; i < fields.Length; i++) //avoid dbid
            {
                var field = fields[i];
                var fieldValue = field.GetValue(data);
                var jsonStr = JsonConvert.SerializeObject(fieldValue);
                sb.Append($"{field.Name} = '{jsonStr}'");
                sb.Append(i < fields.Length - 1 ? ", " : " ");
            }
            sb.Append($"WHERE dbid='{data.dbid}' LIMIT 1");
            string query = sb.ToString();
            var command = await database.ExecuteQueryAsync(query);
            return command != null;
        }

    }
}
