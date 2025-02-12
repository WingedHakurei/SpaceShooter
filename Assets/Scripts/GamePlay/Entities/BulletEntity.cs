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
        public Bullet config;
        public Guid guid;
        public int team;
        public Vector2 position;
        public Vector2 direction;

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
            position += direction * (config.speed * delta);
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

            fighter.TakeDamage(config.damage);
            Destroy();
        }

        private void Destroy()
        {
            _bulletObject.OnTriggerEnter2DHandler -= OnTriggerEnter2D;
            Pool<Trigger2D>.Collect(config.name, _bulletObject);
            _bulletObject = null;
            OnDestroy?.Invoke(guid);
        }

    }
}