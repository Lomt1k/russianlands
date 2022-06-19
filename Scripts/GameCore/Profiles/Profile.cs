using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Profiles
{
    public class Profile
    {
        public ProfileData data { get; }
        public ProfileDynamicData dynamicData { get; }
        public DateTime lastSaveProfileTime { get; private set; }

        public Profile(ProfileData _data, ProfileDynamicData _dynamicData)
        {
            data = _data;
            dynamicData = _dynamicData;
        }

        public async Task SaveProfileIfNeed(DateTime lastActivityTime)
        {
            if (lastSaveProfileTime < lastActivityTime)
            {
                await data.UpdateInDatabase();
                await dynamicData.UpdateInDatabase();
                lastSaveProfileTime = DateTime.UtcNow;
            }
        }

    }
}
