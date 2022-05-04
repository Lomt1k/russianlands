using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    class PhysicalDamageItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Физический Урон";
        public override ItemPropertyType propertyType => ItemPropertyType.PhysicalDamage;

        public int minDamage;
        public int maxDamage;

        public PhysicalDamageItemProperty(int minDamage, int maxDamage)
        {
            this.minDamage = minDamage;
            this.maxDamage = maxDamage;
        }

        public int GetRandomValue()
        {
            return Randomizer.random.Next(minDamage, maxDamage + 1);
        }

        public override string ToString()
        {
            return minDamage == maxDamage 
                ? $"{debugDescription}: {minDamage}"
                : $"{debugDescription}: {minDamage} - {maxDamage}";
        }
    }
}
