using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Locations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Services.DailyDataManagers;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Units.Mobs;
using MarkOne.Scripts.GameCore.Dialogs.Battle;
using MarkOne.Scripts.GameCore.Dialogs.Town.Map;
using MarkOne.Scripts.GameCore.Dialogs.Town;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Resources;

namespace MarkOne.Scripts.GameCore.Services.Mobs;

public class LocationMobsManager : Service
{
    private static readonly ServerDailyDataManager serverDailyDataManager = ServiceLocator.Get<ServerDailyDataManager>();
    private static readonly NotificationsManager notificationsManager = ServiceLocator.Get<NotificationsManager>();
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();
    private static readonly MobFactory mobFactory = ServiceLocator.Get<MobFactory>();

    private Dictionary<MobDifficulty, LocationsMobPack> _mobPacks = new();

    public LocationsMobPack this[MobDifficulty mobDifficulty] => _mobPacks[mobDifficulty];

    public bool isMobsReady { get; private set; }

    public LocationMobsManager()
    {
        serverDailyDataManager.onStartWithOldDay += OnStartWithOldDay;
        serverDailyDataManager.onStartNewDay += OnStartNewDay;
    }

    private void OnStartWithOldDay()
    {
        isMobsReady = false;
        LoadMobPacks();
    }

    private void OnStartNewDay(DateTime oldDate, DateTime newDate)
    {
        isMobsReady = false;
        CreateNewMobPacks();
    }

    private void LoadMobPacks()
    {
        var jsonStr = serverDailyDataManager.GetStringValue("locationMobs", string.Empty);
        if (string.IsNullOrEmpty(jsonStr))
        {
            CreateNewMobPacks();
            return;
        }
        var loadedMobPacks = JsonConvert.DeserializeObject<Dictionary<MobDifficulty, LocationsMobPack>>(jsonStr);
        if (loadedMobPacks is null)
        {
            Program.logger.Error("LocationMobsManager: Error when try to desealize 'locationMobs'. Creating new mobs...");
            CreateNewMobPacks();
            return;
        }
        _mobPacks = loadedMobPacks;
        Program.logger.Info("Location mobs loaded");
        isMobsReady = true;
    }

    private void CreateNewMobPacks()
    {
        _mobPacks.Clear();
        foreach (MobDifficulty difficulty in Enum.GetValues(typeof(MobDifficulty)))
        {
            _mobPacks[difficulty] = GenerateMobPack(difficulty);
        }
        var jsonStr = JsonConvert.SerializeObject(_mobPacks);
        serverDailyDataManager.SetStringValue("locationMobs", jsonStr);
        Program.logger.Info("New location mobs created");
        isMobsReady = true;
    }

    private LocationsMobPack GenerateMobPack(MobDifficulty difficulty)
    {
        var result = new Dictionary<LocationId, SimpleMobData[]>();
        foreach (LocationId locationId in Enum.GetValues(typeof(LocationId)))
        {
            if (locationId == LocationId.None)
                continue;

            var mobsCount = gameDataHolder.locationGeneratedMobs[locationId].mobsCount;
            var array = new SimpleMobData[mobsCount];
            var excludeNames = new List<string>();
            for (var i = 0; i < mobsCount; i++)
            {
                array[i] = mobFactory.GenerateMobForLocation(difficulty, locationId, excludeNames);
                excludeNames.Add(array[i].localizationKey);
            }
            result[locationId] = array;
        }
        return new LocationsMobPack(result);
    }

    public IReadOnlyList<RewardBase> GetLocationRewards(GameSession session, LocationId locationId)
    {
        var townHall = session.player.buildings.GetBuildingLevel(Buildings.BuildingId.TownHall);
        var locationMobSettings = gameDataHolder.locationGeneratedMobs[locationId].GetClosest(townHall);
        return locationMobSettings.locationRewards;
    }

    public BattlePointData? GetMobBattlePointData(GameSession session, LocationId locationId, byte mobIndex)
    {
        if (!isMobsReady)
            return null;

        var difficulty = session.profile.dailyData.GetLocationMobDifficulty();
        var mobData = this[difficulty][locationId][mobIndex];
        var townHall = session.player.buildings.GetBuildingLevel(Buildings.BuildingId.TownHall);
        var locationMobSettings = gameDataHolder.locationGeneratedMobs[locationId].GetClosest(townHall);

        return new BattlePointData
        {
            mob = new Mob(session, mobData),
            price = new ResourceData(ResourceId.Food, locationMobSettings.foodPrice),
            rewards = locationMobSettings.battleRewards,
            onBackButtonFunc = () => new MapDialog(session).StartWithLocation(locationId),
            onBattleEndFunc = (Player player, BattleResult result) =>
            {
                if (result == BattleResult.Win)
                {
                    var defeatedMobs = player.session.profile.dailyData.GetLocationDefeatedMobs(locationId);
                    defeatedMobs.Add(mobIndex);
                }
                return Task.CompletedTask;
            },
            onContinueButtonFunc = async (Player player, BattleResult result) =>
            {
                if (result == BattleResult.Win && isMobsReady)
                {
                    var defeatedMobs = player.session.profile.dailyData.GetLocationDefeatedMobs(locationId);
                    if (defeatedMobs.Count == this[difficulty][locationId].Length)
                    {
                        await GiveLocationRewards(session, locationId).FastAwait();
                        return;
                    }
                }
                await notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.BattleEnd).FastAwait();
            },
        };
    }

    private async Task GiveLocationRewards(GameSession session, LocationId locationId)
    {
        var townHall = session.player.buildings.GetBuildingLevel(Buildings.BuildingId.TownHall);
        var locationMobSettings = gameDataHolder.locationGeneratedMobs[locationId].GetClosest(townHall);

        var sb = new StringBuilder();
        sb.AppendLine(locationId.GetLocalization(session).Bold());

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "dialog_map_all_enemies_defeated"));

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "battle_result_header_rewards"));
        foreach (var reward in locationMobSettings.locationRewards)
        {
            var addedReward = await reward.AddReward(session).FastAwait();
            if (!string.IsNullOrEmpty(addedReward))
            {
                sb.AppendLine(addedReward);
            }
        }

        var photo = locationId.GetPhoto(session);
        if (photo is not null)
        {
            await notificationsManager.ShowNotification(session, photo, sb, () => notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.BattleEnd)).FastAwait();
        }
        else
        {
            await notificationsManager.ShowNotification(session, sb, () => notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.BattleEnd)).FastAwait();
        }
    }

    public string GetTimerViewUntilMobsRespawn(GameSession session)
    {
        var timeSpan = serverDailyDataManager.GetTimeUntilNextDay();
        return $"{Localization.Get(session, "dialog_map_time_until_mob_respawn_header")}\n{timeSpan.GetView(session)}";
    }

}
