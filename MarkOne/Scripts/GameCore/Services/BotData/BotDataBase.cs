using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

namespace MarkOne.Scripts.GameCore.Services.BotData;

public class BotDataBase
{
    public string dataBasePath { get; }
    public SQLiteAsyncConnection db { get; private set; }

    public BotDataBase(string botDataPath)
    {
        dataBasePath = Path.Combine(botDataPath, "database.sqlite");
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

}
