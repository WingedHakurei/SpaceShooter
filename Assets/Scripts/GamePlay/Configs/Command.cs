using UnityEngine;

namespace GamePlay.Configs
{
    public struct Command
    {
        public CommandType type;
        public Vector2 position;
        public float cd;
        public int next;
    }
    
    public enum CommandType
    {
        None,
        Move,
        Attack
    }

    
    
}