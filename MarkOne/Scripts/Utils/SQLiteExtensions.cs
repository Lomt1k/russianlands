using SQLite;
using System;

public static class SQLiteExtensions
{
    public static T GetOrNull<T>(this SQLiteConnection db, object pk) where T : new()
    {
        try
        {
            return db.Get<T>(pk);
        }
        catch (InvalidOperationException)
        {
            return default;
        }
    }

}
