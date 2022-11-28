using System;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    public class StickDataGenerator : ItemDataGeneratorBase
    {
        private float _generalDamage;

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
            AddAbilities();
        }

        private void AddBaseParameters(float rarityMult)
        {
            _generalDamage = (float)Math.Round(rarityMult * gradedPoints * 3.75f);
            var minGeneralDamage = (int)Math.Round(_generalDamage * 0.87f);
            var maxGeneralDamage = (int)Math.Round(_generalDamage * 1.13f);

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

        protected override void AddAbility(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.StealManaKeyword:
                    AddStealManaKeyword(40);
                    break;
                case AbilityType.AdditionalFireDamageKeyword:
                    AddAdditionalFireDamageKeyword((int)Math.Round(_generalDamage * 0.4f), 20);
                    break;
                case AbilityType.AdditionalColdDamageKeyword:
                    AddAdditionalColdDamageKeyword((int)Math.Round(_generalDamage * 0.4f), 20);
                    break;
                case AbilityType.AdditionalLightningDamageKeyword:
                    AddAdditionalLightningDamageKeyword((int)Math.Round(_generalDamage * 0.4f), 20);
                    break;
                case AbilityType.RageKeyword:
                    AddRageKeyword();
                    break;
                case AbilityType.FinishingKeyword:
                    AddFinishingKeyword(30);
                    break;
            }
        }


    }
}
