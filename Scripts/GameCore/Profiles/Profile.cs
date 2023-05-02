using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.Scripts.GameCore.Profiles;

public class Profile
{
    private static readonly DailyRemindersManager remindersManager = Services.Services.Get<DailyRemindersManager>();

    public GameSession session { get; }
    public ProfileData data { get; private set; }
    public ProfileDynamicData dynamicData { get; private set; }
    public ProfileBuildingsData buildingsData { get; private set; }
    public ProfileDailyData dailyData { get; private set; }
    public DateTime lastSaveProfileTime { get; private set; }

    public Profile(GameSession _session, ProfileData _data, ProfileDynamicData _dynamicData, ProfileBuildingsData _buildingsData, ProfileDailyData _dailyData)
    {
        _data.SetupSession(_session);
        _dynamicData.SetupSession(_session);
        _buildingsData.SetupSession(_session);
        _dailyData.SetupSession(_session);

        session = _session;
        data = _data;
        dynamicData = _dynamicData;
        buildingsData = _buildingsData;
        dailyData = _dailyData;
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

        var rawDailyData = new RawProfileDailyData();
        rawDailyData.Fill(dailyData);
        await db.InsertOrReplaceAsync(rawDailyData).FastAwait();

        lastSaveProfileTime = DateTime.UtcNow;
    }

    public async Task Cheat_ResetProfile()
    {
        var previousDbid = data.dbid;
        data = new ProfileData() { dbid = previousDbid }.SetupNewProfile(session.actualUser);
        dynamicData = new ProfileDynamicData(previousDbid);
        buildingsData = new ProfileBuildingsData() { dbid = previousDbid };
        ResetDailyDataWithoutSave();
        await SaveProfile().FastAwait();
    }

    public void ResetDailyDataWithoutSave()
    {
        dailyData = ProfileDailyData.Create(data, dynamicData, buildingsData);
    }

    public static async Task<Profile> Load(GameSession session)
    {
        var db = BotController.dataBase.db;
        var query = db.Table<ProfileData>().Where(x => x.telegram_id == session.actualUser.Id);
        var profileData = await query.FirstOrDefaultAsync().FastAwait();
        if (profileData == null)
        {
            profileData = new ProfileData().SetupNewProfile(session.actualUser);
            await db.InsertAsync(profileData).FastAwait();
        }
        else
        {
            // В первой сессии remindersManager срабатывает только после выбора языка
            await remindersManager.ScheduleReminder(profileData).FastAwait();
        }

        profileData.lastActivityTime = DateTime.UtcNow.AsDateTimeString();
        profileData.lastVersion = ProjectVersion.Current.ToString();
        profileData.username = session.actualUser.Username;

        var dbid = profileData.dbid;
        var rawDynamicData = await db.GetOrNullAsync<RawProfileDynamicData>(dbid).FastAwait();
        if (rawDynamicData == null)
        {
            rawDynamicData = new RawProfileDynamicData() { dbid = dbid };
            await db.InsertAsync(rawDynamicData).FastAwait();
        }
        var profileDynamicData = ProfileDynamicData.Deserialize(rawDynamicData);

        var profileBuildingsData = await db.GetOrNullAsync<ProfileBuildingsData>(dbid).FastAwait();
        if (profileBuildingsData == null)
        {
            profileBuildingsData = new ProfileBuildingsData() { dbid = dbid };
            await db.InsertAsync(profileBuildingsData).FastAwait();
        }

        var rawDailyData = await db.GetOrNullAsync<RawProfileDailyData>(dbid).FastAwait();
        ProfileDailyData dailyData;
        if (rawDailyData == null)
        {
            dailyData = ProfileDailyData.Create(profileData, profileDynamicData, profileBuildingsData);
            rawDailyData = new RawProfileDailyData();
            rawDailyData.Fill(dailyData);
            await db.InsertAsync(rawDailyData).FastAwait();
        }
        else
        {
            dailyData = ProfileDailyData.Deserialize(rawDailyData);
        }

        var profile = new Profile(session, profileData, profileDynamicData, profileBuildingsData, dailyData);
        return profile;
    }

}
