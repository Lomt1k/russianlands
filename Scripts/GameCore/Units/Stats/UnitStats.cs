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
        public int currentStickCharge { get; protected set; }
        public int currentArrows { get; protected set; }
        public DamageInfo resistance { get; protected set; }

        public void SetFullHealth()
        {
            currentHP = maxHP;
        }

        public virtual void OnStartBattle()
        {
            currentMana = 0;
            currentStickCharge = 0;
        }

        public virtual void OnStartMineTurn()
        {
            currentMana++;
        }

        public virtual void OnUseItemInBattle(InventoryItem item)
        {
            switch (item.data.itemType)
            {
                case ItemType.Bow:
                    currentArrows--;
                    break;
                case ItemType.Stick:
                    currentStickCharge = 0;
                    break;
            }
        }

        public void AddOrRemoveResistance(DamageInfo value)
        {
            resistance += value;
        }

        public DamageInfo TryDealDamage(DamageInfo damage)
        {
            var resultDamage = (damage - resistance).EscapeNegative();
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

            if (resistance.HasBigValues())
            {
                sb.Append($"{Emojis.stats[Stat.PhysicalDamage]} " + resistance[DamageType.Physical]);
                sb.AppendLine(Emojis.bigSpace + $"{Emojis.stats[Stat.FireDamage]} " + resistance[DamageType.Fire]);
                sb.Append($"{Emojis.stats[Stat.ColdDamage]} " + resistance[DamageType.Cold]);
                sb.AppendLine(Emojis.bigSpace + $"{Emojis.stats[Stat.LightningDamage]} " + resistance[DamageType.Lightning]);
            }
            else
            {
                sb.Append($"{Emojis.stats[Stat.PhysicalDamage]} " + resistance[DamageType.Physical]);
                sb.Append(Emojis.middleSpace + $"{Emojis.stats[Stat.FireDamage]} " + resistance[DamageType.Fire]);
                sb.Append(Emojis.middleSpace + $"{Emojis.stats[Stat.ColdDamage]} " + resistance[DamageType.Cold]);
                sb.Append(Emojis.middleSpace + $"{Emojis.stats[Stat.LightningDamage]} " + resistance[DamageType.Lightning]);
            }
        }

    }
}
