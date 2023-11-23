using System.Threading.Tasks;
using MarkOne.Scripts.Bot.CallbackData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Dialogs;

public abstract class DialogWithPanel : DialogBase
{
    public abstract DialogPanelBase DialogPanel { get; }

    protected DialogWithPanel(GameSession _session) : base(_session)
    {
    }

    public override async Task TryResendDialog()
    {
        await base.TryResendDialog().FastAwait();
        await DialogPanel.ResendLastMessageAsNew().FastAwait();
    }

    public virtual async Task HandleCallbackQuery(string queryId, DialogPanelButtonCallbackData callback)
    {
        await DialogPanel.HandleButtonPress(callback, queryId).FastAwait();
    }

    public override void OnClose()
    {
        DialogPanel.OnDialogClose();
    }

}
