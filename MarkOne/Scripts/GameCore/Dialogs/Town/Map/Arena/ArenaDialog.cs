﻿using FastTelegramBot.DataTypes.InputFiles;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Dialogs.Battle;
using MarkOne.Scripts.GameCore.Dialogs.Resources;
using MarkOne.Scripts.GameCore.Input;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map.Arena;
public sealed class ArenaDialog : DialogBase
{
    private static readonly ResourceData ticketPrice = new ResourceData(ResourceId.ArenaTicket, 1);
    private static readonly byte targetBattlesCount = gameDataHolder.arenaSettings.battlesInMatch;
    private static readonly byte requiredTownhall = gameDataHolder.arenaSettings.requiredTownhall;

    private static readonly ArenaMatchMaker arenaMatchMaker = ServiceLocator.Get<ArenaMatchMaker>();

    public ArenaDialog(GameSession _session) : base(_session)
    {
    }

    public override async Task Start()
    {
        ClearButtons();
        var sb = new StringBuilder()
            .AppendLine(Emojis.ButtonArena + Localization.Get(session, "menu_item_arena").Bold())
            .AppendLine();

        var hasArenaProgress = session.profile.dynamicData.HasArenaProgress();
        if (hasArenaProgress)
        {
            var arenaProgress = session.profile.dynamicData.arenaProgress.EnsureNotNull();
            if (arenaProgress.results.Count >= targetBattlesCount)
            {
                await ShowResults().FastAwait();
                return;
            }

            var registrationType = arenaProgress.byTicket
                ? Localization.Get(session, "dialog_arena_registered_by_ticket")
                : Localization.Get(session, "dialog_arena_registered_by_food");
            sb.AppendLine(Localization.Get(session, "dialog_arena_in_progress", registrationType, targetBattlesCount));
            sb.AppendLine();
            sb.AppendLine(GetBattlesProgressView(arenaProgress, targetBattlesCount));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_arena_battles_to_end", targetBattlesCount - arenaProgress.results.Count));

            RegisterButton(Localization.Get(session, "dialog_arena_next_battle_button"), StartNextBattle);
        }
        else
        {
            sb.AppendLine(Localization.Get(session, "dialog_arena_not_in_progress"));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_ours"));
            var resources = new ResourceData[]
            {
                session.player.resources.GetResourceData(ResourceId.Food),
                session.player.resources.GetResourceData(ResourceId.ArenaTicket),
            };
            sb.AppendLine(resources.GetLocalizedView(session));
            sb.Append(Localization.Get(session, "dialog_arena_bonus_for_ticket"));

            RegisterButton(GetFoodPrice().GetCompactView(shortView: false), () => TryStartNewMatch(byTicket: false));
            RegisterButton(ticketPrice.GetCompactView(shortView: false), () => TryStartNewMatch(byTicket: true));
        }

        RegisterButton(Emojis.ElementScales + Localization.Get(session, "dialog_arena_shop_button") + (CanCollectFreeChips(session) ? Emojis.ElementWarningRed.ToString() : string.Empty),
            () => new ShopArenaDialog(session).Start());
        RegisterBackButton(Localization.Get(session, "menu_item_map") + Emojis.ButtonMap, () => new MapDialog(session).Start());
        RegisterTownButton(isDoubleBack: true);

        await SendDialogMessage(sb, hasArenaProgress 
            ? GetMultilineKeyboardWithDoubleBack()
            : GetKeyboardWithRowSizes(2,1,2)).FastAwait();
    }

    private string GetBattlesProgressView(PlayerArenaProgress playerArenaProgress, byte battlesCount)
    {
        var sb = new StringBuilder()
            .Append(Localization.Get(session, "dialog_arena_progress_header"));

        var results = playerArenaProgress.results;
        for (int i = 0; i < battlesCount; i++)
        {
            if (i < results.Count)
            {
                var result = results[i];
                var emoji = result.result switch
                {
                    BattleResult.Win => Emojis.ProgressBarPositive,
                    BattleResult.Lose => Emojis.ProgressBarNegative,
                    _ => Emojis.ProgressBarNeutral
                };
                sb.Append(emoji);
                continue;
            }
            sb.Append(Emojis.ProgressBarEmpty);
        }
        sb.Append($" {results.Count} / {battlesCount}");

        return sb.ToString();
    }

