namespace MarkOne.Scripts.GameCore.Services.GameData;

public interface IGameDataWithId<T>
{
    T id { get; }
    void OnBotAppStarted();
}