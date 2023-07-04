using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using SQLite;
using System.IO;
using System.Threading.Tasks;

namespace StatViewer.Scripts;
internal static class StatDataBase
{
    public const string statisticsFolder = "statistics";
    public static readonly string cacheFolder = Path.Combine(statisticsFolder, "cache");
    public static readonly string dataBasePath = Path.Combine(cacheFolder, "cache.sqlite");

    public static SQLiteAsyncConnection db { get; private set; }

    public static async Task ConnectAsync()
    {
        if (!Directory.Exists(cacheFolder))
        {
            Directory.CreateDirectory(cacheFolder);
        }
        db = new SQLiteAsyncConnection(dataBasePath);
        await db.CreateTableAsync<ManifestData>();
        await db.CreateTableAsync<ProfileDailyStatData>();
    }

    public static async Task RefreshCache()
    {
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