    private async Task TryStartNewMatch(bool byTicket)
    {
        var price = byTicket ? ticketPrice : GetFoodPrice();
        var playerResources = session.player.resources;
        if (playerResources.TryPurchase(price, out var notEnoughResources))
        {
            Program.logger.Info($"User {session.actualUser} started new arena with {price.resourceId} (on player level: {session.player.level})");
            await StartNewMatch(byTicket).FastAwait();
            return;
        }
        await new BuyResourcesForDiamondsDialog(session, notEnoughResources,
            onSuccess: () => new ArenaDialog(session).TryStartNewMatch(byTicket),
            onCancel: () => new ArenaDialog(session).Start())
            .Start().FastAwait();
    }

    private ResourceData GetFoodPrice()
    {
        var townhallLevel = session.player.buildings.GetBuildingLevel(BuildingId.TownHall);
        var arenaLevelSettings = gameDataHolder.arenaSettings.GetTownhallSettings(townhallLevel);
        return new ResourceData(ResourceId.Food, arenaLevelSettings.foodPrice);
    }

    private async Task StartNewMatch(bool byTicket)
    {
        session.profile.dynamicData.arenaProgress = new PlayerArenaProgress() { byTicket = byTicket };
        await StartNextBattle().FastAwait();
    }

    private async Task StartNextBattle()
    {
        var estimatedTime = await arenaMatchMaker.TryRegisterPlayer(session.player).FastAwait();
        if (!estimatedTime.HasValue)
        {
            var notification = Localization.Get(session, "dialog_arena_registration_error");
            await notificationsManager.ShowNotification(session, notification, () => new ArenaDialog(session).Start()).FastAwait();
            return;
        }

        var arenaProgress = session.profile.dynamicData.arenaProgress.EnsureNotNull();
        var canWinAllBattles = arenaProgress.results.Count(x => x.result != BattleResult.Win) == 0;

        var sb = new StringBuilder()
            .AppendLine(Localization.Get(session, "dialog_arena_match_making_description"))
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_arena_match_making_approximate_time", estimatedTime.Value.GetView(session)));

        if (canWinAllBattles)
        {
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_arena_match_making_challenge_all_wins", targetBattlesCount));
            sb.AppendLine(GetRewardsForWinAllBattles(arenaProgress.byTicket).GetLocalizedView(session));
        }
        else
        {
            var canEndAllBattlesWithoutLose = arenaProgress.results.Count(x => x.result == BattleResult.Lose) == 0;
            if (canEndAllBattlesWithoutLose)
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "dialog_arena_match_making_challenge_no_lose"));
                sb.AppendLine(GetRewardForEndAllBattlesWithoutLose(arenaProgress.byTicket).GetLocalizedView(session));
            }
        }

        ClearButtons();
        RegisterButton(Emojis.ElementCancel + Localization.Get(session, "dialog_arena_cancel_match_making_button"), TryCancelNextBattle);
        await SendDialogPhotoMessage(InputFiles.Photo_Arena, sb.ToString(), GetOneLineKeyboard()).FastAwait();
    }

    private IEnumerable<ResourceData> GetRewardsForWinAllBattles(bool byTicket)
    {
        var rewardSetiings = byTicket
            ? gameDataHolder.arenaSettings.battleRewardsForTicket
            : gameDataHolder.arenaSettings.battleRewardsForFood;

        if (rewardSetiings.ticketsForWinAllBattles > 0)
        {
            yield return new ResourceData(ResourceId.ArenaTicket, rewardSetiings.ticketsForWinAllBattles);
        }
        if (rewardSetiings.chipsForWinAllBattles > 0)
        {
            yield return new ResourceData(ResourceId.ArenaChip, rewardSetiings.chipsForWinAllBattles);
        }
    }

    private ResourceData GetRewardForEndAllBattlesWithoutLose(bool byTicket)
    {
        var rewardSetiings = byTicket
            ? gameDataHolder.arenaSettings.battleRewardsForTicket
            : gameDataHolder.arenaSettings.battleRewardsForFood;
        return new ResourceData(ResourceId.ArenaChip, rewardSetiings.chipsForDrawAllBattles);
    }

    private async Task TryCancelNextBattle()
    {
        var success = await arenaMatchMaker.TryUnregisterPlayer(session.player).FastAwait();
        if (success)
        {
            await Start().FastAwait();
        }
    }

    private async Task ShowResults()
    {
        var arenaProgress = session.profile.dynamicData.arenaProgress.EnsureNotNull();
        var rewardSetiings = arenaProgress.byTicket
            ? gameDataHolder.arenaSettings.battleRewardsForTicket
            : gameDataHolder.arenaSettings.battleRewardsForFood;

        var sb = new StringBuilder()
            .AppendLine(Localization.Get(session, "dialog_arena_results_description"))
            .AppendLine()
            .AppendLine(Localization.Get(session, "battle_result_header_rewards"));

        var totalRewards = new Dictionary<ResourceId, int>()
        {
            { ResourceId.ArenaChip, 0 },
            { ResourceId.ArenaTicket, 0 },
        };

        var winsCount = arenaProgress.results.Count(x => x.result == BattleResult.Win);
        if (winsCount > 0)
        {
            var chipsCount = winsCount * rewardSetiings.chipsForBattleWin;
            var winsReward = new ResourceData(ResourceId.ArenaChip, chipsCount);
            totalRewards[ResourceId.ArenaChip] += chipsCount;
            sb.AppendLine(Localization.Get(session, "dialog_arena_results_for_victory", winsReward.GetCompactView()));
        }

        var drawsCount = arenaProgress.results.Count(x => x.result == BattleResult.Draw);
        if (drawsCount > 0)
        {
            var chipsCount = drawsCount * rewardSetiings.chipsForBattleDraw;
            var chipsReward = new ResourceData(ResourceId.ArenaChip, chipsCount);
            totalRewards[ResourceId.ArenaChip] += chipsCount;
            sb.AppendLine(Localization.Get(session, "dialog_arena_results_for_draw", chipsReward.GetCompactView()));
        }

        if (rewardSetiings.chipsForMatchEnd > 0)
        {
            var chipsReward = new ResourceData(ResourceId.ArenaChip, rewardSetiings.chipsForMatchEnd);
            totalRewards[ResourceId.ArenaChip] += chipsReward.amount;
            sb.AppendLine(Localization.Get(session, "dialog_arena_results_for_participation", chipsReward.GetCompactView()));
        }

        var isWinAllBattles = winsCount == arenaProgress.results.Count;
        if (isWinAllBattles)
        {
            if (rewardSetiings.ticketsForWinAllBattles > 0)
            {
                var ticketsReward = new ResourceData(ResourceId.ArenaTicket, rewardSetiings.ticketsForWinAllBattles);
                totalRewards[ResourceId.ArenaTicket] += ticketsReward.amount;
                sb.AppendLine(Localization.Get(session, "dialog_arena_results_special_reward", ticketsReward.GetCompactView()));
            }
            if (rewardSetiings.chipsForWinAllBattles > 0)
            {
                var chipsReward = new ResourceData(ResourceId.ArenaChip, rewardSetiings.chipsForWinAllBattles);
                totalRewards[ResourceId.ArenaChip] += chipsReward.amount;
                sb.AppendLine(Localization.Get(session, "dialog_arena_results_special_reward", chipsReward.GetCompactView()));
            }
        }
        else
        {
            var isEndAllBattlesWithoutLose = arenaProgress.results.Count(x => x.result == BattleResult.Lose) == 0;
            if (isEndAllBattlesWithoutLose)
            {
                var chipsReward = new ResourceData(ResourceId.ArenaChip, rewardSetiings.chipsForDrawAllBattles);
                totalRewards[ResourceId.ArenaChip] += chipsReward.amount;
                sb.AppendLine(Localization.Get(session, "dialog_arena_results_special_reward", chipsReward.GetCompactView()));
            }
        }

        var totalRewardsList = new List<ResourceData>();
        if (totalRewards[ResourceId.ArenaChip] > 0)
        {
            totalRewardsList.Add(new ResourceData(ResourceId.ArenaChip, totalRewards[ResourceId.ArenaChip]));
        }
        if (totalRewards[ResourceId.ArenaTicket] > 0)
        {
            totalRewardsList.Add(new ResourceData(ResourceId.ArenaTicket, totalRewards[ResourceId.ArenaTicket]));
        }
        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "dialog_arena_results_total_header"));
        sb.AppendLine(totalRewardsList.GetLocalizedView(session));

        Program.logger.Info($"User {session.actualUser} completed the Arena");
        session.player.resources.Add(totalRewardsList);
        session.profile.dynamicData.arenaProgress = null;
        session.profile.data.lastArenaLeagueFarmedChips += totalRewards[ResourceId.ArenaChip];
        ArenaHelper.TryUpLeagueByFarmedChips(session.player);

        RegisterButton(Localization.Get(session, "menu_item_continue_button"), Start);
        await SendDialogMessage(sb, GetOneLineKeyboard()).FastAwait();
    }

    public static bool CanCollectFreeChips(GameSession session)
    {
        var townhall = session.player.buildings.GetBuildingLevel(BuildingId.TownHall);
        if (townhall < requiredTownhall)
            return false;

        var cooldown = gameDataHolder.arenaShopSettings[townhall].freeChipsDelayInSeconds;
        var nextCollectTime = session.profile.data.lastCollectArenaChipsTime.AddSeconds(cooldown);
        return System.DateTime.UtcNow >= nextCollectTime;
    }

}
