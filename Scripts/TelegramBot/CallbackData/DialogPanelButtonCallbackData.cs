
namespace TextGameRPG.Scripts.TelegramBot.CallbackData
{
    public class DialogPanelButtonCallbackData : CallbackDataBase
    {
        public byte panelId { get; set; }
        public int buttonId { get; set; }
    }
}
