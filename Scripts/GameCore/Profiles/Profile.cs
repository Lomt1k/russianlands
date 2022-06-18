using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Profiles
{
    public class Profile
    {
        public ProfileData data { get; }
        public ProfileDynamicData dynamicData { get; }
        public QuestProgressData questProgressData { get; }
        public DateTime lastSaveProfileTime { get; private set; }

        public Profile(ProfileData _data, ProfileDynamicData _dynamicData, QuestProgressData _questProgressData)
        {
            data = _data;
            dynamicData = _dynamicData;
            questProgressData = _questProgressData;
        }

        public async Task SaveProfileIfNeed(DateTime lastActivityTime)
        {
            if (lastSaveProfileTime < lastActivityTime)
            {
                await data.UpdateInDatabase();
                await dynamicData.UpdateInDatabase();
                await questProgressData.UpdateInDatabase();
                lastSaveProfileTime = DateTime.UtcNow;
            }
        }

    }
}
