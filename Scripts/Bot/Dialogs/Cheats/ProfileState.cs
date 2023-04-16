using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Profiles;

namespace TextGameRPG.Scripts.Bot.Dialogs.Cheats
{
    [JsonObject]
    internal class ProfileState
    {
        public const string idPlacement = "[DBID_FOR_RESTORE]";

        public string environment = string.Empty;
        public string nickname = string.Empty;
        public long databaseId;
        public long telegramId;
        public string lastDate = string.Empty;
        public string lastVersion = string.Empty;
        public List<string> sqlQuerries = new List<string>();

        public static ProfileState Create(Profile profile)
        {
            return new ProfileState
            {
                environment = BotController.dataPath,
                nickname = profile.data.nickname,
                databaseId = profile.data.dbid,
                telegramId = profile.data.telegram_id,
                lastDate = profile.data.lastDate,
                lastVersion = profile.data.lastVersion,
                sqlQuerries = new List<string>
                {
                    CreateQueryForRestore(profile.data),
                    CreateQueryForRestore(profile.dynamicData),
                    CreateQueryForRestore(profile.buildingsData)
                }
            };
        }

        private static string CreateQueryForRestore(ProfileData data)
        {
            var sb = new StringBuilder();
            sb.Append($"UPDATE Profiles SET ");
            var allProperties = data.GetType().GetProperties();
            var propertiesToSave = allProperties.Where(x => !x.GetCustomAttributesData().Any(atr => atr.AttributeType.Name.Equals("IgnoreAttribute"))).ToArray();

            for (int i = 2; i < propertiesToSave.Length; i++) //avoid dbid and telegram_id
            {
                var property = propertiesToSave[i];
                var value = GetPropertyValue(property, data);
                sb.Append($"{property.Name} = '{value}'");
                sb.Append(i < propertiesToSave.Length - 1 ? ", " : " ");
            }
            sb.Append($"WHERE dbid='{idPlacement}'");
            return sb.ToString();
        }

        private static string CreateQueryForRestore(ProfileDynamicData data)
        {
            var sb = new StringBuilder();
            sb.Append($"UPDATE ProfilesDynamic SET ");
            var allProperties = data.GetType().GetProperties();
            var propertiesToSave = allProperties.Where(x => !x.GetCustomAttributesData().Any(atr => atr.AttributeType.Name.Equals("IgnoreAttribute"))).ToArray();

            for (int i = 1; i < propertiesToSave.Length; i++) //avoid dbid
            {
                var property = propertiesToSave[i];
                var value = GetPropertyValue(property, data);
                Program.logger.Info($"{property.Name}");
                var jsonStr = JsonConvert.SerializeObject(value);
                sb.Append($"{property.Name} = '{jsonStr}'");
                sb.Append(i < propertiesToSave.Length - 1 ? ", " : " ");
            }
            sb.Append($"WHERE dbid='{idPlacement}'");
            return sb.ToString();
        }

        private static string CreateQueryForRestore(ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.Append($"UPDATE Buildings SET ");
            var allProperties = data.GetType().GetProperties();
            var propertiesToSave = allProperties.Where(x => !x.GetCustomAttributesData().Any(atr => atr.AttributeType.Name.Equals("IgnoreAttribute"))).ToArray();

            for (int i = 1; i < propertiesToSave.Length; i++) //avoid dbid
            {
                var property = propertiesToSave[i];
                var value = GetPropertyValue(property, data);
                sb.Append($"{property.Name} = '{value}'");
                sb.Append(i < propertiesToSave.Length - 1 ? ", " : " ");
            }
            sb.Append($"WHERE dbid='{idPlacement}'");
            return sb.ToString();
        }

        private static object? GetPropertyValue(PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(DateTime))
            {
                var dateTime = (DateTime)property.GetValue(obj);
                return dateTime.Ticks;
            }
            return property.GetValue(obj);
        }

        public async Task ExecuteQuery(long dbid)
        {
            foreach (var query in sqlQuerries)
            {
                var preparedQuery = query.Replace(idPlacement, dbid.ToString());
                await BotController.dataBase.db.ExecuteAsync(preparedQuery);
            }
        }

    }
}
