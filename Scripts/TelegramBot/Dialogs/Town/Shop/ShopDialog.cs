﻿using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Shop
{
    public class ShopDialog : DialogBase
    {
        public ShopDialog(GameSession _session) : base(_session)
        {
            RegisterButton($"{Emojis.elements[Element.Back]} " + Localization.Get(session, "menu_item_back_button"),
                () => new TownEntryDialog(session, TownEntryReason.BackFromInnerDialog).Start());
        }

        public override async Task Start()
        {
            var text = $"<b>{Emojis.menuItems[MenuItem.Shop]} {Localization.Get(session, "menu_item_shop")}</b>"
                + "\n\nВ разработке...";
            await messageSender.SendTextDialog(session.chatId, text, GetMultilineKeyboard());
        }
    }
}
