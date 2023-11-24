using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MarkOne.Scripts.Utils;
public class LockedDictionary<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary = new();
    private readonly object _lock = new();

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _dictionary.Count;
            }
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            lock (_lock)
            {
                return _dictionary[key];
            }
        }
        set
        {
            lock (_lock)
            {
                _dictionary[key] = value;
            }
        }
    }

    public IEnumerable<TKey> Keys
    {
        get
        {
            lock (_lock)
            {
                return _dictionary.Keys;
            }
        }
    }

    public IEnumerable<TValue> Values
    {
        get
        {
            lock (_lock)
            {
                return _dictionary.Values;
            }
        }
    }

    public void Add(TKey key, TValue value)
    {
        lock (_lock)
        {
            _dictionary.Add(key, value);
        }
    }

    public void Clear()
    {
        lock (_lock) 
        {
            _dictionary.Clear();
        }
    }

    public bool ContainsKey(TKey key)
    {
        lock (_lock)
        {
            return _dictionary.ContainsKey(key);
        }
    }

    public bool Remove(TKey key)
    {
        lock (_lock)
        {
            return _dictionary.Remove(key);
        }
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        lock (_lock)
        {
            return _dictionary.TryGetValue(key, out value);
        }
    }

}
