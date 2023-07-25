using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.Bot;

namespace MarkOne.Scripts.GameCore.Services.BotData;

public class BotDataBase
{
    private SQLiteAsyncConnection _dbConnection;
    private readonly object _dbLock = new();

    public string botDataPath { get; }
    public string dataBasePath { get; }
    public SQLiteAsyncConnection db
    {
        get
        {
            lock (_dbLock)
            {
                return _dbConnection;
            }
        }
        set
        {
            lock (_dbLock)
            {
                _dbConnection = value;
            }
        }
    }


    public BotDataBase(string _botDataPath)
    {
        botDataPath = _botDataPath;
        dataBasePath = Path.Combine(botDataPath, "database.sqlite");
        Task.Run(CreateBackupLoop);
    }

    public async Task<bool> ConnectAsync()
    {
        try
        {
            db = new SQLiteAsyncConnection(dataBasePath);
            await CreateTables().FastAwait();
            Program.logger.Info("Successfully connected to database");
            return true;
        }
        catch (Exception ex)
        {
            Program.logger.Fatal(ex);
        }
        return false;
    }

    private async Task CreateTables()
    {
        await db.CreateTableAsync<ProfileData>().FastAwait();
        await db.CreateTableAsync<RawProfileDynamicData>().FastAwait();
        await db.CreateTableAsync<ProfileBuildingsData>().FastAwait();
        await db.CreateTableAsync<DailyReminderData>().FastAwait();
        await db.CreateTableAsync<ServerDailyData>().FastAwait();
        await db.CreateTableAsync<RawProfileDailyData>().FastAwait();
        await db.CreateTableAsync<NewsData>().FastAwait();
        await db.CreateTableAsync<PaymentData>().FastAwait();
    }

    public async Task CloseAsync()
    {
        try
        {
            if (db != null)
            {
                await db.CloseAsync().FastAwait();
                Program.logger.Info("Closed connection to database");
            }
        }
        catch (Exception ex)
        {
            Program.logger.Error(ex);
        }
    }

    private async Task CreateBackupLoop()
    {
        while (true)
        {
            try
            {
                await Task.Delay(5_000);
                var now = DateTime.UtcNow;
                if (now.Hour == BotController.config.hourForCreateBackup && now.Minute == 0)
                {
                    var backupsFolder = Path.Combine(botDataPath, "Backups");
                    if (!Directory.Exists(backupsFolder))
                    {
                        Directory.CreateDirectory(backupsFolder);
                    }
                    var backupName = $"database_backup_{now:yyyy.MM.dd}.sqlite";
                    Program.logger.Info("Try to create backup for database...");
                    await db.BackupAsync(Path.Combine(backupsFolder, backupName));
                    Program.logger.Info($"Backup for database created: {backupName}");
                    await Task.Delay(60_000);
                }
            }
            catch (Exception)
            {
                //ignored
                await Task.Delay(60_000);
            }
        }
    }

}
