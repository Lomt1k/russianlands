using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.CallbackData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs
{
    public abstract class DialogWithPanel : DialogBase
    {
        public abstract DialogPanelBase DialogPanel { get; }

        protected DialogWithPanel(GameSession _session) : base(_session)
        {
        }

        public override async Task TryResendDialog()
        {
            await base.TryResendDialog().ConfigureAwait(false);
            await DialogPanel.ResendLastMessageAsNew().ConfigureAwait(false);
        }

        public virtual async Task HandleCallbackQuery(string queryId, DialogPanelButtonCallbackData callback)
        {
            await DialogPanel.HandleButtonPress(callback.buttonId, queryId).ConfigureAwait(false);
        }

        public override void OnClose()
        {
            DialogPanel.OnDialogClose();
        }

    }
}
