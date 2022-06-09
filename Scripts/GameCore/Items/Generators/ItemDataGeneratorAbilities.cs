
namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    using ItemAbilities;

    internal abstract partial class ItemDataGeneratorBase
    {
        protected void AddDealPhysicalDamage(int value)
        {
            if (_abilities.TryGetValue(AbilityType.DealDamage, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minPhysicalDamage += value;
                    dealDamage.maxPhysicalDamage += value;
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(AbilityType.DealDamage);
            newAbility.minPhysicalDamage = value;
            newAbility.maxPhysicalDamage = value;
            _abilities.Add(AbilityType.DealDamage, newAbility);
        }

        protected void AddDealPhysicalDamage(int minDamage, int maxDamage)
        {
            if (_abilities.TryGetValue(AbilityType.DealDamage, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minPhysicalDamage += minDamage;
                    dealDamage.maxPhysicalDamage += maxDamage;
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(AbilityType.DealDamage);
            newAbility.minPhysicalDamage = minDamage;
            newAbility.maxPhysicalDamage = maxDamage;
            _abilities.Add(AbilityType.DealDamage, newAbility);
        }

        protected void AddDealFireDamage(int value)
        {
            if (_abilities.TryGetValue(AbilityType.DealDamage, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minFireDamage += value;
                    dealDamage.maxFireDamage += value;
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(AbilityType.DealDamage);
            newAbility.minFireDamage = value;
            newAbility.maxFireDamage = value;
            _abilities.Add(AbilityType.DealDamage, newAbility);
        }

        protected void AddDealFireDamage(int minDamage, int maxDamage)
        {
            if (_abilities.TryGetValue(AbilityType.DealDamage, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minFireDamage += minDamage;
                    dealDamage.maxFireDamage += maxDamage;
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(AbilityType.DealDamage);
            newAbility.minFireDamage = minDamage;
            newAbility.maxFireDamage = maxDamage;
            _abilities.Add(AbilityType.DealDamage, newAbility);
        }

        protected void AddDealColdDamage(int value)
        {
            if (_abilities.TryGetValue(AbilityType.DealDamage, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minColdDamage += value;
                    dealDamage.maxColdDamage += value;
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(AbilityType.DealDamage);
            newAbility.minColdDamage = value;
            newAbility.maxColdDamage = value;
            _abilities.Add(AbilityType.DealDamage, newAbility);
        }

        protected void AddDealColdDamage(int minDamage, int maxDamage)
        {
            if (_abilities.TryGetValue(AbilityType.DealDamage, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minColdDamage += minDamage;
                    dealDamage.maxColdDamage += maxDamage;
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(AbilityType.DealDamage);
            newAbility.minColdDamage = minDamage;
            newAbility.maxColdDamage = maxDamage;
            _abilities.Add(AbilityType.DealDamage, newAbility);
        }

        protected void AddDealLightningDamage(int value)
        {
            if (_abilities.TryGetValue(AbilityType.DealDamage, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minLightningDamage += value;
                    dealDamage.maxLightningDamage += value;
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(AbilityType.DealDamage);
            newAbility.minLightningDamage = value;
            newAbility.maxLightningDamage = value;
            _abilities.Add(AbilityType.DealDamage, newAbility);
        }

        protected void AddDealLightningDamage(int minDamage, int maxDamage)
        {
            if (_abilities.TryGetValue(AbilityType.DealDamage, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minLightningDamage += minDamage;
                    dealDamage.maxLightningDamage += maxDamage;
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(AbilityType.DealDamage);
            newAbility.minLightningDamage = minDamage;
            newAbility.maxLightningDamage = maxDamage;
            _abilities.Add(AbilityType.DealDamage, newAbility);
        }


    }
}
