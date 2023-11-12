using System;
using System.Threading;
using System.Threading.Tasks;
using FastTelegramBot;
using FastTelegramBot.DataTypes.Keyboards;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Dialogs.Events.DailyBonus;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Profiles;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

namespace MarkOne.Scripts.GameCore.Services.DailyReminders;

public class ReminderNoticifationSettings
{
    public int hoursDelay;
    public IDailyReminderMessageCreator[] messageCreators = Array.Empty<IDailyReminderMessageCreator>();
}

public class DailyRemindersManager : Service
{
    private static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    private static ReminderNoticifationSettings[] notificationsSequence = new ReminderNoticifationSettings[]
    {
        // Первое уведомление
        new ReminderNoticifationSettings()
        {
            hoursDelay = 24,
            messageCreators = new IDailyReminderMessageCreator[]
            {
                DailyBonusReminderMessageCreator.Instance,
                DefaultDailyReminderMessageCreator.Instance,
            }
        },
        // Второе уведомление
        new ReminderNoticifationSettings()
        {
            hoursDelay = 10,
            messageCreators = new IDailyReminderMessageCreator[]
            {
                DefaultDailyReminderMessageCreator.Instance,
            }
        },
        // Третье уведомление
        new ReminderNoticifationSettings()
        {
            hoursDelay = 14,
            messageCreators = new IDailyReminderMessageCreator[]
            {
                DailyBonusReminderMessageCreator.Instance,
                DefaultDailyReminderMessageCreator.Instance,
            }
        },
        // Четвёртое уведомление
        new ReminderNoticifationSettings()
        {
            hoursDelay = 10,
            messageCreators = new IDailyReminderMessageCreator[]
            {
                DefaultDailyReminderMessageCreator.Instance,
            }
        },
        // Пятое уведомление
        new ReminderNoticifationSettings()
        {
            hoursDelay = 14,
            messageCreators = new IDailyReminderMessageCreator[]
            {
                DailyBonusReminderMessageCreator.Instance,
                DefaultDailyReminderMessageCreator.Instance,
            }
        },
    };

    private CancellationTokenSource _cts = new();
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

    public async Task ScheduleReminderSequence(Profile profile)
    {
        try
        {
            var profileData = profile.data;
            var reminderData = new DailyReminderData()
            {
                dbid = profileData.dbid,
                userId = profileData.telegram_id,
                timeToSend = DateTime.UtcNow.AddHours(notificationsSequence[0].hoursDelay),
                languageCode = profileData.language,
                notificationIndex = 0,
                lastLoginTownhall = profile.buildingsData.townHallLevel,
                isDailyBonusAvailable = DailyBonusDialog.IsEventAvailable(profileData)
            };
            BotController.dataBase.db.InsertOrReplace(reminderData);
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
            var reminderDatas = db.Table<DailyReminderData>().Where(x => x.timeToSend < now).ToArray();
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
            var notificationIndex = reminderData.notificationIndex;
            if (notificationIndex >= notificationsSequence.Length)
            {
                BotController.dataBase.db.Delete(reminderData);
                return;
            }

            var notificationSettings = notificationsSequence[notificationIndex];
            string? text = null;
            foreach (var messageCreator in notificationSettings.messageCreators)
            {
                text ??= messageCreator.TryGetMessageText(reminderData);
            }
            if (text is null)
            {
                BotController.dataBase.db.Delete(reminderData);
                return;
            }

            var buttonText = Localization.Get(reminderData.languageCode, "restart_button");
            var keyboard = new ReplyKeyboardMarkup(buttonText);
            await messageSender.SendTextDialog(reminderData.userId, text, keyboard, cancellationToken: cancellationToken).FastAwait();
            if (_isLogRequired)
            {
                Program.logger.Info($"Daily notification #{notificationIndex} sended for (ID {reminderData.userId})");
            }

            // schedule next reminder
            if (notificationIndex + 1 >= notificationsSequence.Length)
            {
                BotController.dataBase.db.Delete(reminderData);
            }
            else
            {
                notificationIndex++;
                reminderData.notificationIndex = notificationIndex;
                reminderData.timeToSend = DateTime.UtcNow.AddHours(notificationsSequence[notificationIndex].hoursDelay);
                BotController.dataBase.db.Update(reminderData);
            }
        }
        catch (TelegramBotException telegramBotEx)
        {
            if (telegramBotEx.ErrorCode != 403)
            {
                Program.logger.Error("Catched exception at DailyRemindersManager.SendReminderAsync:\n" + telegramBotEx);
            }
        }
        catch (Exception ex)
        {
            Program.logger.Error("Catched exception at DailyRemindersManager.SendReminderAsync:\n" + ex);
        }
    }

}
