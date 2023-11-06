using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Quests.StageActions;

[JsonObject]
public class CountReferralAction : StageActionBase
{
    private static SessionManager sessionManager = ServiceLocator.Get<SessionManager>();

    public override async Task Execute(GameSession session)
    {
        var regInfo = session.profile.data.regInfo.ToLowerInvariant();
        if (!regInfo.StartsWith("ref"))
        {
            return;
        }
        var unparsedId = regInfo.Replace("ref", string.Empty);
        if (!long.TryParse(unparsedId, out var telegramId))
        {
            return;
        }

        var inviterSession = sessionManager.GetSessionIfExists(telegramId);
        if (inviterSession is null)
        {
            var db = BotController.dataBase.db;
            var profileData = db.Table<ProfileData>()
                .Where(x => x.telegram_id == telegramId)
                .FirstOrDefault();

            if (profileData is null)
            {
                return;
            }

            profileData.totalReferralsCount++;
            db.Update(profileData);
            Program.logger.Info($"User {session.actualUser} is counted as a referral of user (ID {telegramId})");
        }
        else
        {
            inviterSession.profile.data.totalReferralsCount++;
            await inviterSession.profile.SaveProfile().FastAwait();
            Program.logger.Info($"User {session.actualUser} is counted as a referral of user {inviterSession.actualUser}");
        }

        await session.profile.SaveProfile().FastAwait();
    }
}
