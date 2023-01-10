using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
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
            sb.AppendLine($"<pre>{rarity.GetView(session)}</pre>"); //TODO: Add level info

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
            //TODO: Start craft button
            RegisterBackButton(() => StartSelectRarity(itemType));

            await SendDialogMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }

    }
}
