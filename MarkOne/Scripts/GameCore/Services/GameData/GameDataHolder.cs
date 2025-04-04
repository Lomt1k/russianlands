﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.DailyBonus;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Locations;
using MarkOne.Scripts.GameCore.Potions;
using MarkOne.Scripts.GameCore.Quests;
using MarkOne.Scripts.GameCore.ReferralSystem;
using MarkOne.Scripts.GameCore.Shop;
using MarkOne.Scripts.GameCore.Shop.Offers;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Units.Mobs;

namespace MarkOne.Scripts.GameCore.Services.GameData;

public class GameDataHolder : Service
{
#pragma warning disable CS8618

    public static string gameDataPath { get; private set; } = string.Empty;

    public GameDataDictionary<BuildingId, BuildingData> buildings { get; private set; }
    public GameDataDictionary<int, ItemData> items { get; private set; }
    public GameDataDictionary<int, QuestMobData> mobs { get; private set; }
    public GameDataDictionary<int, PotionData> potions { get; private set; }
    public GameDataDictionary<QuestId, QuestData> quests { get; private set; }
    public GameDataDictionary<LocationId, LocationMobSettingsData> locationGeneratedMobs { get; private set; }
    public ArenaSettings arenaSettings { get; private set; }
    public GameDataDictionary<LeagueId, ArenaLeagueSettings> arenaLeagueSettings { get; private set; }
    public GameDataDictionary<byte,ArenaShopSettings> arenaShopSettings { get; private set; }
    public GameDataDictionary<byte, ShopSettings> shopSettings { get; private set; }
    public GameDataDictionary<byte, DailyBonusData> dailyBonuses { get; private set; }
    public GameDataDictionary<byte, ReferralBonusData> referralBonuses { get; private set; }
    public GameDataDictionary<int, OfferData> offers { get; private set; }
    public GameDataDictionary<int, FakePlayerGenerationData> fakePlayers { get; private set; }

    // cache
    public Dictionary<string, ShopItemBase> shopItemsCache { get; private set; } = new();
    public OfferData[] offersOrderedByPriority { get; private set; } = Array.Empty<OfferData>();

#pragma warning restore CS8618

    public event Action? onDataReloaded;

    public void LoadAllData()
    {
        gameDataPath = FindGameDataPath();
        if (gameDataPath is null)
        {
            throw new DirectoryNotFoundException("Folder 'gameData' not found");
        }
        if (!Directory.Exists(gameDataPath))
        {
            Directory.CreateDirectory(gameDataPath);
            Console.WriteLine("\n'gameData' folder not found in Assets! Creating new gameData...");
        }

        buildings = LoadGameDataDictionary<BuildingId, BuildingData>("buildings");
        items = LoadGameDataDictionary<int, ItemData>("items");
        mobs = LoadGameDataDictionary<int, QuestMobData>("mobs");
        potions = LoadGameDataDictionary<int, PotionData>("potions");
        quests = LoadGameDataDictionary<QuestId, QuestData>("quests");
        locationGeneratedMobs = LoadGameDataDictionary<LocationId, LocationMobSettingsData>("locationGeneratedMobs");
        arenaSettings = LoadGameData<ArenaSettings>("arenaSettings");
        arenaLeagueSettings = LoadGameDataDictionary<LeagueId, ArenaLeagueSettings>("arenaLeagueSettings");
        arenaShopSettings = LoadGameDataDictionary<byte, ArenaShopSettings>("arenaShopSettings");
        shopSettings = LoadGameDataDictionary<byte, ShopSettings>("shopSettings");
        dailyBonuses = LoadGameDataDictionary<byte, DailyBonusData>("dailyBonuses");
        referralBonuses = LoadGameDataDictionary<byte, ReferralBonusData>("referralBonuses");
        offers = LoadGameDataDictionary<int, OfferData>("offers");
        fakePlayers = LoadGameDataDictionary<int, FakePlayerGenerationData>("fakePlayers");

        RefreshShopItemsCache();
        offersOrderedByPriority = offers.GetAllData().OrderByDescending(x => x.priority).ToArray();

        Localizations.Localization.LoadAll(gameDataPath);

        Console.WriteLine("Game data loaded");
        onDataReloaded?.Invoke();
    }

    private string? FindGameDataPath()
    {
        var gameDataPostfix = "GameData";
        var fromAssets = Path.Combine("Assets", gameDataPostfix);
        if (Directory.Exists(fromAssets))
        {
            return fromAssets;
        }
        if (Directory.Exists(gameDataPostfix))
        {
            return gameDataPostfix;
        }

        var defaultPath = string.Empty;
        for (int i = 0; i < 10; i++)
        {
            defaultPath = Path.Combine(defaultPath, "..");
            var pathWithGameData = Path.Combine(defaultPath, gameDataPostfix);
            if (Directory.Exists(pathWithGameData))
            {
                return pathWithGameData;
            }
        }
        return null;
    }

    private GameDataDictionary<TId, TData> LoadGameDataDictionary<TId, TData>(string fileName) where TData : IGameDataWithId<TId>
    {
        Console.Write($"Loading {fileName}... ");
        var fullPath = Path.Combine(gameDataPath, fileName + ".json");
        var dataBase = GameDataDictionary<TId, TData>.LoadFromJSON<TId, TData>(fullPath);
        Console.WriteLine(dataBase.count);
        return dataBase;
    }

    private T LoadGameData<T>(string fileName) where T : GameData
    {
        Console.WriteLine($"Loading {fileName}... ");
        var fullPath = Path.Combine(gameDataPath, fileName + ".json");
        var loadedObject = GameData.LoadFromJSON<T>(fullPath);
        return loadedObject;
    }

    private void RefreshShopItemsCache()
    {
        shopItemsCache.Clear();
        foreach (var settings in shopSettings.GetAllData())
        {
            foreach (var item in settings.premiumCategoryItems)
            {
                shopItemsCache.Add(item.vendorCode, item);
            }
            foreach (var item in settings.lootboxCategoryItems)
            {
                shopItemsCache.Add(item.vendorCode, item);
            }
            foreach (var item in settings.diamondsCategoryItems)
            {
                shopItemsCache.Add(item.vendorCode, item);
            }
        }

        // offers
        foreach (var offerData in offers.GetAllData())
        {
            var shopItem = offerData.GenerateShopItem();
            shopItemsCache.Add(shopItem.vendorCode, shopItem);
        }
    }

    public void SaveAllData()
    {
        if (Program.isBotAppStarted)
        {
            throw new InvalidOperationException("Game data can be changed only in Game Data Editor");
        }

        buildings.Save();
        items.Save();
        mobs.Save();
        potions.Save();
        quests.Save();
        locationGeneratedMobs.Save();
        arenaSettings.Save();
        arenaLeagueSettings.Save();
        arenaShopSettings.Save();
        shopSettings.Save();
        dailyBonuses.Save();
        referralBonuses.Save();
        offers.Save();
        fakePlayers.Save();
    }

}
