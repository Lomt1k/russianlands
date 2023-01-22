using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Skills
{
    internal class SkillsDialog : DialogBase
    {
        public SkillsDialog(GameSession _session) : base(_session)
        {
            RegisterButton(Emojis.ElementLevelUp + Localization.Get(session, "dialog_skills_upgrade_skill_button"),
                    () => new UpgradeSkillsDialog(session).Start());
            RegisterBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
                () => new TownCharacterDialog(session).Start());
            RegisterTownButton(isDoubleBack: true);
        }

        public override async Task Start()
        {
            var sb = new StringBuilder();
            sb.AppendLine(session.player.skills.GetShortView());
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_skills_description"));

            await SendDialogMessage(sb, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }
    }
}
