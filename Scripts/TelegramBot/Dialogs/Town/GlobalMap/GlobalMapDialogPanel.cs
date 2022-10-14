﻿using System;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Locations;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.GlobalMap
{
    public class GlobalMapDialogPanel : DialogPanelBase
    {
        public GlobalMapDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
        }

        public override async Task SendAsync()
        {
            await ShowGeneralMap();
        }

        private async Task ShowGeneralMap()
        {
            ClearButtons();
            var locations = Enum.GetValues(typeof(LocationType));
            foreach (LocationType locationType in locations)
            {
                if (locationType == LocationType.None)
                    continue;

                string locationName = locationType.GetLocalization(session);
                if (locationType.IsLocked(session))
                {
                    locationName = Emojis.elements[Element.Locked] + Emojis.space + locationName;
                    RegisterButton(locationName, () => ShowLockedLocationInfo(locationType));
                    continue;
                }
                RegisterButton(locationName, () => ShowLocation(locationType));
            }

            var text = Localization.Get(session, "dialog_map_select_location");
            await SendPanelMessage(text, GetKeyboardWithFixedRowSize(2));
        }

        private async Task ShowLockedLocationInfo(LocationType locationType)
        {
            //TODO
        }

        private async Task ShowLocation(LocationType locationType)
        {
            //TODO
        }

    }
}
