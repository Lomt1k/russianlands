using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Profiles
{
    public class Profile
    {
        public ProfileData data { get; }
        public ProfileDynamicData dynamicData { get; }
        public ProfileBuildingsData buildingsData { get; }
        public DateTime lastSaveProfileTime { get; private set; }

        public Profile(ProfileData _data, ProfileDynamicData _dynamicData, ProfileBuildingsData _buildingsData)
        {
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
            data.nickname = actualUser.FirstName + (actualUser.LastName != null ? ' ' + actualUser.LastName : string.Empty);
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
