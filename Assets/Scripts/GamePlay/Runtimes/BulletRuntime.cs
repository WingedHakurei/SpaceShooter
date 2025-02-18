using System;
using GamePlay.Configs;
using UnityEngine;

namespace GamePlay.Runtimes
{
    public class BulletRuntime : RuntimeBase
    {
        public Bullet config;
        public Guid guid;
        public int team;
        public Vector2 position;
        public Vector2 direction;
        public FighterRuntime shotBy;
    }
}