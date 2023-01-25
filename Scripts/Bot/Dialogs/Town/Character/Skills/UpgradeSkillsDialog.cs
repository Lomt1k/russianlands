using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
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
            RegisterAvailableUpgradeButtons();
            RegisterBackButton(Localization.Get(session, "menu_item_skills") + Emojis.ButtonSkills,
                () => new SkillsDialog(session).Start());
            RegisterDoubleBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
                () => new TownCharacterDialog(session).Start());

            sb.AppendLine();
            var isAnyUpgradeAvailable = buttonsCount > 2;
            if (isAnyUpgradeAvailable)
            {
                sb.AppendLine(Localization.Get(session, "dialog_skills_upgrade_select_skill"));
            }
            else if (_skills.IsAllSkillsMax())
            {
                sb.AppendLine(Localization.Get(session, "dialog_skills_all_skills_has_max_level"));
            }
            else
            {                
                sb.AppendLine(Emojis.ElementWarningGrey.ToString() + Localization.Get(session, "dialog_skills_upgrade_skill_no_upgrades_available"));
            }

            await SendDialogMessage(sb, GetSpecialKeyboard())
                .ConfigureAwait(false);
        }

        private void RegisterAvailableUpgradeButtons()
        {
            foreach (var itemType in PlayerSkills.GetAllSkillTypes())
            {
                var requiredResources = GetRequiredResources(itemType);
                var isUpgradeAvailable = _resources.HasEnough(requiredResources) && !_skills.IsMaxLevel(itemType);
                if (isUpgradeAvailable)
                {
                    RegisterButton(itemType.GetEmoji() + itemType.GetCategoryLocalization(session),
                        () => new UpgradeSelectedSkillDialog(session, itemType).Start());
                }
            }
        }

        private Dictionary<ResourceType, int> GetRequiredResources(ItemType itemType)
        {
            var requiredFruits = _skills.GetRequiredFruits(itemType);
            return new Dictionary<ResourceType, int>
            {
                { requiredFruits[0], 1 },
                { requiredFruits[1], 1 },
                { requiredFruits[2], 1 },
            };
        }

        private ReplyKeyboardMarkup GetSpecialKeyboard()
        {
            return buttonsCount switch
            {
                3 => GetKeyboardWithRowSizes(1, 2),
                4 => GetKeyboardWithRowSizes(1, 1, 2),
                5 => GetKeyboardWithRowSizes(1, 1, 1, 2),
                6 => GetKeyboardWithRowSizes(3, 1, 2),
                7 => GetKeyboardWithRowSizes(3, 2, 2),
                8 => GetKeyboardWithRowSizes(3, 3, 2),
                9 => GetKeyboardWithRowSizes(3, 3, 1, 2),
                10 => GetKeyboardWithRowSizes(3, 3, 2, 2),
                11 => GetKeyboardWithRowSizes(3, 3, 3, 2),
                _ => GetOneLineKeyboard(), // only back buttons
            };
        }


    }
}
