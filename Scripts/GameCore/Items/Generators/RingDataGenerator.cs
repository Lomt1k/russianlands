using System;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    public class RingDataGenerator : ItemDataGeneratorBase
    {
        public RingDataGenerator(ItemDataSeed _seed) : base(_seed)
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
            var damageResist = (int)Math.Round(gradedPoints * 0.25);
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
                    var strength = (int)Math.Round(seed.requiredLevel * gradeMult * 1.3f);
                    AddIncreaseAttributeStrength(strength);
                    break;
                case PropertyType.IncreaseAttributeVitality:
                    var vitality = (int)Math.Round(seed.requiredLevel * gradeMult * 1.3f);
                    AddIncreaseAttributeVitality(vitality);
                    break;
                case PropertyType.IncreaseAttributeSorcery:
                    var sorcery = (int)Math.Round(seed.requiredLevel * gradeMult * 1.3f);
                    AddIncreaseAttributeSorcery(sorcery);
                    break;
                case PropertyType.IncreaseAttributeLuck:
                    var luck = (int)Math.Round(seed.requiredLevel * gradeMult * 1.3f);
                    AddIncreaseAttributeLuck(luck);
                    break;
                case PropertyType.IncreaseMaxHealth:
                    AddIncreaseMaxHealth((int)Math.Round(gradedPoints * 0.75));
                    break;
            }
        }

        protected override void AddAbility(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.RestoreHealthEveryTurn:
                    AddRestoreHealthEveryTurn((int)Math.Round(gradedPoints * 0.65), 20f);
                    break;
                case AbilityType.AddManaEveryTurn:
                    var chance = 8 + (int)Math.Round(seed.requiredLevel / 7f);
                    AddManaEveryTurn(1, chance);
                    break;
            }
        }



    }
}
