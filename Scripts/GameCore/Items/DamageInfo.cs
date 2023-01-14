using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Items
{
    public enum DamageType { Physical, Fire, Cold, Lightning }

    public struct DamageInfo
    {
        private Dictionary<DamageType, int> _damage { get; }
        private int _damageTypesCount;

        public int this[DamageType damageType]
        {
            get => _damage[damageType];
            set => _damage[damageType] = value;
        }
        public int damageTypesCount => _damageTypesCount;

        public DamageInfo(int physicalDamage = 0, int fireDamage = 0, int coldDamage = 0, int lightningDamage = 0)
        {
            _damage = new Dictionary<DamageType, int>();
            _damage[DamageType.Physical] = physicalDamage;
            _damage[DamageType.Fire] = fireDamage;
            _damage[DamageType.Cold] = coldDamage;
            _damage[DamageType.Lightning] = lightningDamage;

            _damageTypesCount = 0;
            foreach (var damage in _damage.Values)
            {
                if (damage != 0)
                    _damageTypesCount++;
            }
        }

        public static DamageInfo operator +(DamageInfo a) => a;
        public static DamageInfo operator +(DamageInfo a, DamageInfo b)
        {
            return new DamageInfo(
                physicalDamage: a[DamageType.Physical] + b[DamageType.Physical],
                fireDamage: a[DamageType.Fire] + b[DamageType.Fire],
                coldDamage: a[DamageType.Cold] + b[DamageType.Cold],
                lightningDamage: a[DamageType.Lightning] + b[DamageType.Lightning]);
        }
        public static DamageInfo operator -(DamageInfo a)
        {
            return new DamageInfo(-a[DamageType.Physical], -a[DamageType.Fire], -a[DamageType.Cold], -a[DamageType.Lightning]);
        }
        public static DamageInfo operator -(DamageInfo a, DamageInfo b) => a + (-b);
        public static DamageInfo operator *(DamageInfo a, int b)
        {
            return new DamageInfo(a[DamageType.Physical] * b, a[DamageType.Fire] * b, a[DamageType.Cold] * b, a[DamageType.Lightning] * b);
        }
        public static DamageInfo operator *(DamageInfo a, float b)
        {
            return new DamageInfo(
                (int)Math.Round(a[DamageType.Physical] * b),
                (int)Math.Round(a[DamageType.Fire] * b),
                (int)Math.Round(a[DamageType.Cold] * b),
                (int)Math.Round(a[DamageType.Lightning] * b)
                );
        }
        public static DamageInfo operator /(DamageInfo a, int b)
        {
            return new DamageInfo(a[DamageType.Physical] / b, a[DamageType.Fire] / b, a[DamageType.Cold] / b, a[DamageType.Lightning] / b);
        }
        public static DamageInfo Zero => new DamageInfo(0);

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
                sb.Append(Emojis.StatPhysicalDamage + _damage[DamageType.Physical].ToString());
                if (damageTypesCount > 1)
                    sb.Append(Emojis.middleSpace);
            }
            if (HasDamageType(DamageType.Fire))
            {
                sb.Append(Emojis.StatFireDamage + _damage[DamageType.Fire].ToString());
                if (HasAllDamageTypes() && HasBigValues())
                {
                    sb.AppendLine();
                }
                else if (damageTypesCount > 2)
                {
                    sb.Append(Emojis.middleSpace);
                }
            }
            if (HasDamageType(DamageType.Cold))
            {
                sb.Append(Emojis.StatColdDamage + _damage[DamageType.Cold].ToString());
                if (damageTypesCount > 3)
                    sb.Append(Emojis.middleSpace);
            }
            if (HasDamageType(DamageType.Lightning))
            {
                sb.Append(Emojis.StatLightningDamage + _damage[DamageType.Lightning].ToString());
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

        public DamageInfo EscapeNegative()
        {
            return new DamageInfo(
                physicalDamage: Math.Max(_damage[DamageType.Physical], 0),
                fireDamage: Math.Max(_damage[DamageType.Fire], 0),
                coldDamage: Math.Max(_damage[DamageType.Cold], 0),
                lightningDamage: Math.Max(_damage[DamageType.Lightning], 0));
        }

    }

    
}
