using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Skills
{
    internal class SkillsDialog : DialogBase
    {
        public SkillsDialog(GameSession _session) : base(_session)
        {
            RegisterBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
                () => new TownCharacterDialog(session).Start());
            RegisterTownButton(isDoubleBack: true);
        }

        public override async Task Start()
        {
            await SendDialogMessage("In development", GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }
    }
}
