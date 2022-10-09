using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Profiles
{
    public class Profile
    {
        public GameSession session { get; }
        public ProfileData data { get; }
        public ProfileDynamicData dynamicData { get; }
        public ProfileBuildingsData buildingsData { get; }
        public DateTime lastSaveProfileTime { get; private set; }

        public Profile(GameSession _session, ProfileData _data, ProfileDynamicData _dynamicData, ProfileBuildingsData _buildingsData)
        {
            _data.SetupSession(_session);
            _dynamicData.SetupSession(_session);
            _buildingsData.SetupSession(_session);

            session = _session;
            data = _data;
            dynamicData = _dynamicData;
            buildingsData = _buildingsData;
        }

        /// <summary>
        /// Первичная настройка профиля после выбора языка
        /// </summary>
        public async Task SetupProfileOnFirstLaunch(User actualUser, LanguageCode language)
        {
            data.language = language.ToString();
            data.nickname = actualUser.FirstName.IsCorrectNickname() ? actualUser.FirstName : "Player_" + (1_000 + new Random().Next(9_000));
            await SaveProfile();
        }

        public async Task SaveProfileIfNeed(DateTime lastActivityTime)
        {
            if (lastSaveProfileTime < lastActivityTime)
            {
                await SaveProfile();
            }
        }

        private async Task SaveProfile()
        {
            await data.UpdateInDatabase();
            await dynamicData.UpdateInDatabase();
            await buildingsData.UpdateInDatabase();
            lastSaveProfileTime = DateTime.UtcNow;
        }

    }
}
