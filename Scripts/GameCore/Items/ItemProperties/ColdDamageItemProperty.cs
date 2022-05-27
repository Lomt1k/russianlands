using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class ColdDamageItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Урон холодом";
        public override ItemPropertyType propertyType => ItemPropertyType.ColdDamage;

        public int minDamage;
        public int maxDamage;

        public ColdDamageItemProperty(int minDamage, int maxDamage)
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
