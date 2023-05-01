using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Skills;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Skills
{
    internal class SkillsDialog : DialogBase
    {
        private PlayerResources _resources;
        private PlayerSkills _skills;

        public SkillsDialog(GameSession _session) : base(_session)
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
                foreach (var resourceId in requiredFruits)
                {
                    if (!isFirstFruit)
                    {
                        sb.Append(" +");
                    }
                    sb.Append(' ' + resourceId.GetEmoji().ToString());
                    isFirstFruit = false;
                }
                sb.Append(Emojis.bigSpace + (itemType.GetCategoryLocalization(session)).Bold() + itemType.GetEmoji());
                sb.AppendLine();
            }

            ClearButtons();
            RegisterSkillButtons();
            RegisterBackButton(Localization.Get(session, "menu_item_character") + Emojis.AvatarMale,
                () => new TownCharacterDialog(session).Start());
            RegisterTownButton(isDoubleBack: true);

            sb.AppendLine();
            if (_skills.IsAllSkillsMax())
            {
                sb.AppendLine(Localization.Get(session, "dialog_skills_all_skills_has_max_level"));
            }
            else
            {
                sb.AppendLine(Localization.Get(session, "dialog_skills_upgrade_select_skill"));
            }

            await SendDialogMessage(sb, GetKeyboardWithRowSizes(3, 3, 2, 2)).FastAwait();
        }

        private void RegisterSkillButtons()
        {
            if (_skills.IsAllSkillsMax())
                return;

            foreach (var itemType in PlayerSkills.GetAllSkillTypes())
            {
                var availableUpgradesByFruits = GetAvailableSkillUpgradesByFruits(itemType);
                var availableUpgradesBySkillLimit = _skills.GetSkillLimit() - _skills.GetValue(itemType);
                var alailableUpgrades = (byte)Math.Min(availableUpgradesByFruits, availableUpgradesBySkillLimit);

                var emoji = alailableUpgrades > 0 ? Emojis.GetNumeric(alailableUpgrades) : itemType.GetEmoji();
                RegisterButton(emoji + itemType.GetCategoryLocalization(session),
                    () => new UpgradeSkillDialog(session, itemType, alailableUpgrades).Start());
            }
        }

        private int GetAvailableSkillUpgradesByFruits(ItemType itemType)
        {
            int result = int.MaxValue;
            var requiredFruits = _skills.GetRequiredFruits(itemType);
            foreach (var resourceId in requiredFruits)
            {
                var resourceAmount = _resources.GetValue(resourceId);
                if (resourceAmount < result)
                {
                    result = resourceAmount;
                }
            }
            return result;
        }

        private Dictionary<ResourceId, int> GetRequiredFruits(ItemType itemType)
        {
            var requiredFruits = _skills.GetRequiredFruits(itemType);
            return new Dictionary<ResourceId, int>
            {
                { requiredFruits[0], 1 },
                { requiredFruits[1], 1 },
                { requiredFruits[2], 1 },
            };
        }

    }
}
