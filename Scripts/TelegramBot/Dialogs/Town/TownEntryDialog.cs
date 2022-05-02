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

            RegisterButton("Карта", null);
            RegisterButton("Жители", null);
            RegisterButton("Инвентарь", null);
            RegisterButton("Задания", null);
            RegisterButton("Почта", null);
            RegisterButton("Настройки", null);

            _keyboard = GetKeyboardWithRowSizes(1, 2, 3);
        }

        public async override void Start()
        {
            string header = $"{Emojis.locations[Location.Town]} <b>" + Localization.Get(session, "location_town") + "</b>\n\n";
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
