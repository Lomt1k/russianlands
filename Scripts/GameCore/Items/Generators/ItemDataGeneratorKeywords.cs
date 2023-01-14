using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities.Keywords;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    /* 
     * Keywords от обычных абилок отличаются тем, что мы точно уверены,
     * что на предмете будет использоваться только один keyword
     */
    public abstract partial class ItemDataGeneratorBase
    {
        protected void AddSwordBlockKeyword(DamageInfo blockInfo, byte chancePercentage = 100)
        {
            _statIcons.Add(ItemStatIcon.KeywordSwordBlock);
            var abilityType = AbilityType.SwordBlockEveryTurnKeyword;
            var newAbility = (SwordBlockKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;
            newAbility.physicalDamage = blockInfo[DamageType.Physical];
            newAbility.fireDamage = blockInfo[DamageType.Fire];
            newAbility.coldDamage = blockInfo[DamageType.Cold];            
            newAbility.lightningDamage = blockInfo[DamageType.Lightning];

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddBowLastShotKeyword()
        {
            _statIcons.Add(ItemStatIcon.KeywordBowLastShot);
            var abilityType = AbilityType.BowLastShotKeyword;
            var newAbility = (BowLastShotKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddAddArrowKeyword(byte chancePercentage)
        {
            _statIcons.Add(ItemStatIcon.KeywordAddArrow);
            var abilityType = AbilityType.AddArrowKeyword;
            var newAbility = (AddArrowKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddStealManaKeyword(byte chancePercentage)
        {
            _statIcons.Add(ItemStatIcon.KeywordStealMana);
            var abilityType = AbilityType.StealManaKeyword;
            var newAbility = (StealManaKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddAdditionalFireDamageKeyword(int damage, byte chancePercentage)
        {
            _statIcons.Add(ItemStatIcon.KeywordAdditionalDamage);
            var abilityType = AbilityType.AdditionalFireDamageKeyword;
            var newAbility = (AdditionalFireDamageKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;
            newAbility.damageAmount = damage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddAdditionalColdDamageKeyword(int damage, byte chancePercentage)
        {
            _statIcons.Add(ItemStatIcon.KeywordAdditionalDamage);
            var abilityType = AbilityType.AdditionalColdDamageKeyword;
            var newAbility = (AdditionalColdDamageKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;
            newAbility.damageAmount = damage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddAdditionalLightningDamageKeyword(int damage, byte chancePercentage)
        {
            _statIcons.Add(ItemStatIcon.KeywordAdditionalDamage);
            var abilityType = AbilityType.AdditionalLightningDamageKeyword;
            var newAbility = (AdditionalLightningDamageKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;
            newAbility.damageAmount = damage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddRageKeyword()
        {
            _statIcons.Add(ItemStatIcon.KeywordRage);
            var abilityType = AbilityType.RageKeyword;
            var newAbility = (RageKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddFinishingKeyword(byte bonusDamagePercentage)
        {
            _statIcons.Add(ItemStatIcon.KeywordFinishing);
            var abilityType = AbilityType.FinishingKeyword;
            var newAbility = (FinishingKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.damageBonusPercentage = bonusDamagePercentage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddAbsorptionKeyword(byte chancePercentage)
        {
            _statIcons.Add(ItemStatIcon.KeywordAbsorption);
            var abilityType = AbilityType.AbsorptionKeyword;
            var newAbility = (AbsorptionKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddAddManaKeyword(byte chancePercentage)
        {
            _statIcons.Add(ItemStatIcon.Mana);
            var abilityType = AbilityType.AddManaKeyword;
            var newAbility = (AddManaKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddStunKeyword(byte chancePercentage)
        {
            _statIcons.Add(ItemStatIcon.KeywordStun);
            var abilityType = AbilityType.StunKeyword;
            var newAbility = (StunKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddSanctionsKeyword(byte chancePercentage)
        {
            _statIcons.Add(ItemStatIcon.KeywordSanctions);
            var abilityType = AbilityType.SanctionsKeyword;
            var newAbility = (SanctionsKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;

            _abilities.Add(abilityType, newAbility);
        }


    }
}
