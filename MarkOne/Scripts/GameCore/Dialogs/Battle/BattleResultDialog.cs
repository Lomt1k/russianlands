using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Dialogs.Town;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Dialogs.Battle;

public class BattleResultDialog : DialogBase
{
    private static readonly NotificationsManager notificationsManager = ServiceLocator.Get<NotificationsManager>();

    private BattleResultData _data;

    public BattleResultDialog(GameSession _session, BattleResultData data) : base(_session)
    {
        _data = data;
    }

    public override async Task Start()
    {
        var sb = new StringBuilder();
        var headerLocalization = "battle_result_" + _data.battleResult.ToString().ToLower();
        var descriptionLocalization = "battle_result_description_" + _data.battleResult.ToString().ToLower();
        sb.AppendLine(Emojis.ButtonBattle + Localization.Get(session, headerLocalization));
        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, descriptionLocalization));

        if (_data.battleResult == BattleResult.Win && _data.rewards != null)
        {
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "battle_result_header_rewards"));
            var premiumRewards = new List<ResourceReward>();
            foreach (var reward in _data.rewards)
            {
                var addedReward =  reward is ResourceRewardBase resourceRewardBase
                    ? await resourceRewardBase.AddRewardWithAddPossiblePremiumRewardToList(session, premiumRewards).FastAwait()
                    : await reward.AddReward(session).FastAwait();
                if (!string.IsNullOrEmpty(addedReward))
                {
                    sb.AppendLine(addedReward);
                }
            }

            if (premiumRewards.Count > 0 && session.player.buildings.GetBuildingLevel(BuildingId.TownHall) >= 2)
            {
                sb.AppendLine();
                if (session.player.IsPremiumActive())
                {
                    sb.AppendLine(Localization.Get(session, "battle_result_header_premium_rewards"));
                    foreach (var reward in premiumRewards)
                    {
                        var addedReward = await reward.AddReward(session).FastAwait();
                        if (!string.IsNullOrEmpty(addedReward))
                        {
                            sb.AppendLine(addedReward);
                        }
                    }
                }
                else
                {
                    sb.AppendLine(Localization.Get(session, "battle_result_header_premium_rewards_locked"));
                    foreach (var reward in premiumRewards)
                    {
                        var possibleReward = reward.GetPossibleRewardsView(session);
                        sb.AppendLine(Emojis.ElementLocked + possibleReward);
                    }
                }
            }
        }

        if (_data.onContinueButtonFunc != null)
        {
            var continueText = Localization.Get(session, "battle_result_continue_button");
            RegisterButton(continueText, () => _data.onContinueButtonFunc(session.player, _data.battleResult));
        }

        if (_data.isReturnToTownAvailable)
        {
            var returnToTownText = Emojis.ButtonTown + Localization.Get(session, "menu_item_town");
            RegisterButton(returnToTownText, ReturnToTown);
        }

        await SendDialogMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    private async Task ReturnToTown()
    {
        await notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.FromQuestAction).FastAwait();
    }
}
