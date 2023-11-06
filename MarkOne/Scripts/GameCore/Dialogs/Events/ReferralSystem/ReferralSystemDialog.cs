using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Events.ReferralSystem;
public class ReferralSystemDialog : DialogBase
{
    public ReferralSystemDialog(GameSession _session) : base(_session)
    {
    }

    public override async Task Start()
    {
        var link = $"https://t.me/{BotController.botname.ToLower()}?start=ref{session.actualUser.Id}";
        var sb = new StringBuilder()
            .AppendLine(Emojis.AvatarSmirkCat + Localization.Get(session, "dialog_referral_system_header").Bold())
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_referral_system_description", link))
            .AppendLine();
        AppendRewardsList(sb);

        ClearButtons();
        RegisterBackButton(Localization.Get(session, "menu_item_events") + Emojis.ButtonEvents, () => new EventsDialog(session).Start());
        RegisterTownButton(isDoubleBack: true);
        MarkAsViewed(session);

        await SendDialogMessage(sb, GetOneLineKeyboard()).FastAwait();
    }

    private void AppendRewardsList(StringBuilder sb)
    {
        sb.AppendLine(Localization.Get(session, "reward_list_header"));
        var bonusDatas = gameDataHolder.referralBonuses.GetAllData();
        foreach (var data in bonusDatas)
        {
            sb.AppendLine(data.GetView(session));
        }
    }

    public static async Task ClaimNewRewardAndShowNotification(GameSession session, Func<Task> onContinue)
    {
        var sb = new StringBuilder()
            .AppendLine(Emojis.AvatarSmirkCat + Localization.Get(session, "dialog_referral_system_header").Bold())
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_referral_system_reward_claimed"))
            .AppendLine()
            .AppendLine(Localization.Get(session, "battle_result_header_rewards"));

        while (IsNewRewardAvailable(session))
        {
            var rewardId = (byte)(session.profile.data.lastReferralBonusId + 1);
            var rewards = gameDataHolder.referralBonuses[rewardId].rewards;

            foreach (var reward in rewards)
            {
                var addedReward = await reward.AddReward(session).FastAwait();
                if (!string.IsNullOrEmpty(addedReward))
                {
                    sb.AppendLine(addedReward);
                }
            }
            session.profile.data.lastReferralBonusId = rewardId;
        }        

        if (IsEventAvailable(session))
        {
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_referral_system_reward_claimed_footer"));
        }

        await notificationsManager.ShowNotification(session, sb, onContinue).FastAwait();
    }

    public static bool IsEventAvailable(GameSession session)
    {
        var firstLocationCompleted = session.profile.dynamicData.quests.IsCompleted(GameCore.Quests.QuestId.Loc_01);
        var allRewardsClaimed = session.profile.data.lastReferralBonusId >= gameDataHolder.referralBonuses.count;
        return firstLocationCompleted && !allRewardsClaimed;
    }

    public static bool IsNewRewardAvailable(GameSession session)
    {
        var profileData = session.profile.data;
        return IsEventAvailable(session) && profileData.lastReferralBonusId < profileData.totalReferralsCount;
    }

    public static bool HasNew(GameSession session)
    {
        return session.profile.data.totalReferralsCount == -1;
    }

    public static void MarkAsViewed(GameSession session)
    {
        if (session.profile.data.totalReferralsCount == -1)
        {
            session.profile.data.totalReferralsCount = 0;
        }
    }

}
