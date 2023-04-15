using System;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.GameDataBase;

namespace TextGameRPG.ViewModels
{
    public class ConsoleGameDataLoaderViewModel : IGameDataLoader
    {
        private static readonly GameDataBase gameDataBase = Services.Get<GameDataBase>();

        public bool isCompleted { get; private set; } = false;

        public ConsoleGameDataLoaderViewModel()
        {
            AddNextState("Application started with console mode");
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
}
