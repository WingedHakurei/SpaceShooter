using System;
using GamePlay.Configs;
using UnityEngine;

namespace GamePlay.Runtimes
{
    public class FighterRuntime
    {
        public Fighter config;
        public Guid guid;
        public int team;
        public Vector2 position;
        public Vector2 targetPosition;
        public float[] cds;
        public int curHp;
    }
}