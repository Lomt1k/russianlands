using System;

namespace MarkOne.Scripts.GameCore.Http.AdminService;
public class HttpAdminSessionInfo
{
    public long telegramId { get; set; }
    public DateTime lastUpdateTime { get; set; }

    public HttpAdminSessionInfo(long _telegramId)
    {
        telegramId = _telegramId;
        lastUpdateTime = DateTime.UtcNow;
    }
}
