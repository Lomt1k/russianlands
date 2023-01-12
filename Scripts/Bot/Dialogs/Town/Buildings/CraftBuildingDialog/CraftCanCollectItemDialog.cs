using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings.CraftBuildingDialog
{
    public class CraftCanCollectItemDialog : DialogBase
    {
        private CraftBuildingBase _building;

        private ProfileBuildingsData buildingsData => session.profile.buildingsData;

        public CraftCanCollectItemDialog(GameSession session, CraftBuildingBase building) : base(session)
        {
            _building = building;
        }

        public override async Task Start()
        {
            var itemType = _building.GetCurrentCraftItemType(buildingsData);
            var rarity = _building.GetCurrentCraftItemRarity(buildingsData);

            var sb = new StringBuilder();
            sb.AppendLine($"<b>{Emojis.items[itemType]} {itemType.GetLocalization(session)}</b>");
            sb.AppendLine($"<pre>{rarity.GetView(session)}</pre>");

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "item_view_possible_requirments"));
            var craftItemLevels = _building.GetCurrentCraftLevels(buildingsData);
            sb.AppendLine(string.Format(Localization.Get(session, "level"), craftItemLevels));

            sb.AppendLine();
            sb.AppendLine($"{Emojis.menuItems[MenuItem.Craft]} {Localization.Get(session, "dialog_craft_completed")}");

            //TODO: Collect item button
            RegisterBackButton(() => new BuildingInfoDialog(session, _building).Start());

            await SendDialogMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }
    }
}
