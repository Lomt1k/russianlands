﻿using System;
using System.Linq;

namespace MarkOne.Scripts.GameCore.Items.Generators;

public class ScrollDataGenerator : ItemDataGeneratorBase
{
    public ScrollDataGenerator(ItemDataSeed _seed) : base(_seed)
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
    }

    private void AddBaseParameters(float rarityMult)
    {
        var parameter = seed.baseParameters[0];
        switch (parameter)
        {
            case "DF":
            case "DC":
            case "DL":
                var manaCostMult = GetManaMultForDamageSpell();
                var generalDamage = Math.Round(gradedPoints * manaCostMult * rarityMult);
                var minDamage = (int)Math.Round(generalDamage * 0.87f);
                var maxDamage = (int)Math.Round(generalDamage * 1.13f);
                AddDealDamageSpell(parameter, minDamage, maxDamage);
                break;
        }

        var addedAbility = _abilities.Values.First();
        addedAbility.manaCost = seed.manaCost;
    }

    private float GetManaMultForDamageSpell()
    {
        switch (seed.manaCost)
        {
            case 2: return 2.5f;
            case 3: return 3.25f;
            case 4: return 4f;
            case 5: return 5f;
        }
        return 0f;
    }

    private void AddDealDamageSpell(string type, int minDamage, int maxDamage)
    {
        switch (type)
        {
            case "DF":
                AddDealFireDamage(minDamage, maxDamage);
                break;
            case "DC":
                AddDealColdDamage(minDamage, maxDamage);
                break;
            case "DL":
                AddDealLightningDamage(minDamage, maxDamage);
                break;
        }
    }


}
