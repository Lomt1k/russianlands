﻿using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure
{
    public class ProfilesTable : DataTableBase
    {
        public override string tableName => "Profiles";

        public override TableColumn[] columns => Profile.GetTableColumns();

        public ProfilesTable(BotDataBase _database) : base(_database)
        {
        }

        public async Task<Profile?> GetOrCreateProfile(User user)
        {
            try
            {
                var sqlQuery = $"SELECT * FROM {tableName} WHERE telegram_id='{user.Id}' LIMIT 1";
                var command = await database.ExecuteQueryAsync(sqlQuery);
                var reader = await command.ExecuteReaderAsync();

                bool hasProfile = reader.HasRows;
                if (hasProfile)
                {
                    var table = new DataTable();
                    table.Load(reader);
                    var profileRow = table.Rows[0];
                    return new Profile(profileRow);
                }
                else
                {
                    var insertQuery = $"INSERT INTO {tableName} " +
                        $"(telegram_id, username) " +
                        $"VALUES " +
                        $"('{user.Id}', '{user.Username}')";
                    var insertCommand = await database.ExecuteQueryAsync(insertQuery);
                    if (insertCommand != null)
                    {
                        var createdProfile = await GetOrCreateProfile(user);
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

        public async Task<bool> UpdateInDatabase(Profile profile)
        {
            var sb = new StringBuilder();
            sb.Append($"UPDATE {tableName} SET ");
            var fields = profile.GetType().GetFields();
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

    }
}
