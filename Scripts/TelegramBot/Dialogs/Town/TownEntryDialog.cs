﻿using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town
{
    public enum TownEntryReason
    {
        StartNewSession,
        EndTutorial,
        BackFromInnerDialog,
        FromQuestAction
    }

    public class TownEntryDialog : DialogBase
    {
        private TownEntryReason _reason;
        private ReplyKeyboardMarkup _keyboard;

        public TownEntryDialog(GameSession _session, TownEntryReason reason) : base(_session)
        {
            _reason = reason;

            RegisterButton($"{Emojis.menuItems[MenuItem.Map]} " + Localization.Get(session, "menu_item_map"),
                () => new GlobalMap.GlobalMapDialog(session).Start());

            var buildingsLocalization = $"{Emojis.menuItems[MenuItem.Buildings]} " + Localization.Get(session, "menu_item_buildings")
                + (session.player.buildings.HasImportantUpdates() ? $"{Emojis.elements[Element.WarningRed]}" : string.Empty);
            RegisterButton(buildingsLocalization, () => new Buildings.BuildingsDialog(session).Start());

            RegisterButton($"{Emojis.menuItems[MenuItem.Character]} " + Localization.Get(session, "menu_item_character"),
                () => new Character.TownCharacterDialog(session).Start());
            RegisterButton($"{Emojis.menuItems[MenuItem.Quests]} " + Localization.Get(session, "menu_item_quests"),
                null);
            RegisterButton($"{Emojis.menuItems[MenuItem.Mail]} " + Localization.Get(session, "menu_item_mail"),
                null);
            RegisterButton($"{Emojis.menuItems[MenuItem.Shop]} " + Localization.Get(session, "menu_item_shop"),
                () => new Shop.ShopDialog(session).Start());

            _keyboard = GetKeyboardWithRowSizes(1, 2, 3);
        }

        public override async Task Start()
        {
            string header = $"{Emojis.menuItems[MenuItem.Town]} <b>" + Localization.Get(session, "menu_item_town") + "</b>\n";
            string resources = session.player.resources.GetGeneralResourcesView();
            string text;
            switch (_reason)
            {
                case TownEntryReason.EndTutorial:
                    text = header + "\n\n" + Localization.Get(session, "dialog_tutorial_town_entry_text_endTutorial");
                    break;
                case TownEntryReason.BackFromInnerDialog:
                    text = header + "\n" + resources + "\n\n" + Localization.Get(session, "dialog_tutorial_town_entry_text_backFromInnerDialog");
                    break;
                case TownEntryReason.FromQuestAction:
                    text = header + "\n" + resources + "\n\n" + Localization.Get(session, "dialog_tutorial_town_entry_text_backFromInnerDialog");
                    break;

                case TownEntryReason.StartNewSession:
                default:
                    text = header + "\n" + Localization.Get(session, "dialog_tutorial_town_entry_text_newSession") + "\n\n" + resources;
                    break;
            }
            
            await messageSender.SendTextDialog(session.chatId, text, _keyboard);
        }

    }
}
