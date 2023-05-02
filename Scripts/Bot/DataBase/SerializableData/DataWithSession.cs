using SQLite;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData;

public abstract class DataWithSession
{
    [Ignore]
    public GameSession session { get; private set; }

    public virtual void SetupSession(GameSession _session)
    {
        session = _session;
    }
}
