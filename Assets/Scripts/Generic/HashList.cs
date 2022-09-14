using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UD.Generic
{

    public class HashList<TKey, TValue> : IEnumerable<TKey>
    {
        private readonly Dictionary<TKey, TValue> dict;
        private readonly List<TKey> list;

        public int Count
        {
            get { return dict.Count; }
        }

        public HashList(int capacity = 4)
        {
            dict = new Dictionary<TKey, TValue>(capacity);
            list = new List<TKey>(capacity);
        }

        public TValue this[int index]
        {
            get
            {
                if (index >= list.Count)
                {
                    Debug.LogError($"Hash list is get fail, {index} is out of range");
                    return default;
                }

                return dict[list[index]];
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!dict.TryGetValue(key, out var value))
                {
                    Debug.LogError($"Hash list is get fail, {value} is not exist");
                    return default;
                }

                return value;
            }
            set
            {
                dict[key] = value;
                var index = list.IndexOf(key);
                if (index > -1)
                {
                    list.RemoveAt(index);
                }

                list.Add(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dict.TryGetValue(key, out value);
        }

        public void Add(TKey key, TValue value)
        {
            if (value == null)
            {
                Debug.LogError("Hash list is add fail, type is null");
                return;
            }


            if (dict.ContainsKey(key))
            {
                Debug.LogError($"Hash list is add fail, {value} is already exist");
                return;
            }

            dict.Add(key, value);
            list.Add(key);
        }

        public bool Remove(TKey key)
        {
            if (!dict.Remove(key))
            {
                return false;
            }

            return list.Remove(key);
        }

        public void Clear()
        {
            list.Clear();
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}