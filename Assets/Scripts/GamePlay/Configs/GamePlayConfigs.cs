using UnityEngine;

namespace GamePlay.Configs
{
    public class GamePlayConfigs
    {
        public const int MaxGameSpeed = 5;
        public int gameSpeed;
        public float gameTime;
        public Fighter player;
        public Vector2 playerPosition;
        public Fighter[] fighters;
        public Weapon[] weapons;
        public Bullet[] bullets;
        public Stage[] stages;
        public Wave[] waves;
        public Behavior[] behaviors;
    }
}