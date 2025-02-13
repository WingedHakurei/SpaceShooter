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
        private readonly Bullet _config;
        
        public Guid guid;
        public int team;
        public Vector2 position;
        public Vector2 direction;

        public BulletEntity(Bullet config)
        {
            _config = config;
        }

        public void Init(Trigger2D bulletObject)
        {
            _bulletObject = bulletObject;
            _bulletObject.transform.position = position;
            _bulletObject.guid = guid;
            _bulletObject.OnTriggerEnter2DHandler += OnTriggerEnter2D;
            _bulletObject.gameObject.SetActive(true);
            OnInitialized?.Invoke(guid, this);
        }

        public void Update(float delta)
        {
            position += direction * (_config.speed * delta);
            _bulletObject.transform.position = position;
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

            if (fighter.team == team)
            {
                return;
            }

            var killed = fighter.TakeDamage(_config.damage);
            // TODO: bullet {guid} from team {team} killed fighter {fighter.guid} from team {fighter.team}
            Destroy();
        }

        private void Destroy()
        {
            _bulletObject.OnTriggerEnter2DHandler -= OnTriggerEnter2D;
            Pool<Trigger2D>.Collect(_config.name, _bulletObject);
            _bulletObject = null;
            OnDestroy?.Invoke(guid);
        }

    }
}