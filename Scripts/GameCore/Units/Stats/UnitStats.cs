using System;
using System.Text;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units.Stats
{
    public abstract class UnitStats
    {
        private int _currentHP;
        private int _damagedHP;
        private int _maxHP;

        public int currentHP => _currentHP;
        public int damagedHP
        {
            get => _damagedHP;
            set
            {
                _damagedHP = value;
                RecalculateCurrentHP();
            }
        }
        public int maxHP
        {
            get => _maxHP;
            set
            {
                _maxHP = value;
                RecalculateCurrentHP();
            }
        }

        public int currentMana { get; protected set; }
        public int currentStickCharge { get; protected set; }
        public int currentArrows { get; protected set; }
        public DamageInfo resistance { get; protected set; }
        public bool isFullHealth => currentHP >= maxHP;

        public void RecalculateCurrentHP()
        {
            var hp = _maxHP - _damagedHP;
            _currentHP = Math.Max(hp, 1);
        }

        public void SetFullHealth()
        {
            damagedHP = 0;
        }

        public void RestoreHealth(int hp)
        {
            damagedHP = Math.Max(damagedHP - hp, 0);
        }

        public void AddMana(int manaAmount)
        {
            currentMana += manaAmount;
        }

        public virtual void OnStartBattle()
        {
            currentMana = 0;
            currentStickCharge = 0;
        }

        public virtual void OnBattleEnd()
        {
            // Гарантируем, что после боя у игрока будет хотя бы 1 HP
            if (damagedHP >= maxHP)
            {
                damagedHP = maxHP - 1;
            }
        }

        public virtual void OnStartMineTurn()
        {
            currentMana++;
        }

        public virtual void OnUseItemInBattle(InventoryItem? item)
        {
            if (item == null)
                return;

            switch (item.data.itemType)
            {
                case ItemType.Bow:
                    currentArrows--;
                    break;
                case ItemType.Stick:
                    currentStickCharge = 0;
                    break;
                case ItemType.Scroll:
                    currentMana -= item.manaCost;
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
            var damageValue = resultDamage.GetTotalValue();
            _currentHP -= damageValue;
            _damagedHP += damageValue;
            return resultDamage;
        }

        public string GetView(GameSession sessionToSend, bool withHealth = true)
        {
            var sb = new StringBuilder();
            if (withHealth)
            {
                sb.AppendLine(Localization.Get(sessionToSend, "unit_view_health"));
                sb.AppendLine($"{Emojis.stats[Stat.Health]} {currentHP} / {maxHP}");
                sb.AppendLine();
            }
            AppendResistsCompactView(sb, sessionToSend);

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
