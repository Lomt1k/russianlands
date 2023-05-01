using TextGameRPG.ViewModels.UserControls;

namespace TextGameRPG.Scripts.GameCore.Services.GameData
{
    public interface IDataWithIntegerID
    {
        string debugName { get; }
        int id { get; }

        void OnSetupAppMode(AppMode appMode);
    }
}
