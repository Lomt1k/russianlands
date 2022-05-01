using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;
using System.Collections.Generic;

namespace TextGameRPG.Scripts.TelegramBot.DataBase
{
    public class BotDataBase
    {        
        private Dictionary<Table,DataTableBase> _tables;

        public string dataBasePath { get; }
        public SQLiteConnection connection { get; private set; }
        public ConnectionState connectionState => connection.State;

        public DataTableBase this[Table index] => _tables[index];

        public BotDataBase(string botDataPath)
        {
            dataBasePath = Path.Combine(botDataPath, "database.sqlite");
            _tables = new Dictionary<Table, DataTableBase>()
            {
                { Table.Profiles, new ProfilesTable(this) }
            };
        }

        public async Task<bool> Connect()
        {
            Program.logger.Info("Connecting to bot database...");
            try
            {
                connection = new SQLiteConnection("Data Source=" + dataBasePath);
                await connection.OpenAsync();
                await RefreshTableStructure();
                Program.logger.Info("Successfully connected to database");
                return connection.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                Program.logger.Fatal(ex);
            }
            return false;
        }

        private async Task<int> RefreshTableStructure()
        {
            foreach (var table in _tables.Values)
            {
                await ExecuteQueryAsync(table.GetAllCommandsToRefreshStructure());
            }
            return 0;
        }

        public async Task<int> ExecuteQueryAsync(string[] querries)
        {
            foreach (var query in querries)
            {
                await ExecuteQueryAsync(query);
            }
            return 0;
        }

        public async Task<SQLiteCommand?> ExecuteQueryAsync(string commandText)
        {
            try
            {
                var command = new SQLiteCommand(commandText, connection);
                await command.ExecuteNonQueryAsync();
                return command;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("duplicate column name"))
                    return null;

                Program.logger.Error(ex);
                return null;
            }
        }

    }
}
