using System;
using System.Threading;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Services
{
    public class RemindersManager : Service
    {
        private static readonly MessageSender messageSender = Services.Get<MessageSender>();

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public override Task OnBotStarted()
        {
            _cts = new CancellationTokenSource();
            SendingLogicAsync(_cts.Token);
            return Task.CompletedTask;
        }

        public override Task OnBotStopped()
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        public async Task ScheduleReminder(GameSession session)
        {
            try
            {
                var profileData = session.profile.data;
                var reminderData = new ReminderData()
                {
                    dbid = profileData.dbid,
                    userId = profileData.telegram_id,
                    timeToSend = DateTime.UtcNow.AddSeconds(30),
                    languageCode = profileData.language
                };
                await BotController.dataBase.db.InsertOrReplaceAsync(reminderData).FastAwait();
            }
            catch (Exception ex) 
            {
                Program.logger.Error("Catched exception when try schedule reminder\n" + ex);
            }
        }

        private async void SendingLogicAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                SendRemindersAsync(cancellationToken);
                await Task.Delay(1_000).FastAwait(); // TODO: change 1_000 to 60_000 !
            }
        }

        private async void SendRemindersAsync(CancellationToken cancellationToken)
        {
            try
            {
                var db = BotController.dataBase.db;
                var now = DateTime.UtcNow;
                var reminderDatas = await db.Table<ReminderData>().Where(x => x.timeToSend < now).ToArrayAsync().FastAwait();
                if (reminderDatas == null || reminderDatas.Length == 0)
                {
                    return;
                }
                foreach (var reminderData in reminderDatas)
                {
                    SendReminderAsync(reminderData, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Program.logger.Error("Catched exception at RemindersManager.SendRemindersAsync:\n" + ex);
            }
        }

        private async void SendReminderAsync(ReminderData reminderData, CancellationToken cancellationToken)
        {
            try
            {
                var text = "Вернись в игру, чел!";
                await messageSender.SendTextDialog(reminderData.userId, text, cancellationToken: cancellationToken).FastAwait();
                await BotController.dataBase.db.DeleteAsync(reminderData).FastAwait();
            }
            catch (Exception ex)
            {
                Program.logger.Error("Catched exception at RemindersManager.SendReminderAsync:\n" + ex);
            }
        }

    }
}
