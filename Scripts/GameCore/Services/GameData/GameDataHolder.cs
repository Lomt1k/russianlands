﻿using System;
using System.IO;
using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Locations;
using MarkOne.Scripts.GameCore.Potions;
using MarkOne.Scripts.GameCore.Quests;
using MarkOne.Scripts.GameCore.Units.Mobs;
using MarkOne.ViewModels;

namespace MarkOne.Scripts.GameCore.Services.GameData;

public class GameDataHolder : Service
{
#pragma warning disable CS8618

    private static readonly string gameDataPath = Path.Combine("Assets", "gameData");

    private GameDataLoader? _loader;

    public GameDataDictionary<BuildingId, BuildingData> buildings { get; private set; }
    public GameDataDictionary<int, ItemData> items { get; private set; }
    public GameDataDictionary<int, QuestMobData> mobs { get; private set; }
    public GameDataDictionary<int, PotionData> potions { get; private set; }
    public GameDataDictionary<QuestId, QuestData> quests { get; private set; }
    public GameDataDictionary<LocationId, LocationMobSettingsData> locationGeneratedMobs { get; private set; }
    public GameDataDictionary<LeagueId, ArenaLeagueSettings> arenaLeagueSettings { get; private set; }

#pragma warning restore CS8618

    public event Action? onDataReloaded;

    public void LoadAllData(GameDataLoader? loader = null)
    {
        _loader = loader;

        if (!Directory.Exists(gameDataPath))
        {
            Directory.CreateDirectory(gameDataPath);
            _loader?.AddNextState("'gameData' folder not found in Assets! Creating new gameData...");
        }

        buildings = LoadGameDataDictionary<BuildingId, BuildingData>("buildings");
        items = LoadGameDataDictionary<int, ItemData>("items");
        mobs = LoadGameDataDictionary<int, QuestMobData>("mobs");
        potions = LoadGameDataDictionary<int, PotionData>("potions");
        quests = LoadGameDataDictionary<QuestId, QuestData>("quests");
        locationGeneratedMobs = LoadGameDataDictionary<LocationId, LocationMobSettingsData>("locationGeneratedMobs");
        arenaLeagueSettings = LoadGameDataDictionary<LeagueId, ArenaLeagueSettings>("arenaLeagueSettings");

        Localizations.Localization.LoadAll(_loader, gameDataPath);

        _loader?.OnGameDataLoaded();
        onDataReloaded?.Invoke();
    }

    private GameDataDictionary<TId, TData> LoadGameDataDictionary<TId, TData>(string fileName) where TData : IGameDataWithId<TId>
    {
        _loader?.AddNextState($"Loading {fileName}...");
        var fullPath = Path.Combine(gameDataPath, fileName + ".json");
        var dataBase = GameDataDictionary<TId, TData>.LoadFromJSON<TId, TData>(fullPath);
        _loader?.AddInfoToCurrentState(dataBase.count.ToString());
        return dataBase;
    }

    public void SaveAllData()
    {
        if (Program.appMode != AppMode.Editor)
            throw new InvalidOperationException("Game data can only be changed in Editor mode");

        buildings.Save();
        items.Save();
        mobs.Save();
        potions.Save();
        quests.Save();
        locationGeneratedMobs.Save();
        arenaLeagueSettings.Save();
    }

}
