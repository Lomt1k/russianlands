using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Character.Avatars;
public class AvatarsDialog : DialogBase
{
    public AvatarsDialog(GameSession _session) : base(_session)
    {
        RegisterBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
            () => new CharacterDialog(session).Start());
        RegisterTownButton(isDoubleBack: true);
    }

    public override async Task Start()
    {
        var text = "Возможность смены аватарок появится в ближайшем обновлении"; // заглушка
        await SendDialogMessage(text, GetOneLineKeyboard()).FastAwait();
    }
}
