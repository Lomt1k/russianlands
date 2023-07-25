using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Dialogs.Town.Character;
using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using FastTelegramBot.DataTypes;

namespace MarkOne.Scripts.GameCore.Dialogs.Quests.MainQuest;

public class EnterNameDialog : DialogBase
{
    private const int minLength = 3;
    private const int maxLength = 16;

    private static readonly ResourceData nickChangePrice = new ResourceData(ResourceId.Diamond, 800);

    public byte freeNickChanges
    {
        get => session.profile.data.freeNickChanges;
        set => session.profile.data.freeNickChanges = value;
    }

    public string currentNickname
    {
        get => session.profile.data.nickname;
        set => session.profile.data.nickname = value;
    }

    public EnterNameDialog(GameSession _session) : base(_session)
    {
    }

    public override async Task Start()
    {
        ClearButtons();
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "dialog_entry_name_description"));
        sb.AppendLine(Localization.Get(session, "dialog_entry_name_requirments_1"));
        sb.AppendLine(Localization.Get(session, "dialog_entry_name_requirments_2", minLength, maxLength));

        sb.AppendLine();
        if (freeNickChanges > 0)
        {
            sb.AppendLine(Localization.Get(session, "dialog_entry_name_free_nick_changes", freeNickChanges));
        }
        else
        {
            sb.Append(nickChangePrice.GetPriceView(session));
        }

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "dialog_entry_name_header"));

        RegisterButton(currentNickname, null);
        await SendDialogMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    public override async Task HandleMessage(Message message)
    {
        var newNickname = message.Text;
        if (newNickname is null || !newNickname.IsCorrectNickname())
        {
            await Start().FastAwait();
            return;
        }

        if (currentNickname.Equals(newNickname))
        {
            await new CharacterDialog(session).Start().FastAwait();
            return;
        }

        if (freeNickChanges > 0)
        {
            freeNickChanges--;
        }
        else if (!session.player.resources.TryPurchase(nickChangePrice))
        {
            ClearButtons();
            var text = Localization.Get(session, "resource_not_enough_diamonds", Emojis.SmileSad);
            RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"), () => new ShopDialog(session).Start());
            RegisterBackButton(() => new CharacterDialog(session).Start());
            await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
            return;
        }

        Program.logger.Info($"CHANGE NICK | User {session.actualUser} changed nick from '{currentNickname}' to '{newNickname}'");
        currentNickname = newNickname;
        var notification = Localization.Get(session, "dialog_entry_name_name_changed", newNickname);
        await notificationsManager.ShowNotification(session, notification, () => new CharacterDialog(session).Start()).FastAwait();
    }

}
