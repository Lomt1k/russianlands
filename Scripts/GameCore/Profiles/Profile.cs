using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Profiles
{
    public class Profile
    {
        public GameSession session { get; }
        public ProfileData data { get; private set; }
        public ProfileDynamicData dynamicData { get; private set; }
        public ProfileBuildingsData buildingsData { get; private set; }
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
                await SaveProfile().FastAwait();
            }
        }

        private async Task SaveProfile()
        {
            var db = BotController.dataBase.db;
            await db.UpdateAsync(data).FastAwait();

            var rawDynamicData = new RawProfileDynamicData();
            rawDynamicData.Fill(dynamicData);
            await db.UpdateAsync(rawDynamicData).FastAwait();

            await db.UpdateAsync(buildingsData).FastAwait();
            lastSaveProfileTime = DateTime.UtcNow;
        }

        public async Task Cheat_ResetProfile()
        {
            var previousDbid = data.dbid;
            data = new ProfileData() { dbid = previousDbid }.SetupNewProfile(session.actualUser);
            dynamicData = new ProfileDynamicData(previousDbid);
            buildingsData = new ProfileBuildingsData() { dbid = previousDbid };
            await SaveProfile().FastAwait();
        }

    }
}
