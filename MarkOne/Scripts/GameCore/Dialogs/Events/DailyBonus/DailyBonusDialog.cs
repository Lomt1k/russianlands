using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Events.DailyBonus;
public class DailyBonusDialog : DialogBase
{
    public DailyBonusDialog(GameSession _session) : base(_session)
    {
    }

    public override async Task Start()
    {
        var sb = new StringBuilder()
            .AppendLine(Emojis.ButtonDailyBonus + Localization.Get(session, "dialog_daily_bonus_event_header").Bold())
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_daily_bonus_description"))
            .AppendLine();
        AppendRewardsList(sb);

        ClearButtons();
        RegisterBackButton(Localization.Get(session, "menu_item_events") + Emojis.ButtonEvents, () => new EventsDialog(session).Start());
        RegisterTownButton(isDoubleBack: true);

        await SendDialogMessage(sb, GetOneLineKeyboard()).FastAwait();
    }

    private void AppendRewardsList(StringBuilder sb)
    {
        sb.AppendLine(Localization.Get(session, "reward_list_header"));
        var dailyBonusDatas = gameDataHolder.dailyBonuses.GetAllData();
        foreach (var data in dailyBonusDatas)
        {
            sb.AppendLine(data.GetView(session));
        }
    }

    public static async Task ClaimNewRewardAndShowNotification(GameSession session, Func<Task> onContinue)
    {
        var sb = new StringBuilder()
            .AppendLine(Localization.Get(session, "dialog_daily_bonus_reward_claimed"))
            .AppendLine()
            .AppendLine(Localization.Get(session, "battle_result_header_rewards"));

        var rewardId = (byte)(session.profile.data.lastDailyBonusId + 1);
        var rewards = gameDataHolder.dailyBonuses[rewardId].rewards;

        foreach (var reward in rewards)
        {
            var addedReward = await reward.AddReward(session).FastAwait();
            if (!string.IsNullOrEmpty(addedReward))
            {
                sb.AppendLine(addedReward);
            }
        }
        session.profile.data.lastDailyBonusReceivedTime = DateTime.UtcNow;
        session.profile.data.lastDailyBonusId = rewardId;

        if (IsEventAvailable(session))
        {
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_daily_bonus_reward_claimed_footer"));
        }

        await notificationsManager.ShowNotification(session, sb, onContinue).FastAwait();
    }

    public static bool IsEventAvailable(GameSession session)
    {
        return IsEventAvailable(session.profile.data);
    }

    public static bool IsEventAvailable(ProfileData profileData)
    {
        return profileData.lastDailyBonusId < gameDataHolder.dailyBonuses.count;
    }

    public static bool IsNewRewardAvailable(GameSession session)
    {
        if (!IsEventAvailable(session))
        {
            return false;
        }

        var now = DateTime.UtcNow.Date;
        var lastTime = session.profile.data.lastDailyBonusReceivedTime.Date;
        return (now - lastTime).TotalDays > 1;
    }

}
