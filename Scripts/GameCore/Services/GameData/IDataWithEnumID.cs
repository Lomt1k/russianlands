using System;

namespace TextGameRPG.Scripts.GameCore.Services.GameData
{
    public interface IDataWithEnumID<T> where T : Enum
    {
        T id { get; }

        void OnSetupAppMode(AppMode appMode);
    }
}
