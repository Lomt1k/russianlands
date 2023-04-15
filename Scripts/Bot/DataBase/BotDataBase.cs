using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.Bot.DataBase
{
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
                await db.CreateTableAsync<ProfileData>().FastAwait();
                await db.CreateTableAsync<ProfileDynamicData>().FastAwait();
                await db.CreateTableAsync<ProfileBuildingsData>().FastAwait();
                Program.logger.Info("Successfully connected to database");
                return true;
            }
            catch (Exception ex)
            {
                Program.logger.Fatal(ex);
            }
            return false;
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
}
