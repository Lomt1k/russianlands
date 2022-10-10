using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Town;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Battle
{
    public class BattleResultDialog : DialogBase
    {
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
            sb.AppendLine($"{Emojis.menuItems[MenuItem.Battle]} {Localization.Get(session, headerLocalization)}");
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, descriptionLocalization));

            if (_data.battleResult == BattleResult.Win && _data.rewards != null)
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "battle_result_header_rewards"));
                foreach (var reward in _data.rewards)
                {
                    sb.AppendLine(reward.GetRewardView(session));
                }
            }

            if (_data.onContinueButtonFunc != null)
            {
                var continueText = Localization.Get(session, "battle_result_continue_button");
                RegisterButton(continueText, () => _data.onContinueButtonFunc(session.player, _data.battleResult));
            }

            if (_data.isReturnToTownAvailable)
            {
                var returnToTownText = $"{Emojis.menuItems[MenuItem.Town]} {Localization.Get(session, "menu_item_town")}";
                RegisterButton(returnToTownText, ReturnToTown);
            }

            await messageSender.SendTextDialog(session.chatId, sb.ToString(), GetMultilineKeyboard());
        }

        private async Task ReturnToTown()
        {
            await new TownDialog(session, TownEntryReason.FromQuestAction).Start();
        }
    }
}
