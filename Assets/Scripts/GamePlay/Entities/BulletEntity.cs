using System;
using System.Collections.Generic;
using GamePlay.Configs;
using UnityEngine;
using Utils;

namespace GamePlay.Entities
{
    public class BulletEntity
    {
        public static Dictionary<Guid, BulletEntity> All { get; } = new();
        private GameObject _bulletObject;
        private Trigger2D _trigger;
        public Bullet config;
        public Guid guid;
        public int team;
        public Vector2 position;
        public Vector2 direction;

        public void Init(GameObject bulletObject)
        {
            _bulletObject = bulletObject;
            _bulletObject.transform.position = position;
            _trigger = _bulletObject.GetComponent<Trigger2D>();
            _trigger.guid = guid;
            _trigger.OnTriggerEnter2DHandler += OnTriggerEnter2D;
            _bulletObject.SetActive(true);
            All[guid] = this;
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
                SelfDestroy();
                return;
            }

            if (!FighterEntity.All.TryGetValue(trigger.guid, out var fighter))
            {
                return;
            }

            if (fighter.team == team)
            {
                return;
            }

            fighter.TakeDamage(config.damage);
            SelfDestroy();
        }

        private void SelfDestroy()
        {
            _trigger.OnTriggerEnter2DHandler -= OnTriggerEnter2D;
            _trigger = null;
            All.Remove(guid);
            Pool.Collect(config.name, _bulletObject);
            _bulletObject = null;
        }

    }
}