using System;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.GameData;

namespace TextGameRPG.ViewModels;

public class GameDataLoader
{
    private static readonly GameDataHolder gameDataBase = Services.Get<GameDataHolder>();

    public bool isCompleted { get; private set; } = false;

    public GameDataLoader()
    {
    }

    public void Load()
    {
        AddNextState("Start loading game data...");
        gameDataBase.LoadAllData(this);
    }

    public void AddInfoToCurrentState(string text)
    {
        Console.Write(' ' + text);
    }

    public void AddNextState(string stateInfo)
    {
        Console.WriteLine();
        Console.Write(stateInfo);
    }

    public void OnGameDataLoaded()
    {
        AddNextState("All Game Data successfully loaded");
        Console.WriteLine();
        isCompleted = true;
    }
}
