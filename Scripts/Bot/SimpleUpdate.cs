using Newtonsoft.Json;
using System;
using Telegram.Bot.Types.Enums;

namespace MarkOne.Scripts.Bot;

[JsonObject]
public class SimpleUpdate
{
    public int id { get; set; }
    public UpdateType updateType { get; set; } = UpdateType.Unknown;
    public SimpleMeesage? message { get; set; }
}

[JsonObject]
public class SimpleMeesage
{
    public int id { get; set; }
    public SimpleUser from { get; set; } = new();
    public DateTime date { get; set; }
    public string? text { get; set; } = string.Empty;
}

[JsonObject]
public class SimpleUser
{
    public long id { get; set; }
    public string firstName { get; set; } = string.Empty;
    public string? lastName { get; set; }
    public string? username { get; set; }
}
