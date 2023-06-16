using MarkOne.Scripts.GameCore.Localizations;
using System;

namespace MarkOne.Scripts.GameCore.Http.AdminService;
public class HttpAdminSessionInfo
{
    public long telegramId { get; set; }
    public DateTime lastUpdateTime { get; set; }
    public LanguageCode languageCode { get; set; }

    public HttpAdminSessionInfo(long _telegramId, LanguageCode _languageCode)
    {
        telegramId = _telegramId;
        lastUpdateTime = DateTime.UtcNow;
        languageCode = _languageCode;
    }
}
