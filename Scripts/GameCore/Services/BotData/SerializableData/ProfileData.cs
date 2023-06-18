﻿using SQLite;
using System;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData.DataTypes;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

[Table("Profiles")]
public class ProfileData : DataWithSession
{
    [PrimaryKey, AutoIncrement]
    public long dbid { get; set; }
    public long telegram_id { get; set; }
    [MaxLength(32)] public string? username { get; set; }
    [MaxLength(64)] public string firstName { get; set; }
    [MaxLength(64)] public string? lastName { get; set; }
    public LanguageCode language { get; set; } = LanguageCode.RU;
    [MaxLength(16)] public string nickname { get; set; }
    [MaxLength(24)] public string regDate { get; set; }
    [MaxLength(16)] public string regVersion { get; set; }
    [MaxLength(24)] public string lastActivityTime { get; set; }
    [MaxLength(16)] public string lastVersion { get; set; }

    public AdminStatus adminStatus { get; set; }
    public byte level { get; set; } = 1;
    public byte freeNickChanges { get; set; } = 1;
    public long endPremiumTime { get; set; }


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


    public ProfileData SetupNewProfile(SimpleUser user)
    {
        telegram_id = user.id;
        regDate = DateTime.UtcNow.AsDateString();
        regVersion = ProjectVersion.Current.ToString();
        lastVersion = regVersion;
        nickname = user.firstName.IsCorrectNickname() ? user.firstName : "Player_" + (new Random().Next(8999) + 1000);
        username = user.username;
        firstName = user.firstName;
        lastName = user.lastName;

        return this;
    }

    public bool IsPremiumActive()
    {
        return endPremiumTime > DateTime.UtcNow.Ticks;
    }

    public bool IsPremiumExpired()
    {
        return !IsPremiumActive() && endPremiumTime > 0;
    }


}
