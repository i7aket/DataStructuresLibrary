using System;
using System.Collections;
using System.Collections.Generic;

namespace DataStructuresLibrary;
public class ResizableLinkedListDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private int size;
    private int count;
    private LinkedList<KeyValuePair<TKey, TValue>>[] items;

    public ResizableLinkedListDictionary(int size = 8)
    {
        this.count = 0;
        this.size = size;
        this.items = new LinkedList<KeyValuePair<TKey, TValue>>[size];
    }

    public TValue this[TKey key]
    {
        get { return Get(key); }
        set
        {
            if (ContainsKey(key))
            {
                Update(key, value);
            }
            else
            {
                Add(key, value);
            }
        }
    }

    public ICollection<TKey> Keys => KeysAsList();

    public ICollection<TValue> Values => ValuesAsList();

    public int Count => count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key), "Key cannot be null");
        }

        if (ContainsKey(key))
        {
            throw new ArgumentException("An element with the same key already exists.");
        }

        ResizeIfNeeded();

        int position = GetArrayPosition(key);
        LinkedList<KeyValuePair<TKey, TValue>> linkedList = GetLinkedList(position);
        KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
        linkedList.AddLast(item);
        count++;
    }


    private void ResizeIfNeeded()
    {
        if (count / (double)size > 0.7)
        {
            Resize(size * 2);
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        size = 8;
        count = 0;
        items = new LinkedList<KeyValuePair<TKey, TValue>>[size];
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ContainsKey(item.Key);
    }

    public bool ContainsKey(TKey key)
    {
        int position = GetArrayPosition(key);
        LinkedList<KeyValuePair<TKey, TValue>> linkedList = GetLinkedList(position);

        foreach (var item in linkedList)
        {
            if (item.Key.Equals(key))
            {
                return true;
            }
        }

        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var linkedList in items)
        {
            if (linkedList == null) continue;
            foreach (var kvp in linkedList)
            {
                array[arrayIndex++] = kvp;
            }
        }
    }


    public bool Remove(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key), "Key cannot be null");
        }

        int position = GetArrayPosition(key);
        LinkedList<KeyValuePair<TKey, TValue>> linkedList = GetLinkedList(position);
        bool removed = false;

        KeyValuePair<TKey, TValue> toRemove = default;
        foreach (var item in linkedList)
        {
            if (item.Key.Equals(key))
            {
                toRemove = item;
                removed = true;
                break;
            }
        }

        if (removed)
        {
            linkedList.Remove(toRemove);
            count--;

            if (count / (double)size < 0.3 && size / 2 >= 8)
            {
                Resize(size / 2);
            }

            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int position = GetArrayPosition(key);
        LinkedList<KeyValuePair<TKey, TValue>> linkedList = GetLinkedList(position);

        foreach (var item in linkedList)
        {
            if (item.Key.Equals(key))
            {
                value = item.Value;
                return true;
            }
        }

        value = default(TValue);
        return false;
    }


    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var linkedList in items)
        {
            if (linkedList == null) continue;
            foreach (var kvp in linkedList)
            {
                yield return kvp;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Update(TKey key, TValue value)
    {
        int position = GetArrayPosition(key);
        LinkedList<KeyValuePair<TKey, TValue>> linkedList = GetLinkedList(position);

        foreach (var node in linkedList)
        {
            if (node.Key.Equals(key))
            {
                var updated = new KeyValuePair<TKey, TValue>(key, value);
                linkedList.Find(node).Value = updated;
                return;
            }
        }

        throw new KeyNotFoundException("Key not found");
    }

    public List<TKey> KeysAsList()
    {
        List<TKey> keys = new();
        foreach (var linkedList in items)
        {
            if (linkedList == null) continue;
            foreach (var kvp in linkedList)
            {
                keys.Add(kvp.Key);
            }
        }
        return keys;
    }

    public List<TValue> ValuesAsList()
    {
        List<TValue> values = new();
        foreach (var linkedList in items)
        {
            if (linkedList == null) continue;
            foreach (var kvp in linkedList)
            {
                values.Add(kvp.Value);
            }
        }
        return values;
    }

    private TValue Get(TKey key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key), "Key cannot be null");
        }

        int position = GetArrayPosition(key);
        LinkedList<KeyValuePair<TKey, TValue>> linkedList = GetLinkedList(position);
        foreach (var item in linkedList)
        {
            if (item.Key.Equals(key))
            {
                return item.Value;
            }
        }

        throw new KeyNotFoundException("Key not found");
    }

    private int GetArrayPosition(TKey key)
    {
        int position = key.GetHashCode() % size;
        return Math.Abs(position);
    }

    private LinkedList<KeyValuePair<TKey, TValue>> GetLinkedList(int position)
    {
        LinkedList<KeyValuePair<TKey, TValue>> linkedList = items[position];
        if (linkedList == null)
        {
            linkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
            items[position] = linkedList;
        }

        return linkedList;
    }

    private void Resize(int newSize)
    {
        var oldItems = items;
        items = new LinkedList<KeyValuePair<TKey, TValue>>[newSize];
        size = newSize;
        count = 0;

        foreach (var linkedList in oldItems)
        {
            if (linkedList == null) continue;
            foreach (var kvp in linkedList)
            {
                Add(kvp.Key, kvp.Value);
            }
        }
    }
}

