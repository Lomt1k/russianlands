using System;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Profiles;

public class Profile
{
    private static readonly DailyRemindersManager remindersManager = ServiceLocator.Get<DailyRemindersManager>();

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

    public async Task SaveProfile()
    {
        var attemptsCount = 20;
        Exception? cachedException = null;
        for (int i = 1; i <= attemptsCount; i++)
        {
            try
            {
                var db = BotController.dataBase.db;
                db.Update(data);

                var rawDynamicData = new RawProfileDynamicData();
                rawDynamicData.Fill(dynamicData);
                db.Update(rawDynamicData);

                db.Update(buildingsData);

                var rawDailyData = new RawProfileDailyData();
                rawDailyData.Fill(dailyData);
                db.InsertOrReplace(rawDailyData);

                lastSaveProfileTime = DateTime.UtcNow;
                var user = session?.actualUser.ToString() ?? $"(ID {data.telegram_id})";
                Program.logger.Info($"Profile saved for {user}");
                return;
            }
            catch (Exception ex)
            {
                cachedException ??= ex;
                if (i < attemptsCount)
                {
                    Program.logger.Error($"Successfull handled exception on SaveProfile:\n{ex}");
                    await Task.Delay(15);
                    continue;
                }
                throw cachedException;
            }
        }
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
        dailyData.SetupSession(session);
    }

    public static async Task<Profile> Load(GameSession session, string messageText)
    {
        var db = BotController.dataBase.db;
        var query = db.Table<ProfileData>().Where(x => x.telegram_id == session.actualUser.Id);
        var profileData = query.FirstOrDefault();
        if (profileData == null)
        {
            profileData = new ProfileData().SetupNewProfile(session.actualUser, messageText);
            db.Insert(profileData);
        }
        else
        {
            // В первой сессии remindersManager срабатывает только после выбора языка
            await remindersManager.ScheduleReminder(profileData).FastAwait();
            // обновляем firstName, lastName, username в начале сессии
            profileData.firstName = session.actualUser.FirstName;
            profileData.lastName = session.actualUser.LastName;
            profileData.username = session.actualUser.Username;
        }

        profileData.lastActivityTime = DateTime.UtcNow;
        profileData.lastVersion = ProjectVersion.Current.ToString();
        profileData.username = session.actualUser.Username;

        var dbid = profileData.dbid;
        var rawDynamicData = db.GetOrNull<RawProfileDynamicData>(dbid);
        if (rawDynamicData == null)
        {
            rawDynamicData = new RawProfileDynamicData() { dbid = dbid };
            db.Insert(rawDynamicData);
        }
        var profileDynamicData = ProfileDynamicData.Deserialize(rawDynamicData);

        var profileBuildingsData = db.GetOrNull<ProfileBuildingsData>(dbid);
        if (profileBuildingsData == null)
        {
            profileBuildingsData = new ProfileBuildingsData() { dbid = dbid };
            db.Insert(profileBuildingsData);
        }

        var rawDailyData = db.GetOrNull<RawProfileDailyData>(dbid);
        ProfileDailyData dailyData;
        if (rawDailyData == null)
        {
            dailyData = ProfileDailyData.Create(profileData, profileDynamicData, profileBuildingsData);
            rawDailyData = new RawProfileDailyData();
            rawDailyData.Fill(dailyData);
            db.Insert(rawDailyData);
        }
        else
        {
            dailyData = ProfileDailyData.Deserialize(rawDailyData);
        }

        var profile = new Profile(session, profileData, profileDynamicData, profileBuildingsData, dailyData);
        return profile;
    }

}
