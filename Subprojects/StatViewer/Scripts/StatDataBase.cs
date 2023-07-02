using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using SQLite;
using System.IO;
using System.Threading.Tasks;

namespace StatViewer.Scripts;
internal static class StatDataBase
{
    public const string statisticsFolder = "statistics";
    public const string dataBasePath = "cache.sqlite";

    public static SQLiteAsyncConnection db { get; private set; }

    public static async Task ConnectAsync()
    {
        db = new SQLiteAsyncConnection(dataBasePath);
        await db.CreateTableAsync<ManifestData>();
        await db.CreateTableAsync<ProfileDailyStatData>();
    }

    public static async Task RefreshCache()
    {
        if (!Directory.Exists(statisticsFolder)) 
        {
            Directory.CreateDirectory(statisticsFolder);
        }
        var allFiles = Directory.EnumerateFiles(statisticsFolder, "*.sqlite");
        foreach (var file in allFiles)
        {
            var fileInfo = new FileInfo(file);
            await RefreshFileIfNeed(fileInfo);
        }
    }

    private static async Task RefreshFileIfNeed(FileInfo fileInfo)
    {
        var manifestData = await db.Table<ManifestData>().Where(x => x.fileName == fileInfo.Name).FirstOrDefaultAsync();
        if (manifestData is not null)
        {
            if (manifestData.fileSize == fileInfo.Length)
            {
                return;
            }
        }

        manifestData = new ManifestData
        {
            fileName = fileInfo.Name,
            fileSize = fileInfo.Length,
        };

        var loadDb = new SQLiteAsyncConnection(fileInfo.FullName);
        var allData = await loadDb.Table<ProfileDailyStatData>().ToArrayAsync();
        await db.InsertOrReplaceAsync(allData);
        await db.InsertOrReplaceAsync(manifestData);
    }

}
