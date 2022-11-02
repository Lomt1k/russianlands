using System;
using System.Collections.Generic;
using System.Text;

namespace TextGameRPG.ViewModels
{
    public interface IGameDataLoader
    {
        public void AddNextState(string stateInfo);
        public void AddInfoToCurrentState(string text);
        public void OnGameDataLoaded();
    }
}
