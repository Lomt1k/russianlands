using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

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

        public async Task SaveProfileIfNeed(DateTime lastActivityTime)
        {
            if (lastSaveProfileTime < lastActivityTime)
            {
                await SaveProfile().ConfigureAwait(false);
            }
        }

        private async Task SaveProfile()
        {
            await data.UpdateInDatabase().ConfigureAwait(false);
            await dynamicData.UpdateInDatabase().ConfigureAwait(false);
            await buildingsData.UpdateInDatabase().ConfigureAwait(false);
            lastSaveProfileTime = DateTime.UtcNow;
        }

    }
}
