
using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items
{
    public struct DamageInfo
    {
        private Dictionary<DamageType, int> _damage { get; }

        public int this[DamageType damageType] => _damage[damageType];

        public DamageInfo(int _physicalDamage, int _fireDamage, int _coldDamage, int _lightningDamage)
        {
            _damage = new Dictionary<DamageType, int>();
            _damage[DamageType.Physical] = _physicalDamage;
            _damage[DamageType.Fire] = _fireDamage;
            _damage[DamageType.Cold] = _coldDamage;
            _damage[DamageType.Lightning] = _lightningDamage;
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
    }

    public enum DamageType { Physical, Fire, Cold, Lightning }
}
