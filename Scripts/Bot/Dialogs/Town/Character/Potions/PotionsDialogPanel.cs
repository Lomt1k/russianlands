using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Potions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Potions
{
    public class PotionsDialogPanel : DialogPanelBase
    {
        public PotionsDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
            foreach (var item in session.player.potions)
            {
                RegisterButton(item.GetNameForList(session), () => OpenPotionInfo(item));
            }
        }

        public override async Task SendAsync()
        {
            var playerPotions = session.player.potions;
            var readyPotionsCount = playerPotions.GetReadyPotionsCount();
            var inProductionCount = playerPotions.Count - readyPotionsCount;

            var sb = new StringBuilder();
            sb.AppendLine(string.Format(Localization.Get(session, "dialog_potions_ready_amount"),
                readyPotionsCount));
            sb.AppendLine(string.Format(Localization.Get(session, "dialog_potions_in_production_amount"),
                inProductionCount));

            await SendPanelMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }

        private async Task OpenPotionInfo(PotionItem item)
        {
            await new PotionItemInfoDialog(session, item).Start()
                .ConfigureAwait(false);
        }

    }
}
