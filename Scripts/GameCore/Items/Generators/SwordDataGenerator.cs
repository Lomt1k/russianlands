using System;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;

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

            var secondaryDamage = seed.rarity switch
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
            }
        }


    }
}
