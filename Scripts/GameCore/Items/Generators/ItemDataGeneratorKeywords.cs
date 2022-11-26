using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;

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
            var newAbility = (AddArrowAbility)ItemAbilityRegistry.GetNewAbility(abilityType);

            newAbility.chanceToSuccessPercentage = chancePercentage;

            _abilities.Add(abilityType, newAbility);
        }


    }
}
