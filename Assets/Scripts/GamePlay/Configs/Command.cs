using System;
using UnityEngine;
using Utils;
using XLua;

namespace GamePlay.Configs
{
    public struct Command
    {
        public CommandType type;
        public Vector2 position;
        public float cd;
        public int next;

        public Command(LuaTable lua)
        {
            type = Enum.Parse<CommandType>(lua.Get<string>(nameof(type)));
            if (type == CommandType.Move)
            {
                var luaPosition = lua.Get<LuaTable>(nameof(position));
                position = new Vector2(luaPosition.Get<int, float>(1), luaPosition.Get<int, float>(2));
            }
            else
            {
                position = Vector2.zero;
            } 
            cd = lua.GetOrDefault<float>(nameof(cd));
            next = lua.Get<int>(nameof(next));
        }
    }
    
    public enum CommandType
    {
        None,
        Move,
        Attack
    }

    
    
}