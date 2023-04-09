
namespace TextGameRPG.Scripts.Bot.CallbackData
{
    public class DialogPanelButtonCallbackData : CallbackDataBase
    {
        /// <summary>
        /// Нужен для того, чтобы кнопка с предыдущей сессии не работала в текущей сессии
        /// </summary>
        public long sessionTime { get; set; }
        public int buttonId { get; set; }
    }
}
