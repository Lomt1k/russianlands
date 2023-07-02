using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.News;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.News;
internal class NewsDialogPanel : DialogPanelBase
{
    private readonly NewsService newsService = ServiceLocator.Get<NewsService>();

    public NewsDialogPanel(DialogWithPanel _dialog) : base(_dialog)
    {
    }

    public override async Task Start()
    {
        await ShowNewsList().FastAwait();
    }

    private async Task ShowNewsList()
    {
        ClearButtons();
        MarkAllNewsAsReaden();
        var lastNews = newsService.lastNews;
        if (lastNews.Length < 1)
        {
            var message = Localization.Get(session, "dialog_news_no_news");
            await SendPanelMessage(message, GetOneLineKeyboard()).FastAwait();
            return;
        }

        var text = Localization.Get(session, "dialog_news_last_news");
        foreach (var news in lastNews)
        {
            RegisterButton(news.title, () => ShowNews(news.id));
        }
        await SendPanelMessage(text, GetMultilineKeyboard()).FastAwait();
    }

    private void MarkAllNewsAsReaden()
    {
        session.profile.data.lastNewsId = newsService.lastNewsId;
    }

    private async Task ShowNews(int newsId)
    {
        var news = newsService.lastNews.Where(x => x.id == newsId).FirstOrDefault();
        if (news is null)
        {
            await ShowNewsList().FastAwait();
            return;
        }

        var publicationDate = Localization.Get(session, "dialog_news_publication_date", news.date.ToShortDateString());
        var description = news.description.Replace("\\n", System.Environment.NewLine);
        var text = new StringBuilder()
            .AppendLine(news.title.Bold())
            .AppendLine()
            .AppendLine(description)
            .AppendLine()
            .Append(publicationDate);

        ClearButtons();
        RegisterBackButton(ShowNewsList);

        await SendPanelMessage(text, GetOneLineKeyboard()).FastAwait();
    }


}
