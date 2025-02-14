using UnityEngine;
using XLua;

namespace GamePlay.Configs
{
    public class Behavior
    {
        public Vector2 startPosition;
        public Command[] commands;

        public Behavior(LuaTable lua)
        {
            var luaStartPosition = lua.Get<LuaTable>(nameof(startPosition));
            startPosition = new Vector2(luaStartPosition.Get<int, float>(1), luaStartPosition.Get<int, float>(2));
            var luaCommands = lua.Get<LuaTable>(nameof(commands));
            commands = new Command[luaCommands.Length];
            for (var i = 0; i < commands.Length; i++)
            {
                commands[i] = new Command(luaCommands.Get<int, LuaTable>(i + 1));
            }
        }
    }
}