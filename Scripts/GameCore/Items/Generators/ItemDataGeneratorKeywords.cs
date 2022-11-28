using TextGameRPG.Scripts.Bot;
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
        protected void AddSwordBlockKeyword(DamageInfo blockInfo, float chancePercentage = 100f)
        {
            _statIcons.Add(Stat.KeywordSwordBlock);
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
            _statIcons.Add(Stat.KeywordBowLastShot);
            var abilityType = AbilityType.BowLastShotKeyword;
            var newAbility = (BowLastShotKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddAddArrowKeyword(float chancePercentage)
        {
            _statIcons.Add(Stat.KeywordAddArrow);
            var abilityType = AbilityType.AddArrowKeyword;
            var newAbility = (AddArrowKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddStealManaKeyword(float chancePercentage)
        {
            _statIcons.Add(Stat.KeywordStealMana);
            var abilityType = AbilityType.StealManaKeyword;
            var newAbility = (StealManaKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddAdditionalFireDamageKeyword(int damage, float chancePercentage)
        {
            _statIcons.Add(Stat.KeywordAdditionalDamage);
            var abilityType = AbilityType.AdditionalFireDamageKeyword;
            var newAbility = (AdditionalFireDamageKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;
            newAbility.damageAmount = damage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddAdditionalColdDamageKeyword(int damage, float chancePercentage)
        {
            _statIcons.Add(Stat.KeywordAdditionalDamage);
            var abilityType = AbilityType.AdditionalColdDamageKeyword;
            var newAbility = (AdditionalColdDamageKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;
            newAbility.damageAmount = damage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddAdditionalLightningDamageKeyword(int damage, float chancePercentage)
        {
            _statIcons.Add(Stat.KeywordAdditionalDamage);
            var abilityType = AbilityType.AdditionalLightningDamageKeyword;
            var newAbility = (AdditionalLightningDamageKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;
            newAbility.damageAmount = damage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddRageKeyword()
        {
            _statIcons.Add(Stat.KeywordRage);
            var abilityType = AbilityType.RageKeyword;
            var newAbility = (RageKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);
            _abilities.Add(abilityType, newAbility);
        }

        protected void AddFinishingKeyword(byte bonusDamagePercentage)
        {
            _statIcons.Add(Stat.KeywordFinishing);
            var abilityType = AbilityType.FinishingKeyword;
            var newAbility = (FinishingKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.damageBonusPercentage = bonusDamagePercentage;

            _abilities.Add(abilityType, newAbility);
        }

        protected void AddAbsorptionKeyword(float chancePercentage)
        {
            _statIcons.Add(Stat.KeywordAbsorption);
            var abilityType = AbilityType.AbsorptionKeyword;
            var newAbility = (AbsorptionKeywordAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;

            _abilities.Add(abilityType, newAbility);
        }


    }
}
