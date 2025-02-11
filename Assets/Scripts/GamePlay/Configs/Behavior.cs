using UnityEngine;

namespace GamePlay.Configs
{
    public class Behavior
    {
        public Vector2 startPosition;
        public Command[] commands;
        public int[] nextIndices;
        public float[] intervals;
    }
}