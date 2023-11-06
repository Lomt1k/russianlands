using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Dialogs.Events.DailyBonus;
using MarkOne.Scripts.GameCore.Dialogs.Events.ReferralSystem;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Events;
public class EventsDialog : DialogBase
{
    private byte _eventsCount;

    public EventsDialog(GameSession _session) : base(_session)
    {
        RegisterEventButtons();
        RegisterTownButton(isDoubleBack: false);
    }

    private void RegisterEventButtons()
    {
        _eventsCount = 0;
        TryRegisterDailyBonusEvent();
        TryRegisterReferralSystemEvent();
    }

    private void TryRegisterDailyBonusEvent()
    {
        if (!DailyBonusDialog.IsEventAvailable(session))
        {
            return;
        }
        RegisterButton(Emojis.ButtonDailyBonus + Localization.Get(session, "dialog_daily_bonus_event_header"),
            () => new DailyBonusDialog(session).Start());
        _eventsCount++;
    }

    private void TryRegisterReferralSystemEvent()
    {
        if (!ReferralSystemDialog.IsEventAvailable(session))
        {
            return;
        }

        var button = Emojis.AvatarSmirkCat + Localization.Get(session, "dialog_referral_system_header")
            + (ReferralSystemDialog.HasNew(session) ? Emojis.ElementWarningRed.ToString() : string.Empty);
        RegisterButton(button, () => new ReferralSystemDialog(session).Start());
        _eventsCount++;
    }

    public override async Task Start()
    {
        var sb = new StringBuilder()
            .AppendLine(Emojis.ButtonEvents + Localization.Get(session, "menu_item_events").Bold())
            .AppendLine()
            .AppendLine(GetDescription());

        await SendDialogMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    private string GetDescription()
    {
        return _eventsCount > 0
            ? Localization.Get(session, "dialog_events_available_events")
            : Localization.Get(session, "dialog_events_no_events");
    }

    public static bool HasNew(GameSession session)
    {
        return ReferralSystemDialog.HasNew(session);
    }

}
