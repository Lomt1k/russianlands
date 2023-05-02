﻿using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Town;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.Scripts.Bot.Dialogs.Battle;

public class BattleResultDialog : DialogBase
{
    private static readonly NotificationsManager notificationsManager = Services.Get<NotificationsManager>();

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

        if (_data.battleResult == BattleResult.Win && _data.rewards != null && _data.rewards.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "battle_result_header_rewards"));
            foreach (var reward in _data.rewards)
            {
                var addedReward = await reward.AddReward(session).FastAwait();
                if (!string.IsNullOrEmpty(addedReward))
                {
                    sb.AppendLine(addedReward);
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
