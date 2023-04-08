using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.DataBase.TablesStructure;
using System.Collections.Generic;

namespace TextGameRPG.Scripts.Bot.DataBase
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
                { Table.Profiles, new ProfilesDataTable(this) },
                { Table.ProfilesDynamic, new ProfilesDynamicDataTable(this) },
                { Table.ProfileBuildings, new ProfileBuildingsDataTable(this) },
            };
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                connection = new SQLiteConnection("Data Source=" + dataBasePath);
                await connection.OpenAsync().FastAwait();
                await RefreshTableStructure().FastAwait();
                Program.logger.Info("Successfully connected to database");
                return connection.State == ConnectionState.Open;
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
                if (connection != null && connection.State != ConnectionState.Closed)
                {
                    await connection.CloseAsync().FastAwait();
                    Program.logger.Info("Closed connection to database");
                }
            }
            catch (Exception ex)
            {
                Program.logger.Error(ex);
            }
        }

        private async Task<int> RefreshTableStructure()
        {
            foreach (var table in _tables.Values)
            {
                await ExecuteQueryAsync(table.GetAllCommandsToRefreshStructure()).FastAwait();
            }
            return 0;
        }

        public async Task<int> ExecuteQueryAsync(string[] querries)
        {
            foreach (var query in querries)
            {
                await ExecuteQueryAsync(query).FastAwait();
            }
            return 0;
        }

        public async Task<SQLiteCommand?> ExecuteQueryAsync(string commandText)
        {
            try
            {
                var command = new SQLiteCommand(commandText, connection);
                await command.ExecuteNonQueryAsync().FastAwait();
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
