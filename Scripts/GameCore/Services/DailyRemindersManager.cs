using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

namespace MarkOne.Scripts.GameCore.Services;

public class DailyRemindersManager : Service
{
    private static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    private static readonly string[] localizationKeys =
    {
        "daily_reminder_variant_0",
        "daily_reminder_variant_1",
        "daily_reminder_variant_2",
        "daily_reminder_variant_3",
        "daily_reminder_variant_4",
        "daily_reminder_variant_5",
    };

    private CancellationTokenSource _cts = new CancellationTokenSource();
    private bool _isLogRequired = false;

    public override Task OnBotStarted()
    {
        _cts = new CancellationTokenSource();
        _isLogRequired = BotController.config.logSettings.logDailyNotifications;
        SendingLogicAsync(_cts.Token);
        return Task.CompletedTask;
    }

    public override Task OnBotStopped()
    {
        _cts.Cancel();
        return Task.CompletedTask;
    }

    public async Task ScheduleReminder(ProfileData profileData)
    {
        try
        {
            var reminderData = new DailyReminderData()
            {
                dbid = profileData.dbid,
                userId = profileData.telegram_id,
                timeToSend = DateTime.UtcNow.AddDays(1),
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
            await Task.Delay(60_000).FastAwait();
        }
    }

    private async void SendRemindersAsync(CancellationToken cancellationToken)
    {
        try
        {
            var db = BotController.dataBase.db;
            var now = DateTime.UtcNow;
            var reminderDatas = await db.Table<DailyReminderData>().Where(x => x.timeToSend < now).ToArrayAsync().FastAwait();
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

    private async void SendReminderAsync(DailyReminderData reminderData, CancellationToken cancellationToken)
    {
        try
        {
            var index = new Random().Next(localizationKeys.Length);
            var text = Localization.Get(reminderData.languageCode, localizationKeys[index]);
            var buttonText = Localization.Get(reminderData.languageCode, "restart_button");
            var keyboard = new ReplyKeyboardMarkup(buttonText);
            await messageSender.SendTextDialog(reminderData.userId, text, keyboard, cancellationToken: cancellationToken).FastAwait();
            await BotController.dataBase.db.DeleteAsync(reminderData).FastAwait();
            if (_isLogRequired)
            {
                Program.logger.Info($"Daily notification sended for {reminderData.userId}");
            }
        }
        catch (Exception ex)
        {
            Program.logger.Error("Catched exception at RemindersManager.SendReminderAsync:\n" + ex);
        }
    }

}
