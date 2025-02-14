using XLua;

namespace Utils
{
    public static class XLuaExtensions
    {
        public static bool TryGet<TValue>(this LuaTable lua, string key, out TValue value)
        {
            if (lua.ContainsKey(key))
            {
                value = lua.Get<TValue>(key);
                return true;
            }
            value = default;
            return false;
        }
        
        public static bool TryGet<TKey, TValue>(this LuaTable lua, TKey key, out TValue value)
        {
            if (lua.ContainsKey(key))
            {
                value = lua.Get<TKey, TValue>(key);
                return true;
            }
            value = default;
            return false;
        }

        public static TValue GetOrDefault<TValue>(this LuaTable lua, string key, TValue defaultValue = default)
        {
            return lua.ContainsKey(key) ? lua.Get<TValue>(key) : defaultValue;
        }
        
        public static TValue GetOrDefault<TKey, TValue>(this LuaTable lua, TKey key, TValue defaultValue = default)
        {
            return lua.ContainsKey(key) ? lua.Get<TKey, TValue>(key) : defaultValue;
        }
    }
}