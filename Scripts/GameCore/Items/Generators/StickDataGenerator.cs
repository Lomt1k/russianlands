using System;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    internal class StickDataGenerator : ItemDataGeneratorBase
    {
        public StickDataGenerator(ItemDataSeed _seed) : base(_seed)
        {
        }

        protected override void GenerateItemData()
        {
            var rarityMult = 1f;
            switch (seed.rarity)
            {
                case ItemRarity.Epic: rarityMult = 1.1f; break;
                case ItemRarity.Legendary: rarityMult = 1.3f; break;
            }
            AddBaseParameters(rarityMult);
            AddProperties();

            requiredCharge = 3;
        }

        private void AddBaseParameters(float rarityMult)
        {
            var generalDamage = Math.Round(rarityMult * gradedPoints * 3.75f);
            var minGeneralDamage = (int)Math.Round(generalDamage * 0.87f);
            var maxGeneralDamage = (int)Math.Round(generalDamage * 1.13f);

            var secondaryDamage = (int)Math.Round(generalDamage * 0.25f);
            bool isFirst = true;
            foreach (var param in seed.baseParameters)
            {
                switch (param)
                {
                    case "DF":
                        if (isFirst)
                            AddDealFireDamage(minGeneralDamage, maxGeneralDamage);
                        else
                            AddDealFireDamage(secondaryDamage);
                        break;
                    case "DC":
                        if (isFirst)
                            AddDealColdDamage(minGeneralDamage, maxGeneralDamage);
                        else
                            AddDealColdDamage(secondaryDamage);
                        break;
                    case "DL":
                        if (isFirst)
                            AddDealLightningDamage(minGeneralDamage, maxGeneralDamage);
                        else
                            AddDealLightningDamage(secondaryDamage);
                        break;
                }
                isFirst = false;
            }
        }

        private void AddProperties()
        {
            foreach (var propertyType in seed.properties)
            {
                AddProperty(propertyType);
            }
        }


    }
}
