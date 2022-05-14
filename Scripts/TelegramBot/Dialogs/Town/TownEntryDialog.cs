using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town
{
    public enum TownEntryReason
    {
        StartNewSession,
        EndTutorial
    }

    public class TownEntryDialog : DialogBase
    {
        private TownEntryReason _reason;
        private ReplyKeyboardMarkup _keyboard;

        public TownEntryDialog(GameSession _session, TownEntryReason reason) : base(_session)
        {
            _reason = reason;

            RegisterButton($"{Emojis.townMenu[TownMenu.Map]} "
                + Localization.Get(session, "menu_item_map"),
                null);
            RegisterButton($"{Emojis.townMenu[TownMenu.Buildings]} "
                + Localization.Get(session, "menu_item_buildings"),
                null);
            RegisterButton($"{Emojis.townMenu[TownMenu.Character]} "
                + Localization.Get(session, "menu_item_character"),
                () => new TownCharacterDialog(session).Start());
            RegisterButton($"{Emojis.townMenu[TownMenu.Quests]} "
                + Localization.Get(session, "menu_item_quests"),
                null);
            RegisterButton($"{Emojis.townMenu[TownMenu.Mail]} "
                + Localization.Get(session, "menu_item_mail"),
                null);
            RegisterButton($"{Emojis.townMenu[TownMenu.Options]} "
                + Localization.Get(session, "menu_item_options"),
                null);

            _keyboard = GetKeyboardWithRowSizes(1, 2, 3);
        }

        public async override void Start()
        {
            string header = $"{Emojis.townMenu[TownMenu.Town]} <b>" + Localization.Get(session, "menu_item_town") + "</b>\n\n";
            string text;
            switch (_reason)
            {
                case TownEntryReason.EndTutorial:
                    text = header + Localization.Get(session, "dialog_tutorial_town_entry_text_endTutorial");
                    break;

                case TownEntryReason.StartNewSession:
                default:
                    text = header + Localization.Get(session, "dialog_tutorial_town_entry_text_newSession");
                    break;
            }
            
            await messageSender.SendTextDialog(session.chatId, text, _keyboard);
        }

    }
}
