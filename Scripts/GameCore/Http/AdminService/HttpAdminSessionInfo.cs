using FastTelegramBot.DataTypes;
using MarkOne.Scripts.GameCore.Localizations;
using System;

namespace MarkOne.Scripts.GameCore.Http.AdminService;
public class HttpAdminSessionInfo
{
    public long telegramId { get; }
    public DateTime lastUpdateTime { get; set; }
    public LanguageCode languageCode { get; }
    public User user { get; }
    public bool withoutLogin => telegramId == -1;

    public HttpAdminSessionInfo(long _telegramId, LanguageCode _languageCode, User _user)
    {
        telegramId = _telegramId;
        lastUpdateTime = DateTime.UtcNow;
        languageCode = _languageCode;
        user = _user;
    }

    public string GetAdminView()
    {
        return user.FirstName + (telegramId > -1 ? $" #{telegramId % 10000}" : string.Empty);
    }

}
