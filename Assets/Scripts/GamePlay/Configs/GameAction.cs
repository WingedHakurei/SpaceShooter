using XLua;

namespace GamePlay.Events
{
    public class GameAction
    {
        public string[] events;

        public GameAction(LuaTable lua)
        {
            var luaEvents = lua.Get<LuaTable>(nameof(events));
            events = new string[luaEvents.Length];
            for (var i = 0; i < events.Length; i++)
            {
                events[i] = luaEvents.Get<int, string>(i + 1);
            }
        }
    }
}