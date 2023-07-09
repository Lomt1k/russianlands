using MarkOne.Scripts.GameCore.Dialogs;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Events;
public class EventsDialog : DialogBase
{
    public EventsDialog(GameSession _session) : base(_session)
    {
        RegisterTownButton(isDoubleBack: false);
    }

    public override async Task Start()
    {
        var text = Localization.Get(session, "dialog_enents_no_events");
        await SendDialogMessage(text, GetOneLineKeyboard()).FastAwait();
    }
}
