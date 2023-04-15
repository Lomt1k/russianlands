using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.Bot.DataBase.TablesStructure
{
    public class ProfilesDataTable : DataTableBase
    {
        public override string tableName => "Profiles";

        public override TableColumn[] columns => ProfileData.GetTableColumns();

        public ProfilesDataTable(BotDataBase _database) : base(_database)
        {
        }

        public async Task<ProfileData?> GetOrCreateProfileData(User user, ChatId? fakeChatId)
        {
            try
            {
                var chatId = fakeChatId ?? user.Id;
                var sqlQuery = $"SELECT * FROM {tableName} WHERE telegram_id='{chatId}' LIMIT 1";
                var command = await database.ExecuteQueryAsync(sqlQuery);
                var reader = await command.ExecuteReaderAsync();

                bool hasProfile = reader.HasRows;
                if (hasProfile)
                {
                    var table = new DataTable();
                    table.Load(reader);
                    var profileRow = table.Rows[0];
                    return new ProfileData(profileRow);
                }
                else
                {
                    // First profile setup
                    var language = BotController.config.defaultLanguageCode.ToString();
                    var nickname = user.FirstName.IsCorrectNickname() ? user.FirstName : "Player_" + (1_000 + new Random().Next(9_000));
                    var regDate = DateTime.UtcNow.AsString();
                    var regVersion = ProjectVersion.Current.ToString();

                    var insertQuery = $"INSERT INTO {tableName} " +
                        $"(telegram_id, username, language, nickname, regDate, regVersion, lastDate, lastVersion) " +
                        $"VALUES " +
                        $"('{chatId}', '{user.Username}', '{language}', '{nickname}', '{regDate}', '{regVersion}', '{regDate}', '{regVersion}')";
                    var insertCommand = await database.ExecuteQueryAsync(insertQuery);
                    if (insertCommand != null)
                    {
                        var createdProfile = await GetOrCreateProfileData(user, fakeChatId);
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

        public async Task<bool> UpdateDataInDatabase(ProfileData profile)
        {
            var sb = new StringBuilder();
            sb.Append($"UPDATE {tableName} SET ");
            var fields = profile.fieldsInfo;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                sb.Append($"{field.Name} = '{field.GetValue(profile)}'");
                sb.Append(i < fields.Length - 1 ? ", " : " ");
            }
            sb.Append($"WHERE dbid='{profile.dbid}' LIMIT 1");
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
                if (column.name.Equals("dbid") || column.name.Equals("telegram_id"))
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
