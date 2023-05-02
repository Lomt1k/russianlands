using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Character;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Shop;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Quests.Characters;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.Scripts.Bot.Dialogs.Quests.MainQuest;

public class EnterNameDialog : DialogBase
{
    private const int minLength = 3;
    private const int maxLength = 16;

    private static readonly NotificationsManager notificationsManager = Services.Get<NotificationsManager>();
    private static readonly ResourceData nickChangePrice = new ResourceData(ResourceId.Diamond, 800);

    private bool _isQuestReplicaStage = false;

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

    /*
     * Тут идёт реплика персонажа, но не через QuestReplicaStage, а костыльно
     * Сделано для того, чтобы пользователь мог либо:
     * - нажать на кнопку "Представиться" в релпике и тогда уже перейти к вводу имени
     * - сразу ввести имя, не нажимая кнопку "представиться"
     * Сделано чтобы избежать отвала игроков, когда они вместо нажатия на кнопку сразу вводят имя и ловят ступор
     */
    public async Task StartFromQuest()
    {
        _isQuestReplicaStage = true;
        var sticker = CharacterType.Vasilisa.GetSticker();
        if (sticker != null)
        {
            await messageSender.SendSticker(session.chatId, sticker, session.cancellationToken).FastAwait();
        }

        var text = Localization.Get(session, "quest_main_vasilisa_encounter_replica");
        var buttonText = GetQuestButtonText(session);
        RegisterButton(buttonText, null);
        await SendDialogMessage(text, GetOneLineKeyboard()).FastAwait();
    }

    private string GetQuestButtonText(GameSession session)
    {
        return Localization.Get(session, "quest_main_vasilisa_encounter_answer");
    }

    public override async Task Start()
    {
        ClearButtons();
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "dialog_entry_name_description"));
        sb.AppendLine(Localization.Get(session, "dialog_entry_name_requirments_1"));
        sb.AppendLine(Localization.Get(session, "dialog_entry_name_requirments_2", minLength, maxLength));

        if (!_isQuestReplicaStage)
        {
            sb.AppendLine();
            if (freeNickChanges > 0)
            {
                sb.AppendLine(Localization.Get(session, "dialog_entry_name_free_nick_changes", freeNickChanges));
            }
            else
            {
                sb.Append(nickChangePrice.GetPriceView(session));
            }
        }

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "dialog_entry_name_header"));

        RegisterButton(currentNickname, null);
        await SendDialogMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    public override async Task HandleMessage(Message message)
    {
        if (_isQuestReplicaStage)
        {
            await HandleMessageInQuestStage(message).FastAwait();
            return;
        }
        await HandleMessageNotInQuest(message).FastAwait();
    }

    public async Task HandleMessageInQuestStage(Message message)
    {
        if (message.Text == null || string.IsNullOrWhiteSpace(message.Text) || message.Text.Equals(GetQuestButtonText(session)))
        {
            await Start().FastAwait();
            return;
        }

        var newNickname = message.Text;
        if (!newNickname.IsCorrectNickname())
        {
            await Start().FastAwait();
            return;
        }

        currentNickname = newNickname;
        await QuestManager.TryInvokeTrigger(session, TriggerType.InvokeFromCode).FastAwait();
        return;
    }

    public async Task HandleMessageNotInQuest(Message message)
    {
        if (message.Text == null || string.IsNullOrWhiteSpace(message.Text))
        {
            await Start().FastAwait();
            return;
        }

        var newNickname = message.Text;
        if (!newNickname.IsCorrectNickname())
        {
            await Start().FastAwait();
            return;
        }

        if (currentNickname.Equals(newNickname))
        {
            await new TownCharacterDialog(session).Start().FastAwait();
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
            RegisterBackButton(() => new TownCharacterDialog(session).Start());
            await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
            return;
        }

        currentNickname = newNickname;
        var notification = Localization.Get(session, "dialog_entry_name_name_changed", newNickname);
        await notificationsManager.ShowNotification(session, notification, () => new TownCharacterDialog(session).Start()).FastAwait();
    }

}
