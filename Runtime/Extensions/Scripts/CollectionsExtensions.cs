using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Nach.Tools.Extensions.Collections
{
    public static class CollectionsExtensions
    {
        public static int GetRandomIndex<T>(this IEnumerable<T> collection)
        {
            int randomIndex = Random.Range(0, collection.Count());

            return randomIndex;
        }

        public static T GetRandomElement<T>(this IEnumerable<T> collection)
        {
            int randomIndex = collection.GetRandomIndex();
            T element = collection.ElementAt(randomIndex);
            return element;
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> collection, bool overwriteRepeatedKeys = false)
        {
            if (collection == null)
            {
                return;
            }

            foreach (var item in collection)
            {
                if (!dictionary.ContainsKey(item.Key))
                {
                    dictionary.Add(item.Key, item.Value);
                }
                else
                {
                    if (overwriteRepeatedKeys)
                    {
                        dictionary[item.Key] = item.Value;
                    }
                }
            }
        }
    }
}