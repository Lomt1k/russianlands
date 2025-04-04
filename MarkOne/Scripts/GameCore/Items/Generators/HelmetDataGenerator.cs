﻿using System;

namespace MarkOne.Scripts.GameCore.Items.Generators;

public class HelmetDataGenerator : ItemDataGeneratorBase
{
    public HelmetDataGenerator(ItemDataSeed _seed) : base(_seed)
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
        AddProperties();
    }

    private void AddBaseParameters(float rarityMult)
    {
        var physicalDamage = (int)Math.Round(rarityMult * gradedPoints / 2);
        AddPhysicalDamageResist(physicalDamage);

        var secondaryDamage = (int)Math.Round(physicalDamage * 0.8f);
        foreach (var param in seed.baseParameters)
        {
            switch (param)
            {
                case "DF":
                    AddFireDamageResist(secondaryDamage);
                    break;
                case "DC":
                    AddColdDamageResist(secondaryDamage);
                    break;
                case "DL":
                    AddLightningDamageResist(secondaryDamage);
                    break;
            }
        }
    }


}
