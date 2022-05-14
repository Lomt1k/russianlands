using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town
{
    internal class TownCharacterDialog : DialogBase
    {
        public TownCharacterDialog(GameSession _session) : base(_session)
        {
            RegisterButton("* Атрибуты *", null);
            RegisterButton("* Инвентарь *", null);
        }

        public async override void Start()
        {
            var text = session.player.GetUnitView();
            await messageSender.SendTextDialog(session.chatId, text, GetMultilineKeyboard());
        }
    }
}
