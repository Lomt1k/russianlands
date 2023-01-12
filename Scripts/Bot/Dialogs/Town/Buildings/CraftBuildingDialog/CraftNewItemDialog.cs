using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Dialogs.Resources;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings.CraftBuildingDialog
{
    public class CraftNewItemDialog : DialogBase
    {
        private CraftBuildingBase _building;

        private ProfileBuildingsData buildingsData => session.profile.buildingsData;

        public CraftNewItemDialog(GameSession session, CraftBuildingBase building) : base(session)
        {
            _building = building;
        }

        public override async Task Start()
        {
            var sb = new StringBuilder();
            sb.AppendLine(session.player.resources.GetCraftResourcesView());
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_craft_select_item_type"));

            ClearButtons();
            foreach (var itemType in _building.craftCategories)
            {
                RegisterButton($"{Emojis.items[itemType]} {itemType.GetLocalization(session)}", () => StartSelectRarity(itemType));
            }
            RegisterBackButton(() => new BuildingInfoDialog(session, _building).Start());

            await SendDialogMessage(sb, GetSpecialKeyboard())
                .ConfigureAwait(false);
        }

        private ReplyKeyboardMarkup GetSpecialKeyboard()
        {
            return _building.craftCategories.Count switch
            {
                1 => GetMultilineKeyboard(),
                2 => GetKeyboardWithRowSizes(2, 1),
                3 => GetKeyboardWithRowSizes(3, 1),
                4 => GetKeyboardWithRowSizes(2, 2, 1),
                5 => GetKeyboardWithRowSizes(3, 2, 1),
                6 => GetKeyboardWithRowSizes(3, 3, 1),
                _ => GetMultilineKeyboard()
            };
        }

        private async Task StartSelectRarity(ItemType itemType)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{Emojis.items[itemType]} {itemType.GetLocalization(session)}</b>");
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_craft_select_item_rarity"));

            ClearButtons();
            RegisterRarityButton(itemType, Rarity.Rare);
            RegisterRarityButton(itemType, Rarity.Epic);
            RegisterRarityButton(itemType, Rarity.Legendary);
            RegisterBackButton(() => Start());

            await SendDialogMessage(sb, GetKeyboardWithRowSizes(3, 1))
                .ConfigureAwait(false);
        }

        private void RegisterRarityButton(ItemType itemType, Rarity rarity)
        {
            RegisterButton(rarity.GetView(session), () => ShowCraftPrice(itemType, rarity));
        }

        private async Task ShowCraftPrice(ItemType itemType, Rarity rarity)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{Emojis.items[itemType]} {itemType.GetLocalization(session)}</b>");
            sb.AppendLine($"<pre>{rarity.GetView(session)}</pre>");

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "item_view_possible_requirments"));
            var craftItemLevels = _building.GetCurrentCraftLevels(buildingsData);
            sb.AppendLine(string.Format(Localization.Get(session, "level"), craftItemLevels));

            sb.AppendLine();
            var craftPrice = _building.GetCraftPrice(buildingsData, rarity);
            sb.Append(ResourceHelper.GetPriceView(session, craftPrice));
            var craftTimeInSeconds = _building.GetCraftTimeInSeconds(buildingsData, rarity);
            var dtNow = DateTime.UtcNow;
            var timeSpan = (dtNow.AddSeconds(craftTimeInSeconds) - dtNow);
            sb.AppendLine(timeSpan.GetView(session, withCaption: true));

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_ours"));
            var playerResources = new Dictionary<ResourceType, int>();
            foreach (var resourceType in craftPrice.Keys)
            {
                playerResources[resourceType] = session.player.resources.GetValue(resourceType);
            }
            sb.Append(ResourceHelper.GetResourcesView(session, playerResources));

            ClearButtons();
            RegisterButton($"{Emojis.menuItems[MenuItem.Craft]} {Localization.Get(session, "dialog_craft_start_craft_buton")}",
                () => StartCraftItem(itemType, rarity));
            RegisterBackButton(() => StartSelectRarity(itemType));

            await SendDialogMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }

        private async Task StartCraftItem(ItemType itemType, Rarity rarity)
        {
            var playerResources = session.player.resources;
            var requiredResources = _building.GetCraftPrice(buildingsData, rarity);
            var successfullPurchase = playerResources.TryPurchase(requiredResources, out var notEnoughResources);
            if (successfullPurchase)
            {
                _building.StartCraft(buildingsData, itemType, rarity);
                await new CraftInProgressDialog(session, _building).Start()
                    .ConfigureAwait(false);
                return;
            }

            var notEnoughMaterials = notEnoughResources.Where(x => x.Key.IsCraftResource()).ToDictionary(x => x.Key, x => x.Value);
            if (notEnoughMaterials.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine(Localization.Get(session, "dialog_craft_not_enough_materials"));
                sb.AppendLine();
                sb.Append(ResourceHelper.GetResourcesView(session, notEnoughMaterials));
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "dialog_craft_how_to_get_materials"));

                ClearButtons();
                RegisterBackButton(() => ShowCraftPrice(itemType, rarity));

                await SendDialogMessage(sb, GetOneLineKeyboard())
                    .ConfigureAwait(false);
                return;
            }

            var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, notEnoughResources,
                onSuccess: async () => await new CraftNewItemDialog(session, _building).StartCraftItem(itemType, rarity).ConfigureAwait(false),
                onCancel: async () => await new CraftNewItemDialog(session, _building).ShowCraftPrice(itemType, rarity).ConfigureAwait(false));
            await buyResourcesDialog.Start()
                .ConfigureAwait(false);
        }

    }
}
