using MarkOne.Scripts.GameCore.Services.Payments;
using SQLite;
using System;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

public enum PaymentStatus : byte
{
    NotPaid = 0,
    WaitingForGoods = 1,
    Received = 2,
    ErrorOnTryReceive = 3,
}

[Table("PaymentData")]
public class PaymentData
{
    [PrimaryKey]
    public string orderId { get; set; }
    public long telegramId { get; set; }
    public PaymentProviderType providerType { get; set; }
    public string vendorCode { get; set; } = string.Empty;
    public PaymentStatus status { get; set; }
    public double rubbles { get; set; }
    public DateTime creationDate { get; set; }
    public DateTime expireDate { get; set; }
    public string comment { get; set; } = string.Empty;
    public string url { get; set; } = string.Empty;
    public string signature { get; set; } = string.Empty;
    
}
