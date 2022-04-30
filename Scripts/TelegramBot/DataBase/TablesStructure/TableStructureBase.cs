using System.Collections.Generic;
using System.Text;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure
{
    public struct TableColumn
    {
        public string name;
        public string type;
        public string defaultValue;

        public TableColumn(string _name, string _type, string _defaultValue)
        {
            name = _name;
            type = _type;
            defaultValue = _defaultValue;
        }
    }

    public abstract class TableStructureBase
    {
        public abstract string tableName { get; }
        public abstract TableColumn[] columns { get; }

        public string[] GetAllCommandsToRefreshStructure()
        {
            List<string> commands = new List<string>();
            commands.Add(GetCommandCreateTableIfNotExists());
            commands.AddRange(GetCommandAddColumnIfNotExists());
            return commands.ToArray();
        }

        private string GetCommandCreateTableIfNotExists()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"CREATE TABLE IF NOT EXISTS {tableName}(");

            for (int i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                sb.Append($"{column.name} {column.type} DEFAULT {column.defaultValue}");

                sb.Append(i < columns.Length - 1 ? ", " : ")");
            }

            return sb.ToString();
        }

        private string[] GetCommandAddColumnIfNotExists()
        {
            string[] commands = new string[columns.Length - 1];
            // escape first column: because it contains "PRIMARY KEY"
            for (int i = 0; i < columns.Length - 1; i++)
            {
                var column = columns[i + 1];
                commands[i] = GetCommandAddColumnIfNotExists(column);
            }
            return commands;
        }

        private string GetCommandAddColumnIfNotExists(TableColumn column)
        {
            return "ALTER TABLE " + tableName + $" ADD COLUMN {column.name} {column.type} default {column.defaultValue}";
        }

    }
}
