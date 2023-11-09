using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Character.Avatars;
public class AvatarDialogPanel : DialogPanelBase
{
    public AvatarDialogPanel(DialogWithPanel _dialog) : base(_dialog)
    {
    }

    public override async Task Start()
    {
        ClearButtons();
        RegisterAvailableAvatars();
        var text = Localization.Get(session, "dialog_avatars_description");
        await SendPanelMessage(text, GetKeyboardWithFixedRowSize(5)).FastAwait();
    }

    private void RegisterAvailableAvatars()
    {
        foreach (var avatarId in Units.Avatars.defaultAvatars)
        {
            RegisterButton(avatarId.GetEmoji().ToString(), () => SelectAvatar(avatarId));
        }
        foreach (var avatarId in session.profile.dynamicData.avatars)
        {
            RegisterButton(avatarId.GetEmoji().ToString(), () => SelectAvatar(avatarId));
        }
    }

    public async Task SelectAvatar(AvatarId avatarId)
    {
        session.profile.data.avatarId = avatarId;
        var unitView = session.player.GetGeneralUnitInfoView(session);
        var text = Localization.Get(session, "dialog_avatars_on_avatar_changed", unitView);

        ClearButtons();
        RegisterButton(Localization.Get(session, "menu_item_continue_button"), Start);

        await SendPanelMessage(text, GetOneLineKeyboard()).FastAwait();
    }

}
