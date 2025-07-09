using System.Collections;
using System.Collections.Generic;

namespace ProjectCode1.Utils
{
    public class FastList<T> : IEnumerable<T>
    {
        readonly List<T> items;
        readonly Dictionary<T, int> dict;

        public int Count { get => items.Count; }

        public FastList()
        {
            items = new();
            dict = new();
        }

        public void Add(T item)
        {
            if (dict.ContainsKey(item))
                return;

            int index = items.Count;
            items.Add(item);
            dict[item] = index;
        }

        public void Remove(T item)
        {
            if (!dict.TryGetValue(item, out int index))
                return;

            dict.Remove(item);

            int last = items.Count - 1;
            if (index == last)
            {
                items.RemoveAt(last);
            }
            else
            {
                T lastItem = items[last];
                items[index] = lastItem;
                items.RemoveAt(last);

                dict[lastItem] = index;
            }
        }

        public bool Contains(T item) => dict.ContainsKey(item);

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}