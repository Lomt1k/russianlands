using System;
using System.Text;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Units.Stats.StatEffects;

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

        public sbyte currentMana { get; set; }
        public byte currentArrows { get; set; }
        public byte currentStickCharge { get; protected set; }
        public DamageInfo resistance { get; protected set; }
        public bool isFullHealth => currentHP >= maxHP;
        public bool isSkipNextTurnRequired = false;
        public List<StatEffectBase> statEffects = new List<StatEffectBase>();

        /// <summary>
        /// resistance with temporary stat effects
        /// </summary>
        public DamageInfo totalResistance
        {
            get
            {
                var result = resistance;
                foreach (var statEffect in statEffects)
                {
                    if (statEffect is ExtraResistanceStatEffect extraResistance)
                    {
                        result += extraResistance.resistance;
                    }
                }
                return result;
            }
        }

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

        public virtual void OnStartBattle()
        {
            currentMana = 0;
            currentStickCharge = 0;
            isSkipNextTurnRequired = false;
            statEffects.Clear();
        }

        public virtual void OnBattleEnd()
        {
            // Гарантируем, что после боя у игрока будет хотя бы 1 HP
            if (damagedHP >= maxHP)
            {
                damagedHP = maxHP - 1;
            }
            statEffects.Clear();
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

        public void PredictDealDamageResult(DamageInfo damage, out DamageInfo resultDamage, out int resultHealth)
        {
            resultDamage = (damage - totalResistance).EscapeNegative();
            resultHealth = _currentHP - resultDamage.GetTotalValue();
        }

        public DamageInfo TryDealDamage(DamageInfo damage)
        {
            var resultDamage = (damage - totalResistance).EscapeNegative();
            var damageValue = resultDamage.GetTotalValue();
            _currentHP -= damageValue;
            _damagedHP += damageValue;
            statEffects.OnResistanceStatEffectsUsed();
            return resultDamage;
        }

        public string GetView(GameSession sessionToSend, bool withHealth = true)
        {
            var sb = new StringBuilder();
            if (withHealth)
            {
                sb.AppendLine(Localization.Get(sessionToSend, "unit_view_health"));
                sb.AppendLine(Emojis.StatHealth + $"{currentHP} / {maxHP}");
                sb.AppendLine();
            }
            AppendResistsCompactView(sb, sessionToSend);

            return sb.ToString();
        }

        protected void AppendResistsCompactView(StringBuilder sb, GameSession session)
        {
            sb.AppendLine(Localization.Get(session, "unit_view_total_resistance"));

            if (totalResistance.HasBigValues())
            {
                sb.Append(Emojis.StatPhysicalDamage + totalResistance[DamageType.Physical].ToString() + Emojis.bigSpace);
                sb.AppendLine(Emojis.StatFireDamage + totalResistance[DamageType.Fire].ToString());
                sb.Append(Emojis.StatColdDamage + totalResistance[DamageType.Cold].ToString() + Emojis.bigSpace);
                sb.AppendLine(Emojis.StatLightningDamage + totalResistance[DamageType.Lightning].ToString());
            }
            else
            {
                sb.Append(Emojis.StatPhysicalDamage + totalResistance[DamageType.Physical].ToString() + Emojis.middleSpace);
                sb.Append(Emojis.StatFireDamage + totalResistance[DamageType.Fire].ToString() + Emojis.middleSpace);
                sb.Append(Emojis.StatColdDamage + totalResistance[DamageType.Cold].ToString() + Emojis.middleSpace);
                sb.Append(Emojis.StatLightningDamage + totalResistance[DamageType.Lightning].ToString());
            }
        }

    }
}
