using System.Collections.Generic;

namespace Utils
{
    public static class CollectionExtensions
    {
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary[key];
        }
        
        public static TValue GetValue<TValue>(this TValue[] array, int index)
        {
            return array[index];
        }
    }
}