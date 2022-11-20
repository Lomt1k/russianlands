using System;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    public class StickDataGenerator : ItemDataGeneratorBase
    {
        public StickDataGenerator(ItemDataSeed _seed) : base(_seed)
        {
        }

        protected override void GenerateItemData()
        {
            var rarityMult = 1f;
            switch (seed.rarity)
            {
                case Rarity.Rare: rarityMult = 1.15f; break;
                case Rarity.Epic: rarityMult = 1.3f; break;
                case Rarity.Legendary: rarityMult = 1.45f; break;
            }
            AddBaseParameters(rarityMult);
        }

        private void AddBaseParameters(float rarityMult)
        {
            var generalDamage = Math.Round(rarityMult * gradedPoints * 3.75f);
            var minGeneralDamage = (int)Math.Round(generalDamage * 0.87f);
            var maxGeneralDamage = (int)Math.Round(generalDamage * 1.13f);

            foreach (var param in seed.baseParameters)
            {
                switch (param)
                {
                    case "DF":
                        AddDealFireDamage(minGeneralDamage, maxGeneralDamage);
                        break;
                    case "DC":
                        AddDealColdDamage(minGeneralDamage, maxGeneralDamage);
                        break;
                    case "DL":
                        AddDealLightningDamage(minGeneralDamage, maxGeneralDamage);
                        break;
                }
            }
        }


    }
}
