using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.GlobalMap
{
    internal class GlobalMapDialogPanel : DialogPanelBase
    {
        private Message? _lastMessage;

        public GlobalMapDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
        }

        public override async Task SendAsync()
        {
            await SendGeneralMap();
        }

        private async Task SendGeneralMap()
        {
            ClearButtons();
            var locations = GameCore.Locations.LocationsHolder.GetAll();
            foreach (var location in locations)
            {
                bool isLocked = location.data.id > session.profile.data.lastUnlockedLocation;
                string locationName = Localization.Get(session, $"menu_item_location_{location.data.id}");
                if (isLocked)
                {
                    locationName = Emojis.elements[Element.Locked] + Emojis.space + locationName;
                }
                RegisterButton(locationName, () => OnLocationClick(location.data.id));
            }

            var text = Localization.Get(session, "dialog_map_select_location");
            _lastMessage = _lastMessage == null 
                ? await messageSender.SendTextMessage(session.chatId, text, GetMultilineKeyboard())
                : await messageSender.EditTextMessage(session.chatId, _lastMessage.MessageId, text, GetMultilineKeyboard());
        }

        private async Task OnLocationClick(int locationId)
        {
            //TODO
        }

        public override void OnDialogClose()
        {
        }

    }
}
