﻿using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Profiles;

namespace TextGameRPG.Scripts.Bot.Dialogs.Cheats
{
    [JsonObject]
    internal class ProfileState
    {
        public const string idPlacement = "[DBID_FOR_RESTORE]";

        public string nickname = string.Empty;
        public long telegramId;
        public string lastDate = string.Empty;
        public string lastVersion = string.Empty;
        public List<string> sqlQuerries = new List<string>();

        public static ProfileState Create(Profile profile)
        {
            return new ProfileState
            {
                nickname = profile.data.nickname,
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
            var fields = data.fieldsInfo;
            for (int i = 2; i < fields.Length; i++) //avoid dbid and telegram_id
            {
                var field = fields[i];
                var fieldValue = field.GetValue(data);
                var jsonStr = JsonConvert.SerializeObject(fieldValue);
                sb.Append($"{field.Name} = '{jsonStr}'");
                sb.Append(i < fields.Length - 1 ? ", " : " ");
            }
            sb.Append($"WHERE dbid='{idPlacement}' LIMIT 1");
            return sb.ToString();
        }

        private static string CreateQueryForRestore(ProfileDynamicData data)
        {
            var sb = new StringBuilder();
            sb.Append($"UPDATE ProfilesDynamic SET ");
            var fields = data.fieldsInfo;
            for (int i = 1; i < fields.Length; i++) //avoid dbid
            {
                var field = fields[i];
                var fieldValue = field.GetValue(data);
                var jsonStr = JsonConvert.SerializeObject(fieldValue);
                sb.Append($"{field.Name} = '{jsonStr}'");
                sb.Append(i < fields.Length - 1 ? ", " : " ");
            }
            sb.Append($"WHERE dbid='{idPlacement}' LIMIT 1");
            return sb.ToString();
        }

        private static string CreateQueryForRestore(ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.Append($"UPDATE Buildings SET ");
            var fields = data.fieldsInfo;
            for (int i = 1; i < fields.Length; i++) //avoid dbid
            {
                var field = fields[i];
                var fieldValue = field.GetValue(data);
                var jsonStr = JsonConvert.SerializeObject(fieldValue);
                sb.Append($"{field.Name} = '{jsonStr}'");
                sb.Append(i < fields.Length - 1 ? ", " : " ");
            }
            sb.Append($"WHERE dbid='{idPlacement}' LIMIT 1");
            return sb.ToString();
        }

    }
}
