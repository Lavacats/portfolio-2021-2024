using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    public Dictionary<TKey, TValue> ToDictionary()
    {
        dictionary.Clear();
        for (int i = 0; i < keys.Count; i++)
        {
            dictionary.Add(keys[i], values[i]);
        }
        return dictionary;
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var pair in dictionary)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary.Clear();
        for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
        {
            dictionary.Add(keys[i], values[i]);
        }
    }

    public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

    public void Add(TKey key, TValue value) => dictionary.Add(key, value);

    public bool Remove(TKey key) => dictionary.Remove(key);

    public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

    public TValue this[TKey key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }

    public int Count => dictionary.Count;
}