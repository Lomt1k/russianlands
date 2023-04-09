using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Shop
{
    public class ShopDialog : DialogBase
    {
        public ShopDialog(GameSession _session) : base(_session)
        {
            RegisterTownButton(isDoubleBack: false);
        }

        public override async Task Start()
        {
            var text = Emojis.ButtonShop + Localization.Get(session, "menu_item_shop").Bold()
                + "\n\nВ разработке...";
            await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
        }
    }
}
