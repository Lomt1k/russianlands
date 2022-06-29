using System;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    public class SwordDataGenerator : ItemDataGeneratorBase
    {
        public SwordDataGenerator(ItemDataSeed _seed) : base(_seed)
        {
        }

        protected override void GenerateItemData()
        {
            var rarityMult = 1f;
            switch (seed.rarity)
            {
                case Rarity.Rare: rarityMult = 1.1f; break;
                case Rarity.Epic: rarityMult = 1.3f; break;
                case Rarity.Legendary: rarityMult = 1.5f; break;
            }
            AddBaseParameters(rarityMult);
            AddProperties();
        }

        private void AddBaseParameters(float rarityMult)
        {
            var physicalDamage = Math.Round(rarityMult * gradedPoints * 3.0f);
            var minPhysicalDamage = (int)Math.Round(physicalDamage * 0.87f);
            var maxPhysicalDamage = (int)Math.Round(physicalDamage * 1.13f);
            AddDealPhysicalDamage(minPhysicalDamage, maxPhysicalDamage);

            var secondaryDamage = (int)Math.Round(physicalDamage * 0.25f);
            foreach (var param in seed.baseParameters)
            {
                switch (param)
                {
                    case "DF":
                        AddDealFireDamage(secondaryDamage);
                        break;
                    case "DC":
                        AddDealColdDamage(secondaryDamage);
                        break;
                    case "DL":
                        AddDealLightningDamage(secondaryDamage);
                        break;
                }
            }
        }


    }
}
