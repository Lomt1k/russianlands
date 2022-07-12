
using System;
using System.Text;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units.Stats
{
    public abstract class UnitStats
    {

        public int maxHP { get; protected set; }
        public int currentHP { get; protected set; }
        public int currentMana { get; protected set; }
        public int physicalResist { get; protected set; }
        public int fireResist { get; protected set; }
        public int coldResist { get; protected set; }
        public int lightningResist { get; protected set; }

        public void SetFullHealth()
        {
            currentHP = maxHP;
        }

        public virtual void OnBattleStart()
        {
            currentMana = 0;
        }

        public void AddManaPoint()
        {
            currentMana++;
        }

        public DamageInfo TryDealDamage(DamageInfo damage)
        {
            var physical = Math.Max(damage[DamageType.Physical] - physicalResist, 0);
            var fire = Math.Max(damage[DamageType.Fire] - fireResist, 0);
            var cold = Math.Max(damage[DamageType.Cold] - coldResist, 0);
            var lightning = Math.Max(damage[DamageType.Lightning] - lightningResist, 0);

            var resultDamage = new DamageInfo(physical, fire, cold, lightning);
            currentHP -= resultDamage.GetTotalValue();
            return resultDamage;
        }

        public abstract string GetView(GameSession session);

        public string GetStartTurnView(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"\n{Emojis.stats[Stat.Health]} {currentHP} / {maxHP}" +
                $"{Emojis.bigSpace}{Emojis.stats[Stat.Mana]} {currentMana}");

            sb.AppendLine();
            AppendResistsCompactView(sb, session);

            return sb.ToString();
        }

        protected void AppendResistsCompactView(StringBuilder sb, GameSession session)
        {
            sb.AppendLine(Localization.Get(session, "unit_view_total_resistance"));

            bool hasBigValues = physicalResist > 999
                || fireResist > 999
                || coldResist > 999
                || lightningResist > 999;

            if (hasBigValues)
            {
                sb.Append($"{Emojis.stats[Stat.PhysicalDamage]} " + physicalResist);
                sb.AppendLine(Emojis.bigSpace + $"{Emojis.stats[Stat.FireDamage]} " + fireResist);
                sb.Append($"{Emojis.stats[Stat.ColdDamage]} " + coldResist);
                sb.AppendLine(Emojis.bigSpace + $"{Emojis.stats[Stat.LightningDamage]} " + lightningResist);
            }
            else
            {
                sb.Append($"{Emojis.stats[Stat.PhysicalDamage]} " + physicalResist);
                sb.Append(Emojis.middleSpace + $"{Emojis.stats[Stat.FireDamage]} " + fireResist);
                sb.Append(Emojis.middleSpace + $"{Emojis.stats[Stat.ColdDamage]} " + coldResist);
                sb.Append(Emojis.middleSpace + $"{Emojis.stats[Stat.LightningDamage]} " + lightningResist);
            }
        }

    }
}
