using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Skills
{
    internal class UpgradeSkillsDialog : DialogBase
    {
        public UpgradeSkillsDialog(GameSession _session) : base(_session)
        {
        }

        public override async Task Start()
        {
            var sb = new StringBuilder();
            sb.AppendLine(session.player.resources.GetFruitsView());

            ClearButtons();
            RegisterBackButton(Localization.Get(session, "menu_item_skills") + Emojis.ButtonSkills,
                () => new SkillsDialog(session).Start());
            RegisterDoubleBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
                () => new TownCharacterDialog(session).Start());

            await SendDialogMessage(sb, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }
    }
}
