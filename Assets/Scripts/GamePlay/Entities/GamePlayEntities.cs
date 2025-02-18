using System;
using System.Collections.Generic;
using GamePlay.Runtimes;

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

        public void SetActionInvoker(Action<string, RuntimeBase> invokeAction)
        {
            if (invokeAction != null)
            {
                BulletEntity.InvokeAction += invokeAction;
            }
        }
    }
}