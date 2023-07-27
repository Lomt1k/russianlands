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
        RefreshLastNews();
    }

    private void RefreshLastNews()
    {
        var db = BotController.dataBase.db;
        _lastNews = db.Table<NewsData>().OrderByDescending(x => x.id).Take(LAST_NEWS_COUNT).ToArray();
    }

    public NewsData AddNews(string title, string description, DateTime? date = null)
    {
        var newsData = new NewsData
        {
            date = date.HasValue ? date.Value : DateTime.UtcNow,
            title = title,
            description = description,
        };
        var db = BotController.dataBase.db;
        db.Insert(newsData);
        RefreshLastNews();
        return newsData;
    }

    public void RemoveNews(int newsId)
    {
        var db = BotController.dataBase.db;
        db.Delete<NewsData>(newsId);
        RefreshLastNews();
    }

    public NewsData? TreGetNewsById(int newsId)
    {
        var db = BotController.dataBase.db;
        return db.Table<NewsData>().Where(x => x.id == newsId).FirstOrDefault();
    }

    public bool TryEditNews(int newsId, string title, string description)
    {
        var db = BotController.dataBase.db;
        var newsData = db.Table<NewsData>().Where(x => x.id == newsId).FirstOrDefault();
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

}
