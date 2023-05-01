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
    internal class UpgradeSkillDialog : DialogBase
    {
        private PlayerResources _resources;
        private PlayerSkills _skills;
        private ItemType _itemType;
        private byte _upgradeButtons;

        public UpgradeSkillDialog(GameSession _session, ItemType itemType, byte availableUpgrades) : base(_session)
        {
            _resources = session.player.resources;
            _skills = session.player.skills;
            _itemType = itemType;
            _upgradeButtons = Math.Min(availableUpgrades, (byte)10);

            for (byte i = 1; i <= _upgradeButtons; i++)
            {
                var amountForDelegate = i; //it is important!
                RegisterButton(i.ToString(), () => TryUpgrade(amountForDelegate));
            }
            RegisterBackButton(() => new SkillsDialog(session).Start());
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
            var ourResources = new Dictionary<ResourceId, int>()
            {
                { requiredFruits[0], _resources.GetValue(requiredFruits[0]) },
                { requiredFruits[1], _resources.GetValue(requiredFruits[1]) },
                { requiredFruits[2], _resources.GetValue(requiredFruits[2]) },
                { ResourceId.Herbs, session.player.resources.GetValue(ResourceId.Herbs) }
            };
            sb.Append(ResourceHelper.GetResourcesView(session, ourResources));

            sb.AppendLine();
            if (_skills.IsMaxLevel(_itemType))
            {
                sb.AppendLine(Localization.Get(session, "dialog_skills_upgrade_skill_has_max_level"));
            }
            else if (!_resources.HasEnough(GetRequiredFruits()))
            {
                sb.AppendLine(Emojis.ElementWarningGrey.ToString() + Localization.Get(session, "dialog_skills_upgrade_skill_no_upgrades_available"));
            }
            else
            {
                sb.Append(Localization.Get(session, "dialog_skills_upgrade_elixirs_amount"));
            }

            await SendDialogMessage(sb, GetSpecialKeyboard()).FastAwait();
        }

        private Dictionary<ResourceId, int> GetRequiredResources()
        {
            var elixirWorkshop = (ElixirWorkshopBuilding)BuildingId.ElixirWorkshop.GetBuilding();
            var requiredFruits = _skills.GetRequiredFruits(_itemType);
            return new Dictionary<ResourceId, int>
            {
                { requiredFruits[0], 1 },
                { requiredFruits[1], 1 },
                { requiredFruits[2], 1 },
                { ResourceId.Herbs, elixirWorkshop.GetCurrentElixirPriceInHerbs(session.profile.buildingsData) },
            };
        }

        private Dictionary<ResourceId, int> GetRequiredFruits()
        {
            var requiredFruits = _skills.GetRequiredFruits(_itemType);
            return new Dictionary<ResourceId, int>
            {
                { requiredFruits[0], 1 },
                { requiredFruits[1], 1 },
                { requiredFruits[2], 1 },
            };
        }

        private ReplyKeyboardMarkup GetSpecialKeyboard()
        {
            return _upgradeButtons > 5
                ? GetKeyboardWithRowSizes(5, _upgradeButtons - 5, 1)
                :  GetKeyboardWithRowSizes(_upgradeButtons, 1);
        }

        private async Task TryUpgrade(byte amount)
        {
            var requiredResources = GetRequiredResources();
            var requiredReourceTypes = requiredResources.Keys.ToArray();
            foreach (var resourceId in requiredReourceTypes)
            {
                requiredResources[resourceId] *= amount;
            }

            var playerResources = session.player.resources;
            var successfullPurchase = playerResources.TryPurchase(requiredResources, out var notEnoughResources);
            if (successfullPurchase)
            {
                await SkillUp(amount).FastAwait();
                return;
            }

            var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, notEnoughResources,
                onSuccess: async () => await new UpgradeSkillDialog(session, _itemType, _upgradeButtons).TryUpgrade(amount).FastAwait(),
                onCancel: async () => await new UpgradeSkillDialog(session, _itemType, _upgradeButtons).Start().FastAwait());
            await buyResourcesDialog.Start().FastAwait();
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

            await SendDialogMessage(sb, GetOneLineKeyboard()).FastAwait();
        }

    }
}
