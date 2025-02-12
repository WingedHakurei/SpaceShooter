using System;
using System.Collections.Generic;
using GamePlay.Configs;
using UnityEngine;
using Utils;

namespace GamePlay.Entities
{
    public class FighterEntity
    {
        public static event Action<Guid, FighterEntity> OnInitialized;
        public static event Action<Guid> OnDestroy;
        
        private Trigger2D _fighterObject;
        public Fighter config;
        public Guid guid;
        public int team;
        public Vector2 position;
        public Vector2 targetPosition;
        public float[] cds;
        public int curHp;

        public void Init(Trigger2D fighterObject)
        {
            _fighterObject = fighterObject;
            _fighterObject.transform.position = position;
            _fighterObject.guid = guid;
            _fighterObject.gameObject.SetActive(true);
            OnInitialized?.Invoke(guid, this);
        }

        public void Update(float delta)
        {
            if (position != targetPosition)
            {
                var newPosition = Vector2.MoveTowards(position, targetPosition, config.speed * delta);
                position = newPosition;
                _fighterObject.transform.position = newPosition;
            }

            for (var i = 0; i < cds.Length; i++)
            {
                if (cds[i] > 0f)
                {
                    cds[i] -= delta;
                }
            }
        }

        public void Attack()
        {
            for (var i = 0; i < cds.Length; i++)
            {
                if (cds[i] > 0f)
                {
                    continue;
                }

                var weapon = config.weapons[i];
                var direction = new Vector2(Mathf.Cos(weapon.angle * Mathf.Deg2Rad), Mathf.Sin(weapon.angle * Mathf.Deg2Rad));
                if (weapon.count == 1)
                {
                    var bulletEntity = new BulletEntity
                    {
                        config = weapon.bullet,
                        team = team,
                        guid = Guid.NewGuid(),
                        position = position,
                        direction = direction,
                    };
                    bulletEntity.Init(Pool<Trigger2D>.Get(weapon.bullet.name));
                }
                else if (weapon.shape == Weapon.Shape.Parallel)
                {
                    var perpendicular = Vector2.Perpendicular(direction);
                    var startOffset = perpendicular * (weapon.shapeRange * -0.5f);
                    for (var j = 0; j < weapon.count; j++)
                    {
                        var bulletPosition = position + startOffset + perpendicular * Mathf.Lerp(0, weapon.shapeRange, j / (float)(weapon.count - 1));
                        var bulletEntity = new BulletEntity
                        {
                            config = weapon.bullet,
                            team = team,
                            guid = Guid.NewGuid(),
                            position = bulletPosition,
                            direction = direction,
                        };
                        bulletEntity.Init(Pool<Trigger2D>.Get(weapon.bullet.name));
                    }
                }
                else
                {
                    var startDirection = new Vector2(
                        Mathf.Cos((weapon.angle + weapon.shapeRange * 0.5f) * Mathf.Deg2Rad), 
                        Mathf.Sin((weapon.angle + weapon.shapeRange * 0.5f) * Mathf.Deg2Rad));
                    var endDirection = new Vector2(
                        Mathf.Cos((weapon.angle - weapon.shapeRange * 0.5f) * Mathf.Deg2Rad), 
                        Mathf.Sin((weapon.angle - weapon.shapeRange * 0.5f) * Mathf.Deg2Rad));
                    for (var j = 0; j < weapon.count; j++)
                    {
                        var bulletEntity = new BulletEntity
                        {
                            config = weapon.bullet,
                            team = team,
                            guid = Guid.NewGuid(),
                            position = position,
                            direction = Vector3.Slerp(startDirection, endDirection, j / (float)(weapon.count - 1)),
                        };
                        bulletEntity.Init(Pool<Trigger2D>.Get(weapon.bullet.name));
                    }
                }
                cds[i] = weapon.cd;
            }
        }

        public void TakeDamage(int damage)
        {
            if (curHp <= 0)
            {
                return;
            }

            curHp -= damage;
            if (curHp > 0)
            {
                return;
            }

            curHp = 0;
            Destroy();
        }
        
        private void Destroy()
        {
            Pool<Trigger2D>.Collect(config.name, _fighterObject);
            _fighterObject = null;
            OnDestroy?.Invoke(guid);
        }
    }
}