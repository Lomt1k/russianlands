using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;

namespace TextGameRPG.Scripts.TelegramBot.DataBase
{
    public class BotDataBase
    {
        private SQLiteConnection _connection;

        public string dataBasePath { get; }
        public ConnectionState connectionState => _connection.State;


        public BotDataBase(string botDataPath)
        {
            dataBasePath = Path.Combine(botDataPath, "database.sqlite");
        }

        public async Task<bool> Connect()
        {
            Program.logger.Info("Connecting to bot database...");
            try
            {
                _connection = new SQLiteConnection("Data Source=" + dataBasePath);
                await _connection.OpenAsync();
                await RefreshTableStructure();
                Program.logger.Info("Successfully connected to database");
                return _connection.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                Program.logger.Fatal(ex.Message);
            }
            return false;
        }

        private async Task<int> RefreshTableStructure()
        {
            await ExecuteQueryAsync(new ProfilesTable().GetAllCommandsToRefreshStructure());

            return 0;
        }

        private async Task<int> ExecuteQueryAsync(string[] querries)
        {
            foreach (var query in querries)
            {
                await ExecuteQueryAsync(query);
            }
            return 0;
        }

        private async Task<int> ExecuteQueryAsync(string commandText)
        {
            try
            {
                var command = new SQLiteCommand(commandText, _connection);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("duplicate column name"))
                    return 0;

                Program.logger.Error(ex.Message);
                return 0;
            }
        }

    }
}
