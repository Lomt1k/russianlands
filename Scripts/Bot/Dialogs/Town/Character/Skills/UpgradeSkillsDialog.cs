using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Skills;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Skills
{
    internal class UpgradeSkillsDialog : DialogBase
    {
        private PlayerResources _resources;
        private PlayerSkills _skills;

        public UpgradeSkillsDialog(GameSession _session) : base(_session)
        {
            _resources = session.player.resources;
            _skills = session.player.skills;
        }

        public override async Task Start()
        {
            var sb = new StringBuilder();
            sb.AppendLine(_resources.GetFruitsView());

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_skills_upgrade_recipes"));
            foreach (var itemType in PlayerSkills.GetAllSkillTypes())
            {
                var requiredFruits = _skills.GetRequiredFruits(itemType);
                var isFirstFruit = true;
                foreach (var resourceType in requiredFruits)
                {
                    if (!isFirstFruit)
                    {
                        sb.Append(" +");
                    }
                    sb.Append(' ' + resourceType.GetEmoji().ToString());
                    isFirstFruit = false;
                }
                sb.Append(Emojis.bigSpace + (itemType.GetCategoryLocalization(session)).Bold() + itemType.GetEmoji());
                sb.AppendLine();
            }

            ClearButtons();
            RegisterSkillButtons();
            RegisterBackButton(Localization.Get(session, "menu_item_skills") + Emojis.ButtonSkills,
                () => new SkillsDialog(session).Start());
            RegisterDoubleBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
                () => new TownCharacterDialog(session).Start());

            sb.AppendLine();
            if (_skills.IsAllSkillsMax())
            {
                sb.AppendLine(Localization.Get(session, "dialog_skills_all_skills_has_max_level"));
            }
            else
            {
                sb.AppendLine(Localization.Get(session, "dialog_skills_upgrade_select_skill"));
            }

            await SendDialogMessage(sb, GetKeyboardWithRowSizes(3, 3, 2, 2))
                .ConfigureAwait(false);
        }

        private void RegisterSkillButtons()
        {
            if (_skills.IsAllSkillsMax())
                return;

            foreach (var itemType in PlayerSkills.GetAllSkillTypes())
            {
                RegisterButton(itemType.GetEmoji() + itemType.GetCategoryLocalization(session),
                    () => new UpgradeSelectedSkillDialog(session, itemType).Start());
            }
        }


    }
}
