﻿using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.Bot;

namespace MarkOne.Scripts.GameCore.Services.BotData;

public class BotDataBase
{
    private SQLiteConnection _dbConnection;
    private readonly object _dbLock = new();

    public string botDataPath { get; }
    public string dataBasePath { get; }
    public SQLiteConnection db
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

    public bool Connect()
    {
        try
        {
            db = new SQLiteConnection(dataBasePath);
            CreateTables();
            Program.logger.Info("Successfully connected to database");
            return true;
        }
        catch (Exception ex)
        {
            Program.logger.Fatal(ex);
        }
        return false;
    }

    private void CreateTables()
    {
        db.CreateTable<ProfileData>();
        db.CreateTable<RawProfileDynamicData>();
        db.CreateTable<ProfileBuildingsData>();
        db.CreateTable<DailyReminderData>();
        db.CreateTable<ServerDailyData>();
        db.CreateTable<RawProfileDailyData>();
        db.CreateTable<RawNewsData>();
        db.CreateTable<PaymentData>();
    }

    public void Close()
    {
        try
        {
            if (db != null)
            {
                db.Close();
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
                    db.Backup(Path.Combine(backupsFolder, backupName));
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
