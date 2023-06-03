using Newtonsoft.Json;
using System;
using Telegram.Bot.Types.Enums;

namespace MarkOne.Scripts.Bot;

[JsonObject]
public class SimpleUpdate
{
    public int id { get; set; }
    public UpdateType updateType { get; set; } = UpdateType.Unknown;
    public SimpleMessage? message { get; set; }
    public SimpleCallbackQuery? callbackQuery { get; set; }
}

[JsonObject]
public class SimpleMessage
{
    public int id { get; set; }
    public SimpleUser from { get; set; } = new();
    public DateTime date { get; set; }
    public string? text { get; set; } = string.Empty;
    public SimpleDocument? document { get; set; }
}

[JsonObject]
public class SimpleCallbackQuery
{
    public string id { get; set; } = string.Empty;
    public SimpleUser from { get; set; } = new();
    public string? data { get; set; } = string.Empty;
}

[JsonObject]
public class SimpleUser
{
    public long id { get; set; }
    public string firstName { get; set; } = string.Empty;
    public string? lastName { get; set; }
    public string? username { get; set; }

    public override string ToString()
    {
        return username is not null ? $"@{username} (ID {id})"
            : lastName is not null ? $"{firstName} {lastName} (ID {id})"
            : $"{firstName} (ID {id})";
    }
}

[JsonObject]
public class SimpleDocument
{
    public string fileId { get; set; } = string.Empty;
    public string fileUniqueId { get; set; } = string.Empty;
    public string? fileName { get; set; }
    public long? fileSize { get; set; }
    public string? mimeType { get; set; }    
}
