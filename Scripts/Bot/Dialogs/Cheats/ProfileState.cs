using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite;
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
        public string profileQuery = string.Empty;
        public string profileDynamicQuery = string.Empty;
        public string buildingsQuery = string.Empty;

        public static ProfileState Create(Profile profile)
        {
            return new ProfileState
            {
                environment = BotController.dataPath,
                nickname = profile.data.nickname,
                databaseId = profile.data.dbid,
                telegramId = profile.data.telegram_id,
                lastDate = profile.data.lastActivityTime,
                lastVersion = profile.data.lastVersion,
                profileQuery = CreateQueryForRestore(profile.data),
                profileDynamicQuery = CreateQueryForRestore(profile.dynamicData),
                buildingsQuery = CreateQueryForRestore(profile.buildingsData)
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
                var kvp = string.Format("{0} = '{1}'", property.Name, value);
                sb.Append(kvp);
                sb.Append(i < propertiesToSave.Length - 1 ? ", " : " ");
            }
            sb.Append($"WHERE dbid='{idPlacement}'");
            return sb.ToString();
        }

        private static string CreateQueryForRestore(ProfileDynamicData data)
        {
            var rawDynamicData = new RawProfileDynamicData();
            rawDynamicData.Fill(data);

            var sb = new StringBuilder();
            sb.Append($"UPDATE ProfilesDynamic SET ");
            var allProperties = rawDynamicData.GetType().GetProperties();
            var propertiesToSave = allProperties.Where(x => !x.GetCustomAttributesData().Any(atr => atr.AttributeType == typeof(IgnoreAttribute))).ToArray();

            for (int i = 1; i < propertiesToSave.Length; i++) //avoid dbid
            {
                var property = propertiesToSave[i];
                var value = GetPropertyValue(property, rawDynamicData);
                var kvp = string.Format("{0} = '{1}'", property.Name, value);
                sb.Append(kvp);
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
            var propertiesToSave = allProperties.Where(x => !x.GetCustomAttributesData().Any(atr => atr.AttributeType == typeof(IgnoreAttribute))).ToArray();

            for (int i = 1; i < propertiesToSave.Length; i++) //avoid dbid
            {
                var property = propertiesToSave[i];
                var value = GetPropertyValue(property, data);
                var kvp = string.Format("{0} = '{1}'", property.Name, value);
                sb.Append(kvp);
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
            if (property.PropertyType.IsEnum)
            {
                bool storeAsText = property.PropertyType.GetTypeInfo().CustomAttributes.Any(x => x.AttributeType == typeof(StoreAsTextAttribute));
                if (storeAsText)
                {
                    return property.GetValue(obj).ToString();
                }
                var underlyingValue = Convert.ChangeType(property.GetValue(obj), Enum.GetUnderlyingType(property.PropertyType));
                return underlyingValue;
            }

            return property.GetValue(obj);
        }

        public async Task ExecuteQuerries(long dbid)
        {
            await ExecuteQuery(dbid, profileQuery).FastAwait();
            await ExecuteQuery(dbid, profileDynamicQuery).FastAwait();
            await ExecuteQuery(dbid, buildingsQuery).FastAwait();
        }

        private async Task ExecuteQuery(long dbid, string query)
        {
            var preparedQuery = query.Replace(idPlacement, dbid.ToString());
            await BotController.dataBase.db.ExecuteAsync(preparedQuery).FastAwait();
        }

    }
}
