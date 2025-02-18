using System;
using System.Collections.Generic;
using GamePlay.Configs;
using UnityEngine;
using Utils;

namespace GamePlay.Entities
{
    public class BulletEntity
    {
        public static event Action<Guid, BulletEntity> OnInitialized;
        public static event Action<Guid> OnDestroy;
        public delegate bool QueryFighterDelegate(Guid guid, out FighterEntity fighter);
        public static QueryFighterDelegate QueryFighter;
        
        private Trigger2D _bulletObject;
        public BulletRuntime Runtime { get; }

        public BulletEntity(BulletRuntime runtime)
        {
            Runtime = runtime;
        }

        public void Init(Trigger2D bulletObject)
        {
            _bulletObject = bulletObject;
            _bulletObject.transform.position = Runtime.position;
            _bulletObject.guid = Runtime.guid;
            _bulletObject.OnTriggerEnter2DHandler += OnTriggerEnter2D;
            _bulletObject.gameObject.SetActive(true);
            OnInitialized?.Invoke(Runtime.guid, this);
        }

        public void Update(float delta)
        {
            Runtime.position += Runtime.direction * (Runtime.config.speed * delta);
            _bulletObject.transform.position = Runtime.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var trigger = other.GetComponent<Trigger2D>();
            if (!trigger)
            {
                Destroy();
                return;
            }

            if (!QueryFighter.Invoke(trigger.guid, out var fighter))
            {
                return;
            }

            if (fighter.Runtime.team == Runtime.team)
            {
                return;
            }

            var killed = fighter.TakeDamage(Runtime.config.damage);
            // TODO: bullet {guid} from team {team} killed fighter {fighter.guid} from team {fighter.team}
            Destroy();
        }

        private void Destroy()
        {
            _bulletObject.OnTriggerEnter2DHandler -= OnTriggerEnter2D;
            Pool<Trigger2D>.Collect(Runtime.config.name, _bulletObject);
            _bulletObject = null;
            OnDestroy?.Invoke(Runtime.guid);
        }

    }
}