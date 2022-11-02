using System;

namespace TextGameRPG.ViewModels
{
    public class ConsoleGameDataLoaderViewModel : IGameDataLoader
    {
        public bool isCompleted { get; private set; } = false;

        public ConsoleGameDataLoaderViewModel()
        {
            AddNextState("Application started with console mode");
            Scripts.GameCore.GameDataBase.GameDataBase.instance.LoadAllData(this);
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
            isCompleted = true;
        }
    }
}
