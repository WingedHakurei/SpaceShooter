using System;
using XLua;

namespace GamePlay.Configs
{
    public class Stage
    {
        public Wave[] waves;
        public float[] intervals;

        public Stage(LuaTable lua, Func<int, Wave> idToWave)
        {
            var luaWaves = lua.Get<LuaTable>(nameof(waves));
            waves = new Wave[luaWaves.Length];
            for (var i = 0; i < waves.Length; i++)
            {
                waves[i] = idToWave(luaWaves.Get<int, int>(i + 1));
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