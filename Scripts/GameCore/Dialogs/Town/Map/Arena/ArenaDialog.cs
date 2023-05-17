using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Dialogs.Battle;
using MarkOne.Scripts.GameCore.Dialogs.Resources;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map.Arena;
public class ArenaDialog : DialogBase
{
    private static readonly ResourceData ticketPrice = new ResourceData(ResourceId.ArenaTicket, 1);
    private static readonly byte targetBattlesCount = gameDataHolder.arenaSettings.battlesInMatch;

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

        RegisterButton(Emojis.ButtonMarket + Localization.Get(session, "dialog_arena_shop_button"), null); // заглушка
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
        var arenaProgress = session.profile.dynamicData.arenaProgress.EnsureNotNull();
        var canWinAllBattles = arenaProgress.results.Count(x => x.result != BattleResult.Win) == 0;

        var sb = new StringBuilder()
            .AppendLine(Localization.Get(session, "dialog_arena_match_making_description"))
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_arena_match_making_approximate_time", 180)); // заглушка

        if (canWinAllBattles)
        {
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_arena_match_making_challenge", targetBattlesCount));
            sb.AppendLine(GetRewardsForWinAllBattles(arenaProgress.byTicket).GetLocalizedView(session));
        }

        ClearButtons();
        RegisterButton(Emojis.ElementCancel + Localization.Get(session, "dialog_arena_cancel_match_making_button"), TryCancelNextBattle);
        await SendDialogMessage(sb, GetOneLineKeyboard()).FastAwait();
        // TODO: Register player in match making
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

    private async Task TryCancelNextBattle()
    {
        // TODO: Check cancel available
        await Start().FastAwait();
    }

}
