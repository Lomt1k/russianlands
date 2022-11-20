using System;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    public class AmuletDataGenerator : ItemDataGeneratorBase
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
            var damageResist = (int)Math.Round(gradedPoints * 0.3);
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
                case PropertyType.IncreaseMaxHealth:
                    AddIncreaseMaxHealth((int)Math.Round(gradedPoints * 1f));
                    break;
            }
        }

        protected override void AddAbility(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.RestoreHealthEveryTurn:
                    AddRestoreHealthEveryTurn((int)Math.Round(gradedPoints * 1.0), 17f);
                    break;
                case AbilityType.AddManaEveryTurn:
                    var chance = 13 + (int)Math.Round(seed.requiredLevel / 5f);
                    AddManaEveryTurn(1, chance);
                    break;
            }
        }



    }
}
