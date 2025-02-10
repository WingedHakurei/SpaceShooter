using System.Collections.Generic;

namespace GamePlay.Entities
{
    public class GamePlayEntities
    {
        public Dictionary<int, FighterEntity> fighters = new();
        public Dictionary<int, BulletEntity> bullets = new();
    }
}