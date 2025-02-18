using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GamePlay.Runtimes;
using XLua;

namespace Editor.XLua
{
    public static class XLuaConfig
    {
        [CSharpCallLua] public static IEnumerable<Type> CSharpCallLua = new Type[]
        {
            typeof(Predicate<RuntimeBase>),
            typeof(Action<RuntimeBase>),
        };
        [LuaCallCSharp] public static IEnumerable<Type> LuaCallCSharp
        {
            get
            {
                var basicTypes = new Type[]
                {
                    typeof(Action<string, RuntimeBase>)
                };
    
                var namespaces = new string[]
                {
                    nameof(GamePlay.Configs),
                    nameof(GamePlay.Runtimes)
                };
                var customTypes = Assembly.Load("Assembly-CSharp").GetExportedTypes()
                    .Where(t => namespaces.Contains(t.Namespace));
    
                return basicTypes.Concat(customTypes);
            }
        }
    }
}