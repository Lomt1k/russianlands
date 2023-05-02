using SQLite;
using System;
using System.Threading.Tasks;

public static class SQLiteExtensions
{
    public static async Task<T> GetOrNullAsync<T>(this SQLiteAsyncConnection db, object pk) where T : new()
    {
        try
        {
            return await db.GetAsync<T>(pk).FastAwait();
        }
        catch (InvalidOperationException)
        {
            return default(T);
        }
    }

}
