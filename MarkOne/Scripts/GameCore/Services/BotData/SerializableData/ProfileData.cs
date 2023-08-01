using SQLite;
using System;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData.DataTypes;
using FastTelegramBot.DataTypes;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

[Table("Profiles")]
public class ProfileData : DataWithSession
{
    [PrimaryKey, AutoIncrement]
    public long dbid { get; set; }
    public long telegram_id { get; set; }
    [MaxLength(32)] public string? username { get; set; }
    [MaxLength(64)] public string firstName { get; set; } = string.Empty;
    [MaxLength(64)] public string? lastName { get; set; }
    public LanguageCode language { get; set; } = LanguageCode.RU;
    [MaxLength(16)] public string nickname { get; set; } = string.Empty;
    [MaxLength(24)] public DateTime regDate { get; set; }
    [MaxLength(16)] public string regVersion { get; set; } = string.Empty;
    [MaxLength(32)] public string regInfo { get; set; } = string.Empty;
    [MaxLength(24)] public DateTime lastActivityTime { get; set; }
    [MaxLength(16)] public string lastVersion { get; set; } = string.Empty;

    public AdminStatus adminStatus { get; set; }
    public byte level { get; set; } = 1;
    public byte freeNickChanges { get; set; } = 1;
    public DateTime endPremiumTime { get; set; }
    public DateTime lastPremiumDailyRewardTime { get; set; }
    public DateTime lastOfferReminderTime { get; set; }
    public int lastNewsId { get; set; } = -1;
    public string specialNotification { get; set; } = string.Empty;
    public uint revenueRUB { get; set; }
    public bool hasWaitingGoods { get; set; }
    public bool isDoubleDiamondsBonusUsed { get; set; }


    // resources
    public int resourceGold { get; set; } = 3500;
    public int resourceFood { get; set; } = 1000;
    public int resourceDiamond { get; set; } = 100;
    public int resourceHerbs { get; set; }
    public int resourceWood { get; set; }

    // craft resources
    public int resourceCraftPiecesCommon { get; set; }
    public int resourceCraftPiecesRare { get; set; }
    public int resourceCraftPiecesEpic { get; set; }
    public int resourceCraftPiecesLegendary { get; set; }

    // skill resources
    public int resourceFruitApple { get; set; }
    public int resourceFruitPear { get; set; }
    public int resourceFruitMandarin { get; set; }
    public int resourceFruitCoconut { get; set; }
    public int resourceFruitPineapple { get; set; }
    public int resourceFruitBanana { get; set; }
    public int resourceFruitWatermelon { get; set; }
    public int resourceFruitStrawberry { get; set; }
    public int resourceFruitBlueberry { get; set; }
    public int resourceFruitKiwi { get; set; }
    public int resourceFruitCherry { get; set; }
    public int resourceFruitGrape { get; set; }
    // other resources
    public int resourceCrossroadsEnergy { get; set; } = 5;
    public DateTime lastCrossroadsResourceUpdate { get; set; } = DateTime.UtcNow;
    public int resourceArenaTicket { get; set; }
    public int resourceArenaChip { get; set; }

    // skills
    public byte skillSword { get; set; }
    public byte skillBow { get; set; }
    public byte skillStick { get; set; }
    public byte skillScroll { get; set; }
    public byte skillArmor { get; set; }
    public byte skillHelmet { get; set; }
    public byte skillBoots { get; set; }
    public byte skillShield { get; set; }

    // arena shop items
    public DateTime lastCollectArenaChipsTime { get; set; }
    public DateTime lastArenaItemsUpdateTime { get; set; }
    public string? arenaItemId_0 { get; set; }
    public string? arenaItemId_1 { get; set; }
    public string? arenaItemId_2 { get; set; }
    public string? arenaItemId_3 { get; set; }
    public string? arenaItemId_4 { get; set; }


    public ProfileData SetupNewProfile(User user, string messageText = "")
    {
        telegram_id = user.Id;
        regDate = DateTime.UtcNow;
        regVersion = ProjectVersion.Current.ToString();
        regInfo = messageText.Contains("/start ") ? messageText.Replace("/start ", string.Empty) : "organic";
        lastVersion = regVersion;
        username = user.Username;
        firstName = user.FirstName;
        lastName = user.LastName;
        SetupDefaultNickname(user);
        // Для рандомизации сообщения в окне города
        resourceGold += new Random().Next(11) * 50;

        return this;
    }

    private void SetupDefaultNickname(User user)
    {
        if (user.Username is not null && user.Username.IsCorrectNickname())
        {
            nickname = user.Username;
            return;
        }
        if (user.LastName is not null)
        {
            var fullName = $"{user.FirstName} {user.LastName}";
            if (fullName.IsCorrectNickname())
            {
                nickname = fullName;
                return;
            }
        }
        if (user.FirstName.IsCorrectNickname())
        {
            nickname = user.FirstName;
            return;
        }

        nickname = "Player_" + (new Random().Next(8999) + 1000);
    }

    public bool IsPremiumActive()
    {
        return endPremiumTime > DateTime.UtcNow;
    }

    public bool IsPremiumExpired()
    {
        return !IsPremiumActive() && endPremiumTime > DateTime.MinValue;
    }

    public void AddSpecialNotification(string text)
    {
        var notification = $"{Emojis.ElementSmallBlack} {text}\n\n";
        specialNotification += notification;
    }


}
