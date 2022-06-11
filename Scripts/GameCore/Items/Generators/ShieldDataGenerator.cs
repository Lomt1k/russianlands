using System;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    internal class ShieldDataGenerator : ItemDataGeneratorBase
    {
        public ShieldDataGenerator(ItemDataSeed _seed) : base(_seed)
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
            var physicalDamage = (int)Math.Round(rarityMult * gradedPoints * 1.5f);
            AddBlockIncomingPhysicalDamage(physicalDamage);

            var secondaryDamage = (int)Math.Round(physicalDamage * 0.8f);
            foreach (var param in seed.baseParameters)
            {
                switch (param)
                {
                    case "DF":
                        AddBlockIncomingFireDamage(secondaryDamage);
                        break;
                    case "DC":
                        AddBlockIncomingColdDamage(secondaryDamage);
                        break;
                    case "DL":
                        AddBlockIncomingLightningDamage(secondaryDamage);
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
