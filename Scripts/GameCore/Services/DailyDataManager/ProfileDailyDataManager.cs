using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Services.DailyDataManagers;

namespace TextGameRPG.Scripts.GameCore.Services.DailyDataManager
{
    public class ProfileDailyDataManager : Service
    {
        private static readonly ServerDailyDataManager serverDailyDataManager = Services.Get<ServerDailyDataManager>();
        private static readonly SessionManager sessionManager = Services.Get<SessionManager>();

        private static SQLiteAsyncConnection db => BotController.dataBase.db;

        public ProfileDailyDataManager()
        {
            serverDailyDataManager.onStartNewDay += OnStartNewDay;
        }

        private void OnStartNewDay(DateTime oldDate, DateTime newDate)
        {
            ExportAndResetProfileDailyData(oldDate);
        }

        private async void ExportAndResetProfileDailyData(DateTime oldDate)
        {
            var allData = await db.Table<RawProfileDailyData>().ToListAsync().FastAwait();
            ExportStatisticData(oldDate, allData);
            ResetAllProfileDailyData();
        }

        private async void ExportStatisticData(DateTime date, List<RawProfileDailyData> rawProfileDailyDatas)
        {
            var stringDate = date.ToString("yyyy.MM.dd");
            if (rawProfileDailyDatas.Count == 0)
            {
                return;
            }
            var statisticsDir = Path.Combine(BotController.dataPath, "Statistics");
            if (!Directory.Exists(statisticsDir))
            {
                Directory.CreateDirectory(statisticsDir);
            }
            var statDBPath = Path.Combine(statisticsDir, $"stats_{stringDate}.sqlite");
            var statsDB = new SQLiteAsyncConnection(statDBPath, storeDateTimeAsTicks: false);
            await statsDB.CreateTableAsync<ProfileDailyStatData>().FastAwait();

            var statDatas = new List<ProfileDailyStatData>();
            foreach (var rawProfileDailyData in rawProfileDailyDatas)
            {
                var statData = ProfileDailyStatData.Create(rawProfileDailyData, date, stringDate);
                statDatas.Add(statData);
            }
            await statsDB.InsertAllAsync(statDatas).FastAwait();
            await statsDB.CloseAsync().FastAwait();
            Program.logger.Info($"Daily statistics exported to file: stats_{stringDate}.sqlite");
        }

        private async void ResetAllProfileDailyData()
        {
            await db.DeleteAllAsync<ProfileDailyData>().FastAwait();
            var allSessions = sessionManager.GetAllSessions();
            foreach (var session in allSessions)
            {
                session.profile?.ResetDailyDataWithoutSave();
            }
        }
    }
}
