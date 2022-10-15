﻿using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.GlobalMap
{
    public class GlobalMapDialog : DialogBase
    {
        private GlobalMapDialogPanel _mapPanel;

        public GlobalMapDialog(GameSession _session) : base(_session)
        {
            _mapPanel = new GlobalMapDialogPanel(this, 0);
            RegisterPanel(_mapPanel);

            RegisterButton($"{Emojis.locations[MapLocation.Arena]} " + Localization.Get(session, "menu_item_arena"),
                null);
            RegisterBackButton(() => new TownDialog(session, TownEntryReason.BackFromInnerDialog).Start());
        }

        public override async Task Start()
        {
            if (session.tooltipController.IfNextTooltipForPanelWithWaitingButtonClick())
            {
                ClearButtons();
            }
            else
            {
                var header = $"{Emojis.menuItems[MenuItem.Map]} <b>{Localization.Get(session, "menu_item_map")}</b>";
                await SendDialogMessage(header, GetMultilineKeyboard());
            }            
            await SendPanelsAsync();
        }

        public async Task StartWithLocation(LocationType locationType)
        {
            var header = $"{Emojis.menuItems[MenuItem.Map]} <b>{Localization.Get(session, "menu_item_map")}</b>";
            await SendDialogMessage(header, GetMultilineKeyboard());
            await _mapPanel.ShowLocation(locationType);
        }

    }
}
