using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Locations;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.GlobalMap
{
    public class GlobalMapDialogPanel : DialogPanelBase
    {
        private Message? _lastMessage;

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
            var locations = LocationsHolder.GetAll();
            foreach (var location in locations)
            {
                bool isLocked = location.data.id > session.profile.data.lastUnlockedLocation;
                string locationName = Localization.Get(session, $"menu_item_location_{location.data.id}");
                if (isLocked)
                {
                    locationName = Emojis.elements[Element.Locked] + Emojis.space + locationName;
                    RegisterButton(locationName, () => ShowLockedLocationInfo(location.type));
                    continue;
                }
                RegisterButton(locationName, () => ShowLocation(location.type));
            }

            var text = Localization.Get(session, "dialog_map_select_location");
            _lastMessage = _lastMessage == null 
                ? await messageSender.SendTextMessage(session.chatId, text, GetMultilineKeyboard())
                : await messageSender.EditTextMessage(session.chatId, _lastMessage.MessageId, text, GetMultilineKeyboard());
        }

        private async Task ShowLockedLocationInfo(LocationType locationType)
        {
            //TODO
        }

        private async Task ShowLocation(LocationType locationType)
        {
            //TODO
        }

        public override void OnDialogClose()
        {
        }

    }
}
