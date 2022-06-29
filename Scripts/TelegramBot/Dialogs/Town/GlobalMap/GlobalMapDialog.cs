﻿using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.GlobalMap
{
    public class GlobalMapDialog : DialogBase
    {
        public GlobalMapDialog(GameSession _session) : base(_session)
        {
            var dialogPanel = new GlobalMapDialogPanel(this, 0);
            RegisterPanel(dialogPanel);

            RegisterButton($"{Emojis.locations[MapLocation.Arena]} " + Localization.Get(session, "menu_item_arena"),
                null);
            RegisterButton($"{Emojis.elements[Element.Back]} " + Localization.Get(session, "menu_item_back_button"),
                () => new TownEntryDialog(session, TownEntryReason.BackFromInnerDialog).Start());
        }

        public override async Task Start()
        {
            var header = $"{Emojis.menuItems[MenuItem.Map]} <b>{Localization.Get(session, "menu_item_map")}</b>";
            await messageSender.SendTextDialog(session.chatId, header, GetMultilineKeyboard());
            await SendPanelsAsync();
        }
    }
}
