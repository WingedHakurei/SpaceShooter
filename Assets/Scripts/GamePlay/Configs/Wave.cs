using System;
using XLua;

namespace GamePlay.Configs
{
    public class Wave
    {
        public Fighter[] fighters;
        public Behavior[] behaviors;
        public float[] intervals;

        public Wave(LuaTable lua, Func<string, Fighter> nameToFighter, Func<int, Behavior> idToBehavior)
        {
            var luaFighters = lua.Get<LuaTable>(nameof(fighters));
            fighters = new Fighter[luaFighters.Length];
            for (var i = 0; i < fighters.Length; i++)
            {
                fighters[i] = nameToFighter(luaFighters.Get<int, string>(i + 1));
            }
            var luaBehaviors = lua.Get<LuaTable>(nameof(behaviors));
            behaviors = new Behavior[luaBehaviors.Length];
            for (var i = 0; i < behaviors.Length; i++)
            {
                behaviors[i] = idToBehavior(luaBehaviors.Get<int, int>(i + 1));
            }
            var luaIntervals = lua.Get<LuaTable>(nameof(intervals));
            intervals = new float[luaIntervals.Length];
            for (var i = 0; i < intervals.Length; i++)
            {
                intervals[i] = luaIntervals.Get<int, float>(i + 1);
            }
        }
    }
}