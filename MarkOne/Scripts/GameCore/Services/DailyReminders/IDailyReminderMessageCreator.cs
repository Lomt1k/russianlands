using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using System;

namespace MarkOne.Scripts.GameCore.Services.DailyReminders;
public interface IDailyReminderMessageCreator
{
    string? TryGetMessageText(DailyReminderData data);
}

public class DefaultDailyReminderMessageCreator : IDailyReminderMessageCreator
{
    private static readonly string[] localizationKeys =
    {
        "daily_reminder_variant_0",
        "daily_reminder_variant_1",
        "daily_reminder_variant_2",
        "daily_reminder_variant_3",
        "daily_reminder_variant_4",
        "daily_reminder_variant_5",
    };

    private static DefaultDailyReminderMessageCreator? _instance;

    public static DefaultDailyReminderMessageCreator Instance => _instance ??= new();

    public string? TryGetMessageText(DailyReminderData data)
    {
        var index = new Random().Next(localizationKeys.Length);
        return Localization.Get(data.languageCode, localizationKeys[index]);
    }
}

public class DailyBonusReminderMessageCreator : IDailyReminderMessageCreator
{
    private static readonly string[] notCompletedTutorKeys =
    {
        "daily_reminder_reward_available_after_turor_variant_1",
        "daily_reminder_reward_available_after_turor_variant_2",
        "daily_reminder_reward_available_after_turor_variant_3",
    };

    private static readonly string[] completedTutorKeys =
    {
        "daily_reminder_reward_available_variant_1",
        "daily_reminder_reward_available_variant_2",
        "daily_reminder_reward_available_variant_3",
    };

    private static DailyBonusReminderMessageCreator? _instance;

    public static DailyBonusReminderMessageCreator Instance => _instance ??= new();

    public string? TryGetMessageText(DailyReminderData data)
    {
        if (!data.isDailyBonusAvailable)
        {
            return null;
        }

        var localizationKeys = data.lastLoginTownhall < 2
            ? notCompletedTutorKeys
            : completedTutorKeys;

        var index = new Random().Next(localizationKeys.Length);
        return Localization.Get(data.languageCode, localizationKeys[index]);
    }
}
