using SQLite;
using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.Bot.DataBase.SerializableData;

public abstract class DataWithSession
{
    [Ignore]
    public GameSession session { get; private set; }

    public virtual void SetupSession(GameSession _session)
    {
        session = _session;
    }
}
