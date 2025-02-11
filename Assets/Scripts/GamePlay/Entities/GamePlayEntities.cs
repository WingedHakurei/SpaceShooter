using System;
using System.Collections.Generic;

namespace GamePlay.Entities
{
    public class GamePlayEntities
    {
        public readonly Dictionary<Guid, FighterEntity> fighters = new();
        public readonly Dictionary<Guid, BulletEntity> bullets = new();

        public GamePlayEntities()
        {
            FighterEntity.OnInitialized += fighters.Add;
            FighterEntity.OnDestroy += guid => fighters.Remove(guid);
            BulletEntity.OnInitialized += bullets.Add;
            BulletEntity.OnDestroy += guid => bullets.Remove(guid);
            BulletEntity.QueryFighter = fighters.TryGetValue;
        }
    }
}