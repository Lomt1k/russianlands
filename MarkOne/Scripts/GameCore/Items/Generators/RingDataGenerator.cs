﻿using System;
using MarkOne.Scripts.GameCore.Items.ItemAbilities;
using MarkOne.Scripts.GameCore.Items.ItemProperties;

namespace MarkOne.Scripts.GameCore.Items.Generators;

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
        var damageResist = (int)Math.Round(gradedPoints * 0.15);
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
                AddIncreaseMaxHealth((int)Math.Round(gradedPoints * 0.5f));
                break;
        }
    }

    protected override void AddAbility(AbilityType abilityType)
    {
        switch (abilityType)
        {
            case AbilityType.RestoreHealthEveryTurn:
                AddRestoreHealthEveryTurn((int)Math.Round(gradedPoints * 0.5), 17);
                break;
            case AbilityType.AddManaEveryTurn:
                var chance = (byte)(10 + Math.Round(seed.requiredLevel / 5f));
                AddManaEveryTurn(1, chance);
                break;
        }
    }



}
