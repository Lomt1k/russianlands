using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.Bot.Dialogs.Resources;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.General;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Skills;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Skills
{
    internal class UpgradeSelectedSkillDialog : DialogBase
    {
        private PlayerResources _resources;
        private PlayerSkills _skills;
        private ItemType _itemType;

        public UpgradeSelectedSkillDialog(GameSession _session, ItemType itemType) : base(_session)
        {
            _resources = session.player.resources;
            _skills = session.player.skills;
            _itemType = itemType;

            var buttonsLimit = _skills.GetSkillLimit() - _skills.GetValue(_itemType);
            buttonsLimit = Math.Min(buttonsLimit, GetAvailableSkillUpgradesByFruits(_itemType));
            buttonsLimit = Math.Min(buttonsLimit, 5);
            for (byte i = 1; i <= buttonsLimit; i++)
            {
                var amountForDelegate = i; //it is important!
                RegisterButton(i.ToString(), () => TryUpgrade(amountForDelegate));
            }
            RegisterBackButton(() => new UpgradeSkillsDialog(session).Start());
        }

        private int GetAvailableSkillUpgradesByFruits(ItemType itemType)
        {
            int result = int.MaxValue;
            var requiredFruits = _skills.GetRequiredFruits(itemType);
            foreach (var resourceType in requiredFruits)
            {
                var resourceAmount = _resources.GetValue(resourceType);
                if (resourceAmount < result)
                {
                    result = resourceAmount;
                }
            }
            return result;
        }

        public override async Task Start()
        {
            var sb = new StringBuilder();
            sb.AppendLine(_itemType.GetEmoji() + _itemType.GetCategoryLocalization(session).Bold());
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_skills_upgrade_selected_skill_description"));

            sb.AppendLine();
            sb.Append(ResourceHelper.GetPriceView(session, GetRequiredResources()));

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_ours"));
            var requiredFruits = _skills.GetRequiredFruits(_itemType);
            var ourResources = new Dictionary<ResourceType, int>()
            {
                { requiredFruits[0], _resources.GetValue(requiredFruits[0]) },
                { requiredFruits[1], _resources.GetValue(requiredFruits[1]) },
                { requiredFruits[2], _resources.GetValue(requiredFruits[2]) },
                { ResourceType.Herbs, session.player.resources.GetValue(ResourceType.Herbs) }
            };
            sb.Append(ResourceHelper.GetResourcesView(session, ourResources));

            sb.AppendLine();
            sb.Append(Localization.Get(session, "dialog_skills_upgrade_elixirs_amount"));

            await SendDialogMessage(sb, GetSpecialKeyboard())
                .ConfigureAwait(false);
        }

        private Dictionary<ResourceType, int> GetRequiredResources()
        {
            var elixirWorkshop = (ElixirWorkshopBuilding)BuildingType.ElixirWorkshop.GetBuilding();
            var requiredFruits = _skills.GetRequiredFruits(_itemType);
            return new Dictionary<ResourceType, int>
            {
                { requiredFruits[0], 1 },
                { requiredFruits[1], 1 },
                { requiredFruits[2], 1 },
                { ResourceType.Herbs, elixirWorkshop.GetCurrentElixirPriceInHerbs(session.profile.buildingsData) },
            };
        }

        private ReplyKeyboardMarkup GetSpecialKeyboard()
        {
            return GetKeyboardWithRowSizes(buttonsCount - 1, 1);
        }

        private async Task TryUpgrade(byte amount)
        {
            var requiredResources = GetRequiredResources();
            var requiredReourceTypes = requiredResources.Keys.ToArray();
            foreach (var resourceType in requiredReourceTypes)
            {
                requiredResources[resourceType] *= amount;
            }

            var playerResources = session.player.resources;
            var successfullPurchase = playerResources.TryPurchase(requiredResources, out var notEnoughResources);
            if (successfullPurchase)
            {
                await SkillUp(amount)
                    .ConfigureAwait(false);
                return;
            }

            var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, notEnoughResources,
                onSuccess: async () => await new UpgradeSelectedSkillDialog(session, _itemType).TryUpgrade(amount).ConfigureAwait(false),
                onCancel: async () => await new UpgradeSelectedSkillDialog(session, _itemType).Start().ConfigureAwait(false));
            await buyResourcesDialog.Start().ConfigureAwait(false);
        }

        private async Task SkillUp(byte amount)
        {
            _skills.AddValue(_itemType, amount);

            var sb = new StringBuilder();
            sb.AppendLine(_itemType.GetEmoji() + _itemType.GetCategoryLocalization(session).Bold());
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_skills_upgrade_successfull", _skills.GetValue(_itemType)));

            ClearButtons();
            RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => new SkillsDialog(session).Start());

            await SendDialogMessage(sb, GetOneLineKeyboard())
                .ConfigureAwait(false);
        }

    }
}
