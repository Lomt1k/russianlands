using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;
using System;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Services.News;
public class NewsService : Service
{
    public const int LAST_NEWS_COUNT = 7;

    private NewsData[] _lastNews = Array.Empty<NewsData>();

    public int lastNewsId => _lastNews.Length > 0 ? _lastNews[0].id : -1;
    public NewsData[] lastNews => _lastNews;

    public override async Task OnBotStarted()
    {
        await RefreshLastNews().FastAwait();
    }

    private async Task RefreshLastNews()
    {
        var db = BotController.dataBase.db;
        _lastNews = await db.Table<NewsData>().OrderByDescending(x => x.id).Take(LAST_NEWS_COUNT).ToArrayAsync().FastAwait();
    }

    public async Task AddNews(string title, string description, DateTime? date = null)
    {
        var newsData = new NewsData
        {
            date = date.HasValue ? date.Value : DateTime.UtcNow,
            title = title,
            description = description,
        };
        var db = BotController.dataBase.db;
        await db.InsertAsync(newsData).FastAwait();
        await RefreshLastNews();
    }

    public async Task RemoveNews(int newsId)
    {
        var db = BotController.dataBase.db;
        await db.DeleteAsync<NewsData>(newsId).FastAwait();
        await RefreshLastNews();
    }

    public bool HasNew(GameSession session)
    {
        return lastNewsId > session.profile.data.lastNewsId;
    }

}
