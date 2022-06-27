using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units.Mobs
{
    internal enum MobType
    {
        None = 0,
        Humanoid = 1,
        Animal = 2,
        Monster = 3,
        Demon = 4,
        Undead = 5,        
    }

    [JsonObject]
    internal class MobData
    {
        public string debugName { get; set; } = string.Empty;
        public string localizationKey { get; set; } = string.Empty;
        public MobType mobType { get; set; } = MobType.None;
        public Rarity rarity { get; set; } = Rarity.Common;
        public bool withGrade { get; set; } = true;
        public MobEncounterSettings encounterSettings { get; set; } = new MobEncounterSettings();
        public MobStatsSettings statsSettings { get; set; } = new MobStatsSettings();
        public List<MobAttack> mobAttacks = new List<MobAttack>();
        //TODO: Add rewards

        public bool CanStartEncounter(GameSession session)
        {
            //TODO: Add rarity check logic
            var playerLevel = session.profile.data.level;
            return playerLevel >= encounterSettings.requiredLevel
                && playerLevel <= encounterSettings.maxLevel;
        }
    }

    [JsonObject]
    internal class MobEncounterSettings
    {
        public bool anyLocation { get; set; } = false;
        public LocationType location { get; set; } = LocationType.None;
        public int requiredLevel { get; set; }
        public int maxLevel { get; set; }
        public bool isCombatAvoidanceAvailable { get; set; } = true;
        public string encounterLocalization { get; set; } = string.Empty;

        public bool CanSpawnOnLocation(LocationType locationType)
        {
            return anyLocation || locationType == location;
        }
    }

    [JsonObject]
    internal class MobStatsSettings
    {
        public int health = 100;
        public int physicalResist;
        public int fireResist;
        public int coldResist;
        public int lightningResist;
    }

    [JsonObject]
    internal class MobAttack
    {
        public string localizationKey = string.Empty;
        public int manaCost;
        public int minPhysicalDamage;
        public int maxPhysicalDamage;
        public int minFireDamage;
        public int maxFireDamage;
        public int minColdDamage;
        public int maxColdDamage;
        public int minLightningDamage;
        public int maxLightningDamage;

        public DamageInfo GetRandomValues()
        {
            var random = new Random();
            return new DamageInfo(
                random.Next(minPhysicalDamage, maxPhysicalDamage + 1),
                random.Next(minFireDamage, maxFireDamage + 1),
                random.Next(minColdDamage, maxColdDamage + 1),
                random.Next(minLightningDamage, maxLightningDamage + 1));
        }
    }





}
