
namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    using ItemAbilities;
    using System;
    using System.Collections.Generic;

    public abstract partial class ItemDataGeneratorBase
    {
        private Dictionary<AbilityType, int> _stackOfChances = new Dictionary<AbilityType, int>();

        #region Deal Damage

        protected void AddDealPhysicalDamage(int value, float chancePercentage = 100f)
        {
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minPhysicalDamage += value;
                    dealDamage.maxPhysicalDamage += value;

                    var temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.minPhysicalDamage = value;
            newAbility.maxPhysicalDamage = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddDealPhysicalDamage(int minDamage, int maxDamage, float chancePercentage = 100f)
        {
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minPhysicalDamage += minDamage;
                    dealDamage.maxPhysicalDamage += maxDamage;

                    var temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.minPhysicalDamage = minDamage;
            newAbility.maxPhysicalDamage = maxDamage;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddDealFireDamage(int value, float chancePercentage = 100f)
        {
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minFireDamage += value;
                    dealDamage.maxFireDamage += value;

                    var temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.minFireDamage = value;
            newAbility.maxFireDamage = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddDealFireDamage(int minDamage, int maxDamage, float chancePercentage = 100f)
        {
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minFireDamage += minDamage;
                    dealDamage.maxFireDamage += maxDamage;

                    var temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.minFireDamage = minDamage;
            newAbility.maxFireDamage = maxDamage;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddDealColdDamage(int value, float chancePercentage = 100f)
        {
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minColdDamage += value;
                    dealDamage.maxColdDamage += value;

                    var temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.minColdDamage = value;
            newAbility.maxColdDamage = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddDealColdDamage(int minDamage, int maxDamage, float chancePercentage = 100f)
        {
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minColdDamage += minDamage;
                    dealDamage.maxColdDamage += maxDamage;

                    var temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.minColdDamage = minDamage;
            newAbility.maxColdDamage = maxDamage;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddDealLightningDamage(int value, float chancePercentage = 100f)
        {
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minLightningDamage += value;
                    dealDamage.maxLightningDamage += value;

                    var temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.minLightningDamage = value;
            newAbility.maxLightningDamage = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddDealLightningDamage(int minDamage, int maxDamage, float chancePercentage = 100f)
        {
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minLightningDamage += minDamage;
                    dealDamage.maxLightningDamage += maxDamage;

                    var temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (DealDamageAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.minLightningDamage = minDamage;
            newAbility.maxLightningDamage = maxDamage;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }
        #endregion

        #region Block Incoming Damage

        protected void AddBlockIncomingPhysicalDamage(int value, float chancePercentage = 30f)
        {
            var abilityType = AbilityType.BlockIncomingDamageEveryTurn;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is BlockIncomingDamageEveryTurnAbility blockDamage)
                {
                    blockDamage.physicalDamage += value;
                    var temp = blockDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    blockDamage.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (BlockIncomingDamageEveryTurnAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.physicalDamage = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddBlockIncomingFireDamage(int value, float chancePercentage = 30f)
        {
            var abilityType = AbilityType.BlockIncomingDamageEveryTurn;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is BlockIncomingDamageEveryTurnAbility blockDamage)
                {
                    blockDamage.fireDamage += value;
                    var temp = blockDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    blockDamage.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (BlockIncomingDamageEveryTurnAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.fireDamage = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddBlockIncomingColdDamage(int value, float chancePercentage = 30f)
        {
            var abilityType = AbilityType.BlockIncomingDamageEveryTurn;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is BlockIncomingDamageEveryTurnAbility blockDamage)
                {
                    blockDamage.coldDamage += value;
                    var temp = blockDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    blockDamage.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (BlockIncomingDamageEveryTurnAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.coldDamage = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddBlockIncomingLightningDamage(int value, float chancePercentage = 30f)
        {
            var abilityType = AbilityType.BlockIncomingDamageEveryTurn;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is BlockIncomingDamageEveryTurnAbility blockDamage)
                {
                    blockDamage.lightningDamage += value;
                    var temp = blockDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    blockDamage.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (BlockIncomingDamageEveryTurnAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.lightningDamage = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        #endregion

        protected void AddRestoreHealthEveryTurn(int value, float chancePercentage = 100f)
        {
            var abilityType = AbilityType.RestoreHealthEveryTurn;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is RestoreHealthEveryTurnAbility restoreHealth)
                {
                    restoreHealth.healthValue += value;

                    var temp = restoreHealth.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    restoreHealth.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (RestoreHealthEveryTurnAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.healthValue = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddManaEveryTurn(int value, float chancePercentage = 100f)
        {
            var abilityType = AbilityType.AddManaEveryTurn;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is AddManaEveryTurnAbility addManaAbility)
                {
                    addManaAbility.manaValue += value;

                    var temp = addManaAbility.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    addManaAbility.chanceToSuccessPercentage = (float)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (AddManaEveryTurnAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.manaValue = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }


    }
}
