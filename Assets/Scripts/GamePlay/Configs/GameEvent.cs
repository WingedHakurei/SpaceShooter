using System;
using GamePlay.Runtimes;
using XLua;

namespace GamePlay.Events
{
    public class GameEvent: IDisposable
    {
        public Predicate<RuntimeBase> condition;
        public Action<RuntimeBase> effect;

        public GameEvent(LuaTable lua)
        {
            condition = lua.Get<Predicate<RuntimeBase>>(nameof(condition));
            effect = lua.Get<Action<RuntimeBase>>(nameof(effect));
        }

        public void Dispose()
        {
            condition = null;
            effect = null;
        }
    }
}