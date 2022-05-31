using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    internal class AttributesDialog : DialogBase
    {
        public AttributesDialog(GameSession _session) : base(_session)
        {
            RegisterButton($"{Emojis.elements[Element.Back]} " + Localization.Get(session, "menu_item_back_button"),
                () => new TownCharacterDialog(session).Start());
            
            RegisterPanel(new AttributesDialogPanel(this, 0));
        }

        public override async void Start()
        {
            string header = $"{Emojis.menuItems[MenuItem.Attributes]} <b>{Localization.Get(session, "menu_item_attributes")}</b>";
            await messageSender.SendTextDialog(session.chatId, header, GetOneLineKeyboard());
            await SendPanelsAsync();
        }
    }
}
