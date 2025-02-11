using UnityEngine;

namespace GamePlay.Configs
{
    public struct Command
    {
        public CommandType type;
        public Vector2 position;
    }
    
    public enum CommandType
    {
        None,
        Move,
        Attack
    }

    
    
}