using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class FireDamageItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Урон огнём";
        public override ItemPropertyType propertyType => ItemPropertyType.FireDamage;

        public int minDamage;
        public int maxDamage;

        public FireDamageItemProperty(int minDamage, int maxDamage)
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
