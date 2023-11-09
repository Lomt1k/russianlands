using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Character.Avatars;
public class AvatarsDialog : DialogWithPanel
{
    private AvatarDialogPanel _avatarDialogPanel;
    public override DialogPanelBase DialogPanel => _avatarDialogPanel;

    public AvatarsDialog(GameSession _session) : base(_session)
    {
        _avatarDialogPanel = new AvatarDialogPanel(this);
        RegisterBackButton(Localization.Get(session, "menu_item_character"),
            () => new CharacterDialog(session).Start());
        RegisterTownButton(isDoubleBack: true);
    }    

    public override async Task Start()
    {
        var header = Emojis.ButtonAvatar + Localization.Get(session, "menu_item_avatar").Bold();
        await SendDialogMessage(header.Bold(), GetOneLineKeyboard()).FastAwait();
        await _avatarDialogPanel.Start().FastAwait();
    }
}
