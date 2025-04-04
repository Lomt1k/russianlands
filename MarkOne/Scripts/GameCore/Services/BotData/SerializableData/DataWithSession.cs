﻿using SQLite;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

public abstract class DataWithSession
{
    [Ignore]
    public GameSession session { get; private set; }

    public virtual void SetupSession(GameSession _session)
    {
        session = _session;
    }
}
