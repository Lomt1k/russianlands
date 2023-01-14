using System;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Shop;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings.CraftBuildingDialog
{
    public class CraftInProgressDialog : DialogBase
    {
        private CraftBuildingBase _building;

        private ProfileBuildingsData buildingsData => session.profile.buildingsData;
        public CraftInProgressDialog(GameSession session, CraftBuildingBase building) : base(session)
        {
            _building = building;
        }

        public override async Task Start()
        {
            var itemType = _building.GetCurrentCraftItemType(buildingsData);
            var rarity = _building.GetCurrentCraftItemRarity(buildingsData);

            var sb = new StringBuilder();
            sb.AppendLine(itemType.GetEmoji() + itemType.GetLocalization(session).Bold());
            sb.AppendLine($"<pre>{rarity.GetView(session)}</pre>");

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "item_view_possible_requirments"));
            var craftItemLevels = _building.GetCurrentCraftLevels(buildingsData);
            sb.AppendLine(Localization.Get(session, "level", craftItemLevels));

            sb.AppendLine();
            var timeSpan = _building.GetEndCraftTime(buildingsData) - DateTime.UtcNow;
            var productionView = Localization.Get(session, "dialog_craft_progress", timeSpan.GetView(session));
            sb.AppendLine(Emojis.ElementSmallBlack + productionView);

            ClearButtons();
            var diamondsForBoost = GetBoostPriceInDiamonds();
            var priceView = ResourceType.Diamond.GetEmoji().code + diamondsForBoost;
            var buttonText = Localization.Get(session, "menu_item_boost_button", priceView);
            RegisterButton(buttonText, () => TryBoostCraftForDiamonds());
            RegisterBackButton(() => new BuildingInfoDialog(session, _building).Start());

            await SendDialogMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }

        public async Task TryBoostCraftForDiamonds()
        {
            if (IsCraftCanBeFinished())
            {
                var message = Emojis.ElementClock + Localization.Get(session, "dialog_craft_boost_expired");
                ClearButtons();
                RegisterButton(Localization.Get(session, "menu_item_continue_button"),
                    () => new CraftCanCollectItemDialog(session, _building).Start());
                await SendDialogMessage(message, GetOneLineKeyboard())
                    .ConfigureAwait(false);
                return;
            }

            var requiredDiamonds = GetBoostPriceInDiamonds();
            var playerResources = session.player.resources;
            var successsPurchase = playerResources.TryPurchase(ResourceType.Diamond, requiredDiamonds, out var notEnoughDiamonds);
            if (successsPurchase)
            {
                _building.BoostCraft(buildingsData);

                var sb = new StringBuilder();
                sb.AppendLine(Emojis.ButtonCraft + Localization.Get(session, "dialog_craft_boosted"));
                if (requiredDiamonds > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine(Localization.Get(session, "resource_header_spent"));
                    sb.AppendLine(ResourceType.Diamond.GetLocalizedView(session, requiredDiamonds));
                }

                ClearButtons();
                RegisterButton(Localization.Get(session, "menu_item_continue_button"),
                    () => new CraftCanCollectItemDialog(session, _building).TryToGetItem());

                await SendDialogMessage(sb, GetOneLineKeyboard())
                    .ConfigureAwait(false);
                return;
            }

            ClearButtons();
            var text = Localization.Get(session, "resource_not_enough_diamonds", Emojis.SmileSad);
            RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"),
                () => new ShopDialog(session).Start());
            RegisterBackButton(() => new CraftInProgressDialog(session, _building).Start());

            await SendDialogMessage(text, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }

        public int GetBoostPriceInDiamonds()
        {
            if (IsCraftCanBeFinished())
                return 0;

            var endCraftTime = _building.GetEndCraftTime(buildingsData);
            var seconds = (int)(endCraftTime - DateTime.UtcNow).TotalSeconds;
            return ResourceHelper.CalculateCraftBoostPriceInDiamonds(seconds);
        }

        public bool IsCraftCanBeFinished()
        {
            return _building.IsCraftStarted(buildingsData) && _building.IsCraftCanBeFinished(buildingsData);
        }
    }
}
