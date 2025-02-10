using System;
using System.Collections.Generic;

namespace GamePlay.Entities
{
    public class GamePlayEntities
    {
        public Dictionary<Guid, FighterEntity> fighters = FighterEntity.All;
        public Dictionary<Guid, BulletEntity> bullets = BulletEntity.All;
    }
}