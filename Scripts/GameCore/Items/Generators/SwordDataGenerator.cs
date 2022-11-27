using System;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    public class SwordDataGenerator : ItemDataGeneratorBase
    {
        private int _secondaryDamage;

        public SwordDataGenerator(ItemDataSeed _seed) : base(_seed)
        {
        }

        protected override void GenerateItemData()
        {
            var rarityMult = 1f;
            switch (seed.rarity)
            {
                case Rarity.Rare: rarityMult = 1.1f; break;
                case Rarity.Epic: rarityMult = 1.2f; break;
                case Rarity.Legendary: rarityMult = 1.3f; break;
            }
            AddBaseParameters(rarityMult);
            AddProperties();
            AddAbilities();
        }

        private void AddBaseParameters(float rarityMult)
        {
            var physicalDamage = Math.Round(rarityMult * gradedPoints * 3.0f);
            var minPhysicalDamage = (int)Math.Round(physicalDamage * 0.87f);
            var maxPhysicalDamage = (int)Math.Round(physicalDamage * 1.13f);
            AddDealPhysicalDamage(minPhysicalDamage, maxPhysicalDamage);

            _secondaryDamage = seed.rarity switch
            {
                Rarity.Rare => (int)Math.Round(physicalDamage * 0.25f),
                Rarity.Epic => (int)Math.Round(physicalDamage * 0.50f),
                Rarity.Legendary => (int)Math.Round(physicalDamage * 0.55f),
                _ => 0
            };

            foreach (var param in seed.baseParameters)
            {
                switch (param)
                {
                    case "DF":
                        AddDealFireDamage(_secondaryDamage);
                        break;
                    case "DC":
                        AddDealColdDamage(_secondaryDamage);
                        break;
                    case "DL":
                        AddDealLightningDamage(_secondaryDamage);
                        break;
                }
            }
        }

        protected override void AddAbility(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.SwordBlockEveryTurnKeyword:
                    var damageAbility = (DealDamageAbility)_abilities[AbilityType.DealDamage];
                    if (damageAbility != null)
                    {
                        var blockInfo = damageAbility.GetAverageValues() * 0.5f;
                        AddSwordBlockKeyword(blockInfo, 15);
                    }
                    break;
                case AbilityType.AddArrowKeyword:
                    AddAddArrowKeyword(50);
                    break;
                case AbilityType.StealManaKeyword:
                    AddStealManaKeyword(65);
                    break;
                case AbilityType.AdditionalFireDamageKeyword:
                    AddAdditionalFireDamageKeyword(_secondaryDamage, 20);
                    break;
                case AbilityType.AdditionalColdDamageKeyword:
                    AddAdditionalColdDamageKeyword(_secondaryDamage, 20);
                    break;
                case AbilityType.AdditionalLightningDamageKeyword:
                    AddAdditionalLightningDamageKeyword(_secondaryDamage, 20);
                    break;
                case AbilityType.RageKeyword:
                    AddRageKeyword();
                    break;
            }
        }


    }
}
