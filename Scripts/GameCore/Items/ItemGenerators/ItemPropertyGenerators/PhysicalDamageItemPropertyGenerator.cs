using System;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.ItemGenerators.ItemPropertyGenerators
{
    class PhysicalDamageItemPropertyGenerator : ItemPropertyGeneratorBase
    {
        public override string debugDescription => "Физический Урон";
        public override ItemPropertyGeneratorType propertyType => ItemPropertyGeneratorType.PhysicalDamage;

        public int minDamage;
        public int maxDamage;

        public PhysicalDamageItemPropertyGenerator(int minValue, int maxValue)
        {
            minDamage = minValue;
            maxDamage = maxValue;
        }

        public override ItemPropertyBase Generate()
        {
            var damage = new Random().Next(minDamage, maxDamage + 1);
            return new PhysicalDamageItemProperty(damage);
        }

        public override ItemPropertyGeneratorBase Clone()
        {
            return new PhysicalDamageItemPropertyGenerator(minDamage, maxDamage);
        }

        public override string ToString()
        {
            return "- " + debugDescription + $": {minDamage} - {maxDamage} (random value)";
        }

    }
}
