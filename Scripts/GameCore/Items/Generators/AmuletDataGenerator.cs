using System;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    internal class AmuletDataGenerator : ItemDataGeneratorBase
    {
        public AmuletDataGenerator(ItemDataSeed _seed) : base(_seed)
        {
        }

        protected override void GenerateItemData()
        {
            AddBaseParameters();
            AddProperties();
            AddAbilities();
        }

        private void AddBaseParameters()
        {
            var damageResist = (int)Math.Round(gradedPoints * 0.5);
            foreach (var param in seed.baseParameters)
            {
                switch (param)
                {
                    case "DP":
                        AddPhysicalDamageResist(damageResist);
                        break;
                    case "DF":
                        AddFireDamageResist(damageResist);
                        break;
                    case "DC":
                        AddColdDamageResist(damageResist);
                        break;
                    case "DL":
                        AddLightningDamageResist(damageResist);
                        break;
                }
            }
        }

        protected override void AddProperty(PropertyType propertyType)
        {
            switch (propertyType)
            {
                case PropertyType.IncreaseAttributeStrength:
                    var strength = (int)Math.Round(seed.requiredLevel * gradeMult / 4) + 1;
                    AddIncreaseAttributeStrength(strength);
                    break;
                case PropertyType.IncreaseAttributeVitality:
                    var vitality = (int)Math.Round(seed.requiredLevel * gradeMult / 4) + 1;
                    AddIncreaseAttributeVitality(vitality);
                    break;
                case PropertyType.IncreaseAttributeSorcery:
                    var sorcery = (int)Math.Round(seed.requiredLevel * gradeMult / 4) + 1;
                    AddIncreaseAttributeSorcery(sorcery);
                    break;
                case PropertyType.IncreaseAttributeLuck:
                    var luck = (int)Math.Round(seed.requiredLevel * gradeMult / 4) + 1;
                    AddIncreaseAttributeLuck(luck);
                    break;
                case PropertyType.IncreaseMaxHealth:
                    AddIncreaseMaxHealth((int)Math.Round(gradedPoints * 1.5));
                    break;
            }
        }

        protected override void AddAbility(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.RestoreHealthEveryTurn:
                    AddRestoreHealthEveryTurn((int)Math.Round(gradedPoints * 1.25), 20f);
                    break;
                case AbilityType.AddManaEveryTurn:
                    var chance = 15 + seed.requiredLevel / 10;
                    AddManaEveryTurn(1, chance);
                    break;
            }
        }



    }
}
