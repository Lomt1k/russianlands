
namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData
{
    public abstract class RawDynamicData<T>
    {
        public abstract void Fill(T data);
        public abstract T Deserialize();
    }
}
