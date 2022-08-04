using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.TelegramBot;

namespace TextGameRPG.Scripts.GameCore.Items
{
    public struct DamageInfo
    {
        private Dictionary<DamageType, int> _damage { get; }
        private int _damageTypesCount;

        public int this[DamageType damageType] => _damage[damageType];
        public int damageTypesCount => _damageTypesCount;

        public DamageInfo(int _physicalDamage, int _fireDamage, int _coldDamage, int _lightningDamage)
        {
            _damage = new Dictionary<DamageType, int>();
            _damage[DamageType.Physical] = _physicalDamage;
            _damage[DamageType.Fire] = _fireDamage;
            _damage[DamageType.Cold] = _coldDamage;
            _damage[DamageType.Lightning] = _lightningDamage;

            _damageTypesCount = 0;
            foreach (var damage in _damage.Values)
            {
                if (damage != 0)
                    _damageTypesCount++;
            }
        }

        public int GetTotalValue()
        {
            int sum = 0;
            foreach (var damage in _damage.Values)
            {
                sum += damage;
            }
            return sum;
        }

        public string? GetCompactView()
        {
            if (GetTotalValue() < 1)
                return null;

            var sb = new StringBuilder();
            if (HasDamageType(DamageType.Physical))
            {
                sb.Append($"{Emojis.stats[Stat.PhysicalDamage]} {_damage[DamageType.Physical]}");
                if (damageTypesCount > 1)
                    sb.Append(' ');
            }
            if (HasDamageType(DamageType.Fire))
            {
                sb.Append($"{Emojis.stats[Stat.FireDamage]} {_damage[DamageType.Fire]}");
                if (HasAllDamageTypes() && HasBigValues())
                {
                    sb.AppendLine();
                }
                else if (damageTypesCount > 2)
                {
                    sb.Append(' ');
                }
            }
            if (HasDamageType(DamageType.Cold))
            {
                sb.Append($"{Emojis.stats[Stat.ColdDamage]} {_damage[DamageType.Cold]}");
                if (damageTypesCount > 3)
                    sb.Append(' ');
            }
            if (HasDamageType(DamageType.Lightning))
            {
                sb.Append($"{Emojis.stats[Stat.LightningDamage]} {_damage[DamageType.Lightning]}");
            }

            return sb.ToString();
        }

        public bool HasDamageType(DamageType damageType)
        {
            return _damage[damageType] != 0;
        }

        public bool HasAllDamageTypes()
        {
            foreach (var damage in _damage.Values)
            {
                if (damage == 0)
                    return false;
            }
            return true;
        }

        public bool HasBigValues()
        {
            foreach (var damage in _damage.Values)
            {
                if (damage > 999)
                    return true;
            }
            return false;
        }
    }

    public enum DamageType { Physical, Fire, Cold, Lightning }
}
