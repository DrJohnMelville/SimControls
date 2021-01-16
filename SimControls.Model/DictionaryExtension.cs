using System;
using System.Collections.Generic;

namespace SimControls.Model
{
    public static class DictionaryExtension
    {
        public static TValue GetOrCreate<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> creator)
        {
            TValue ret;
            if (!dict.TryGetValue(key, out ret))
            {
                ret = creator();
                dict.Add(key, ret);
            }
            return ret;
        }
    }
}