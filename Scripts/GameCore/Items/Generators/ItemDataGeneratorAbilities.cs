namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    using ItemAbilities;
    using System;
    using System.Collections.Generic;
    using TextGameRPG.Scripts.Bot;

    public abstract partial class ItemDataGeneratorBase
    {
        private Dictionary<AbilityType, int> _stackOfChances = new Dictionary<AbilityType, int>();

        #region Deal Damage

        protected void AddDealPhysicalDamage(int value, byte chancePercentage = 100)
        {
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minPhysicalDamage += value;
                    dealDamage.maxPhysicalDamage += value;

                    float temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
                    _statIcons.Add(Stat.PhysicalDamage);
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

        protected void AddDealPhysicalDamage(int minDamage, int maxDamage, byte chancePercentage = 100)
        {
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minPhysicalDamage += minDamage;
                    dealDamage.maxPhysicalDamage += maxDamage;

                    float temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
                    _statIcons.Add(Stat.PhysicalDamage);
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

        protected void AddDealFireDamage(int value, byte chancePercentage = 100)
        {
            _statIcons.Add(Stat.FireDamage);
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minFireDamage += value;
                    dealDamage.maxFireDamage += value;

                    float temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
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

        protected void AddDealFireDamage(int minDamage, int maxDamage, byte chancePercentage = 100)
        {
            _statIcons.Add(Stat.FireDamage);
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minFireDamage += minDamage;
                    dealDamage.maxFireDamage += maxDamage;

                    float temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
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

        protected void AddDealColdDamage(int value, byte chancePercentage = 100)
        {
            _statIcons.Add(Stat.ColdDamage);
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minColdDamage += value;
                    dealDamage.maxColdDamage += value;

                    float temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
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

        protected void AddDealColdDamage(int minDamage, int maxDamage, byte chancePercentage = 100)
        {
            _statIcons.Add(Stat.ColdDamage);
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minColdDamage += minDamage;
                    dealDamage.maxColdDamage += maxDamage;

                    float temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
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

        protected void AddDealLightningDamage(int value, byte chancePercentage = 100)
        {
            _statIcons.Add(Stat.LightningDamage);
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minLightningDamage += value;
                    dealDamage.maxLightningDamage += value;

                    float temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
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

        protected void AddDealLightningDamage(int minDamage, int maxDamage, byte chancePercentage = 100)
        {
            _statIcons.Add(Stat.LightningDamage);
            var abilityType = AbilityType.DealDamage;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is DealDamageAbility dealDamage)
                {
                    dealDamage.minLightningDamage += minDamage;
                    dealDamage.maxLightningDamage += maxDamage;

                    float temp = dealDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    dealDamage.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
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

        protected void AddBlockIncomingPhysicalDamage(int value, byte chancePercentage = 30)
        {
            var abilityType = AbilityType.BlockIncomingDamageEveryTurn;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is BlockIncomingDamageEveryTurnAbility blockDamage)
                {
                    blockDamage.physicalDamage += value;
                    float temp = blockDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    blockDamage.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
                    _statIcons.Add(Stat.PhysicalDamage);
                }
                return;
            }

            var newAbility = (BlockIncomingDamageEveryTurnAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.physicalDamage = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddBlockIncomingFireDamage(int value, byte chancePercentage = 30)
        {
            _statIcons.Add(Stat.FireDamage);
            var abilityType = AbilityType.BlockIncomingDamageEveryTurn;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is BlockIncomingDamageEveryTurnAbility blockDamage)
                {
                    blockDamage.fireDamage += value;
                    float temp = blockDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    blockDamage.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (BlockIncomingDamageEveryTurnAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.fireDamage = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddBlockIncomingColdDamage(int value, byte chancePercentage = 30)
        {
            _statIcons.Add(Stat.ColdDamage);
            var abilityType = AbilityType.BlockIncomingDamageEveryTurn;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is BlockIncomingDamageEveryTurnAbility blockDamage)
                {
                    blockDamage.coldDamage += value;
                    float temp = blockDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    blockDamage.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (BlockIncomingDamageEveryTurnAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.coldDamage = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddBlockIncomingLightningDamage(int value, byte chancePercentage = 30)
        {
            _statIcons.Add(Stat.LightningDamage);
            var abilityType = AbilityType.BlockIncomingDamageEveryTurn;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is BlockIncomingDamageEveryTurnAbility blockDamage)
                {
                    blockDamage.lightningDamage += value;
                    float temp = blockDamage.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    blockDamage.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
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

        protected void AddRestoreHealthEveryTurn(int value, byte chancePercentage = 100)
        {
            _statIcons.Add(Stat.RestoreHealth);
            var abilityType = AbilityType.RestoreHealthEveryTurn;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is RestoreHealthEveryTurnAbility restoreHealth)
                {
                    restoreHealth.healthValue += value;

                    float temp = restoreHealth.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    restoreHealth.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (RestoreHealthEveryTurnAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.healthValue = value;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddManaEveryTurn(byte manaAmount, byte chancePercentage = 100)
        {
            _statIcons.Add(Stat.Mana);
            var abilityType = AbilityType.AddManaEveryTurn;
            if (_abilities.TryGetValue(abilityType, out var ability))
            {
                if (ability is AddManaEveryTurnAbility addManaAbility)
                {
                    addManaAbility.manaValue = (byte)(addManaAbility.manaValue + manaAmount);

                    float temp = addManaAbility.chanceToSuccessPercentage * _stackOfChances[abilityType];
                    temp += chancePercentage;
                    _stackOfChances[abilityType]++;
                    addManaAbility.chanceToSuccessPercentage = (byte)Math.Round(temp / _stackOfChances[abilityType]);
                }
                return;
            }

            var newAbility = (AddManaEveryTurnAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            newAbility.manaValue = manaAmount;
            newAbility.chanceToSuccessPercentage = chancePercentage;
            _stackOfChances[abilityType] = 1;
            _abilities.Add(abilityType, newAbility);
        }


    }
}
