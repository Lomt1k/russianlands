using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Managers.GameDataBase;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Units.Mobs
{
    [JsonObject]
    public class MobData : IDataWithIntegerID
    {
        public int id { get; set; }
        public string debugName { get; set; } = "New Mob";
        public string localizationKey { get; set; } = string.Empty;
        public MobType mobType { get; set; } = MobType.Default;
        public MobStatsSettings statsSettings { get; set; } = new MobStatsSettings();

        public List<MobAttack> mobAttacks { get; } = new List<MobAttack>();

        public void OnSetupAppMode(AppMode appMode)
        {
            if (appMode == AppMode.Bot)
            {
                debugName = string.Empty;
            }
        }
    }

    [JsonObject]
    public class MobStatsSettings
    {
        public int level = 1;
        public int health = 100;
        public int physicalResist;
        public int fireResist;
        public int coldResist;
        public int lightningResist;
    }

    [JsonObject]
    public class MobAttack
    {
        public string localizationKey = "battle_action_mob_normal_attack";
        public sbyte manaCost;
        public int minPhysicalDamage;
        public int maxPhysicalDamage;
        public int minFireDamage;
        public int maxFireDamage;
        public int minColdDamage;
        public int maxColdDamage;
        public int minLightningDamage;
        public int maxLightningDamage;

        public DamageInfo GetRandomValues(float gradeMult = 1f)
        {
            var random = new Random();
            var physicalDamage = random.Next(minPhysicalDamage, maxPhysicalDamage + 1);
            var fireDamage = random.Next(minFireDamage, maxFireDamage + 1);
            var coldDamage = random.Next(minColdDamage, maxColdDamage + 1);
            var lightningDamage = random.Next(minLightningDamage, maxLightningDamage + 1);

            return new DamageInfo(
                (int)Math.Round(physicalDamage * gradeMult),
                (int)Math.Round(fireDamage * gradeMult),
                (int)Math.Round(coldDamage * gradeMult),
                (int)Math.Round(lightningDamage * gradeMult));
        }

        public string GetView(GameSession session)
        {
            var sb = new StringBuilder();
            var header = Localization.Get(session, localizationKey) + ':';
            sb.Append(header.Preformatted());
            if (maxPhysicalDamage > 0)
            {
                sb.AppendLine();
                sb.Append(Emojis.StatPhysicalDamage + $"{minPhysicalDamage} - {maxPhysicalDamage}");
            }
            if (maxFireDamage > 0)
            {
                sb.AppendLine();
                sb.Append(Emojis.StatFireDamage + $"{minFireDamage} - {maxFireDamage}");
            }
            if (maxColdDamage > 0)
            {
                sb.AppendLine();
                sb.Append(Emojis.StatColdDamage + $"{minColdDamage} - {maxColdDamage}");
            }
            if (maxLightningDamage > 0)
            {
                sb.AppendLine();
                sb.Append(Emojis.StatLightningDamage + $"{minLightningDamage} - {maxLightningDamage}");
            }
            return sb.ToString();
        }
    }





}
