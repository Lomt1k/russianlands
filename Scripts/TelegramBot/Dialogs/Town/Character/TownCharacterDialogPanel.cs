using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    public class TownCharacterDialogPanel : DialogPanelBase
    {
        private bool _isUnitsViewShowed;

        public TownCharacterDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
        }

        public override async Task SendAsync()
        {
            await ShowUnitView()
                .ConfigureAwait(false);

            if (!session.player.unitStats.isFullHealth)
            {
                WaitOneSecondAndInvokeRegen();
            }
        }

        public async Task ShowUnitView()
        {
            ClearButtons();
            RegisterButton($"{Emojis.elements[Element.Info]} {Localization.Get(session, "dialog_character_attributes_tooltip")}", () => ShowAttributesInfo());
            var text = session.player.unitStats.GetView(session);

            await SendPanelMessage(text, GetMultilineKeyboard())
                .ConfigureAwait(false);
            _isUnitsViewShowed = true;
        }

        public async Task ShowAttributesInfo()
        {
            _isUnitsViewShowed = false;
            ClearButtons();
            RegisterBackButton(() => ShowUnitView());
            var text = string.Format(Localization.Get(session, "dialog_character_attributes_info"), 
                Emojis.stats[Stat.AttributeStrength], Emojis.stats[Stat.AttributeVitality],
                Emojis.stats[Stat.AttributeSorcery], Emojis.stats[Stat.AttributeLuck]);

            await SendPanelMessage(text, GetOneLineKeyboard())
                .ConfigureAwait(false);
        }

        private async void WaitOneSecondAndInvokeRegen()
        {
            try
            {
                while (!session.player.unitStats.isFullHealth)
                {
                    await Task.Delay(1_000).ConfigureAwait(false);
                    if (session.IsTasksCancelled() || dialog != session.currentDialog)
                        return;

                    if (_isUnitsViewShowed)
                    {
                        session.player.healhRegenerationController.InvokeRegen();
                        await ShowUnitView()
                            .ConfigureAwait(false);
                    }
                }
            }
            catch (System.Exception ex) { } //ignored
        }

    }
}
