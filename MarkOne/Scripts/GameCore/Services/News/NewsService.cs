using FastTelegramBot;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Services.News;
public class NewsService : Service
{
    public const int LAST_NEWS_COUNT = 7;

    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();
    private static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    private NewsData[] _lastNews = Array.Empty<NewsData>();
    private CancellationTokenSource _mailingCTS = new();

    public int lastNewsId => _lastNews.Length > 0 ? _lastNews[0].id : -1;
    public NewsData[] lastNews => _lastNews;

    public override async Task OnBotStarted()
    {
        RefreshLastNews();
        _mailingCTS = new CancellationTokenSource();
        Task.Run(() => MailingLogicLoopAsync(_mailingCTS.Token));
    }

    public override Task OnBotStopped()
    {
        _mailingCTS.Cancel();
        return Task.CompletedTask;
    }

    private void RefreshLastNews()
    {
        var db = BotController.dataBase.db;
        var rawNewsDatas = db.Table<RawNewsData>().OrderByDescending(x => x.id).Take(LAST_NEWS_COUNT).ToArray();
        _lastNews = NewsData.Deserialize(rawNewsDatas);
    }

    public NewsData AddNews(string title, string description, DateTime? date = null)
    {
        var db = BotController.dataBase.db;
        var targetDbids = db.Table<ProfileBuildingsData>()
            .Where(profileBuildingsData => profileBuildingsData.townHallLevel >= 2)
            .Select(profileBuildingsData => profileBuildingsData.dbid)
            .ToHashSet();
        var targetTelegramIds = db.Table<ProfileData>()
            .Where(profileData => targetDbids.Contains(profileData.dbid))
            .Select(profileData => profileData.telegram_id)
            .ToList();

        var newsData = new NewsData
        {
            date = date.HasValue ? date.Value : DateTime.UtcNow,
            title = title,
            description = description,
            currentMailsCount = 0,
            totalMailsCount = targetTelegramIds.Count(),
            targetTelegramIds = targetTelegramIds,
        };
        var rawNewsData = new RawNewsData();
        rawNewsData.Fill(newsData);
        
        db.Insert(rawNewsData);
        RefreshLastNews();
        return newsData;
    }

    public void RemoveNews(int newsId)
    {
        var db = BotController.dataBase.db;
        db.Delete<RawNewsData>(newsId);
        RefreshLastNews();
    }

    public NewsData? TreGetNewsById(int newsId)
    {
        var db = BotController.dataBase.db;
        var rawNewsData = db.Table<RawNewsData>().Where(x => x.id == newsId).FirstOrDefault();
        return rawNewsData != null ? NewsData.Deserialize(rawNewsData) : null;
    }

    public bool TryEditNews(int newsId, string title, string description)
    {
        var db = BotController.dataBase.db;
        var newsData = db.Table<RawNewsData>().Where(x => x.id == newsId).FirstOrDefault();
        if (newsData is null)
        {
            return false;
        }
        newsData.title = title;
        newsData.description = description;
        var result = db.Update(newsData);
        if (result > 0)
        {
            RefreshLastNews();
        }
        return result > 0 ? true : false;
    }

    public bool HasNew(GameSession session)
    {
        return lastNewsId > session.profile.data.lastNewsId;
    }

    private async Task MailingLogicLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Task.Run(() => SendMailsAsync(cancellationToken), cancellationToken);
            await Task.Delay(1_000).FastAwait();
        }
    }

    private async Task SendMailsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var newsData = lastNews
                .Where(x => !x.IsMailingCompleted())
                .OrderBy(x => x.id)
                .FirstOrDefault();

            if (newsData is null)
            {
                return;
            }

            var messagesPerSecond = BotController.config.sendingLimits.sendMessagePerSecondLimit;
            var count = Math.Min(messagesPerSecond, newsData.targetTelegramIds.Count);

            var telegramIds = newsData.targetTelegramIds.GetRange(0, count);
            newsData.targetTelegramIds.RemoveRange(0, count);
            newsData.currentMailsCount += count;
            var rawNewsData = new RawNewsData();
            rawNewsData.Fill(newsData);
            var db = BotController.dataBase.db;
            db.InsertOrReplace(rawNewsData);

            var message = new StringBuilder()
                .AppendLine(newsData.title.Bold())
                .AppendLine()
                .AppendLine(newsData.description.Replace("\\n", Environment.NewLine))
                .ToString();

            foreach (var telegramId in telegramIds)
            {
                Task.Run(() => SendMailAsync(telegramId, message, cancellationToken), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Program.logger.Error("Catched exception at RemindersManager.SendRemindersAsync:\n" + ex);
        }
    }

    private async Task SendMailAsync(long telegramId, string message, CancellationToken cancellationToken)
    {
        try
        {
            if (sessionManager.IsSessionExists(telegramId))
            {
                return;
            }
            await messageSender.SendTextMessage(telegramId, message, cancellationToken: cancellationToken).FastAwait();
        }
        catch (TelegramBotException telegramBotEx)
        {
            if (telegramBotEx.ErrorCode != 403)
            {
                Program.logger.Error("Catched exception at NewsService.SendMailAsync:\n" + telegramBotEx);
            }
        }
        catch (Exception ex)
        {
            Program.logger.Error("Catched exception at NewsService.SendMailAsync:\n" + ex);
        }
    }

}
