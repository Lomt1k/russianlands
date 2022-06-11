using System;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    internal class HelmetDataGenerator : ItemDataGeneratorBase
    {
        public HelmetDataGenerator(ItemDataSeed _seed) : base(_seed)
        {
        }

        protected override void GenerateItemData()
        {
            var rarityMult = 1f;
            switch (seed.rarity)
            {
                case ItemRarity.Rare: rarityMult = 1.1f; break;
                case ItemRarity.Epic: rarityMult = 1.3f; break;
                case ItemRarity.Legendary: rarityMult = 1.5f; break;
            }
            AddBaseParameters(rarityMult);
            AddProperties();
        }

        private void AddBaseParameters(float rarityMult)
        {
            var physicalDamage = (int)Math.Round(rarityMult * gradedPoints / 2);
            AddPhysicalDamageResist(physicalDamage);

            var secondaryDamage = (int)Math.Round(rarityMult * gradedPoints / 2);
            foreach (var param in seed.baseParameters)
            {
                switch (param)
                {
                    case "DF":
                        AddFireDamageResist(secondaryDamage);
                        break;
                    case "DC":
                        AddColdDamageResist(secondaryDamage);
                        break;
                    case "DL":
                        AddLightningDamageResist(secondaryDamage);
                        break;
                }
            }
        }

        private void AddProperties()
        {
            foreach (var propertyType in seed.properties)
            {
                AddProperty(propertyType);
            }
        }

        private void AddProperty(PropertyType propertyType)
        {
            switch (propertyType)
            {
                case PropertyType.IncreaseAttributeStrength:
                    var strength = (int)Math.Round(seed.requiredLevel * gradeMult / 10) + 1;
                    AddIncreaseAttributeStrength(strength);
                    break;
                case PropertyType.IncreaseAttributeVitality:
                    var vitality = (int)Math.Round(seed.requiredLevel * gradeMult / 10) + 1;
                    AddIncreaseAttributeVitality(vitality);
                    break;
                case PropertyType.IncreaseAttributeSorcery:
                    var sorcery = (int)Math.Round(seed.requiredLevel * gradeMult / 10) + 1;
                    AddIncreaseAttributeSorcery(sorcery);
                    break;
                case PropertyType.IncreaseAttributeLuck:
                    var luck = (int)Math.Round(seed.requiredLevel * gradeMult / 10) + 1;
                    AddIncreaseAttributeLuck(luck);
                    break;
                case PropertyType.IncreaseMaxHealth:
                    AddIncreaseMaxHealth((int)Math.Round(gradedPoints * 0.5));
                    break;
            }
        }


    }
}
