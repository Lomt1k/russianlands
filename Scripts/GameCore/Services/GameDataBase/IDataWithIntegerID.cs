namespace TextGameRPG.Scripts.GameCore.Services.GameDataBase
{
    public interface IDataWithIntegerID
    {
        string debugName { get; }
        int id { get; }

        void OnSetupAppMode(AppMode appMode);
    }
}
