using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.Bot.DataBase.TablesStructure
{
    public class ProfilesDynamicDataTable : DataTableBase
    {
        public override string tableName => "ProfilesDynamic";

        public override TableColumn[] columns => ProfileDynamicData.GetTableColumns();

        public ProfilesDynamicDataTable(BotDataBase _database) : base(_database)
        {
        }

        public async Task<ProfileDynamicData?> GetOrCreateData(long dbid)
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
                    return new ProfileDynamicData(profileRow);
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

        public async Task<bool> UpdateDataInDatabase(ProfileDynamicData data)
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

        // for cheat
        public async Task<bool> ResetToDefaultValues(long dbid)
        {
            var sb = new StringBuilder();
            sb.Append($"UPDATE {tableName} SET ");
            for (int i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                if (column.name.Equals("dbid"))
                    continue;

                var defaultValue = column.defaultValue.Replace("'", string.Empty);
                sb.Append($"{column.name} = '{defaultValue}'");
                sb.Append(i < columns.Length - 1 ? ", " : " ");
            }
            sb.Append($"WHERE dbid='{dbid}' LIMIT 1");
            string query = sb.ToString();
            var command = await database.ExecuteQueryAsync(query);
            return command != null;
        }

    }
}
