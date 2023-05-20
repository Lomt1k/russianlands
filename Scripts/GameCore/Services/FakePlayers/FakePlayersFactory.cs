using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Items.Generators;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Skills;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkOne.Scripts.GameCore.Services.FakePlayers;

public sealed class FakePlayersFactory : Service
{
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    public FakePlayer Generate(FakePlayerGenerationSettings settings)
    {
        var random = new Random();
        var playerLevel = GeneratePlayerLevel(random, settings);
        var items = GeneratePlayerItems(random, settings, playerLevel);
        var skills = GeneratePlayerSkillls(random, settings);
        var nickname = GenerateRandomName(random);
        var isPremium = false;
        return new FakePlayer(items, skills, playerLevel, nickname, isPremium);
    }

    private byte GeneratePlayerLevel(Random random, FakePlayerGenerationSettings settings)
    {
        return (byte)random.Next(settings.minPlayerLevel, settings.maxPlayerLevel + 1);
    }

    private IEnumerable<InventoryItem> GeneratePlayerItems(Random random, FakePlayerGenerationSettings settings, byte playerLevel)
    {
        var availableHallGradePairs = ItemGenerationHelper.CalculateAvailableHallGradePairs(settings.minItemLevel, playerLevel).ToArray();

        yield return GenerateItem(random, settings, availableHallGradePairs, ItemType.Sword);
        yield return GenerateItem(random, settings, availableHallGradePairs, ItemType.Bow);
        yield return GenerateItem(random, settings, availableHallGradePairs, ItemType.Stick);
        yield return GenerateItem(random, settings, availableHallGradePairs, ItemType.Helmet);
        yield return GenerateItem(random, settings, availableHallGradePairs, ItemType.Armor);
        yield return GenerateItem(random, settings, availableHallGradePairs, ItemType.Boots);
        yield return GenerateItem(random, settings, availableHallGradePairs, ItemType.Shield);
        yield return GenerateItem(random, settings, availableHallGradePairs, ItemType.Scroll);
        yield return GenerateItem(random, settings, availableHallGradePairs, ItemType.Scroll);
        yield return GenerateItem(random, settings, availableHallGradePairs, ItemType.Scroll);
    }



    private InventoryItem GenerateItem(Random random, FakePlayerGenerationSettings settings, HallGradePair[] hallGradePairs, ItemType itemType)
    {
        var rarity = ((WeightedRarity)settings.itemRarities.GetRandom(random)).rarity;
        var hallGradePair = hallGradePairs[random.Next(hallGradePairs.Length)].EnsureNotNull();
        return ItemGenerationManager.GenerateItem(hallGradePair.townhall, itemType, rarity, hallGradePair.grade);
    }

    private Dictionary<ItemType, byte> GeneratePlayerSkillls(Random random, FakePlayerGenerationSettings settings)
    {
        var skills = new Dictionary<ItemType, byte>();
        foreach (var skillType in PlayerSkills.GetAllSkillTypes())
        {
            skills[skillType] = (byte)random.Next(settings.minSkillLevel, settings.maxSkillLevel + 1);
        }
        return skills;
    }

    private string GenerateRandomName(Random random)
    {
        var botnames = gameDataHolder.botnames;
        return botnames[random.Next(botnames.Count)];
    }

}
