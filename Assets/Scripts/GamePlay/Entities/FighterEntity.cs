using System;
using System.Collections.Generic;
using GamePlay.Configs;
using GamePlay.Runtimes;
using UnityEngine;
using Utils;

namespace GamePlay.Entities
{
    public class FighterEntity
    {
        public static event Action<Guid, FighterEntity> OnInitialized;
        public static event Action<Guid> OnDestroy;
        
        private Trigger2D _fighterObject;
        public FighterRuntime Runtime { get; }

        public FighterEntity(FighterRuntime runtime)
        {
            Runtime = runtime;
        }

        public void Init(Trigger2D fighterObject)
        {
            _fighterObject = fighterObject;
            _fighterObject.transform.position = Runtime.position;
            _fighterObject.guid = Runtime.guid;
            _fighterObject.OnTriggerEnter2DHandler += OnTriggerEnter2D;
            _fighterObject.gameObject.SetActive(true);
            OnInitialized?.Invoke(Runtime.guid, this);
        }

        public void Update(float delta)
        {
            if (Runtime.position != Runtime.targetPosition)
            {
                var newPosition = Vector2.MoveTowards(Runtime.position, Runtime.targetPosition, Runtime.config.speed * delta);
                Runtime.position = newPosition;
                _fighterObject.transform.position = newPosition;
            }

            for (var i = 0; i < Runtime.cds.Length; i++)
            {
                if (Runtime.cds[i] > 0f)
                {
                    Runtime.cds[i] -= delta;
                }
            }
        }

        public void MoveDirection(Vector2 delta)
        {
            Runtime.targetPosition = Runtime.position + Runtime.config.speed * delta;
        }

        public void Attack()
        {
            for (var i = 0; i < Runtime.cds.Length; i++)
            {
                if (Runtime.cds[i] > 0f)
                {
                    continue;
                }

                var weapon = Runtime.config.weapons[i];
                var direction = new Vector2(Mathf.Cos(weapon.angle * Mathf.Deg2Rad), Mathf.Sin(weapon.angle * Mathf.Deg2Rad));
                if (weapon.count == 1)
                {
                    var bulletEntity = new BulletEntity(new BulletRuntime
                    {
                        config = weapon.bullet,
                        team = Runtime.team,
                        guid = Guid.NewGuid(),
                        position = Runtime.position,
                        direction = direction,
                        shotBy = Runtime
                    });
                    bulletEntity.Init(Pool<Trigger2D>.Get(weapon.bullet.name));
                }
                else if (weapon.shape == Weapon.Shape.Parallel)
                {
                    var perpendicular = Vector2.Perpendicular(direction);
                    var startOffset = perpendicular * (weapon.shapeRange * -0.5f);
                    for (var j = 0; j < weapon.count; j++)
                    {
                        var bulletPosition = Runtime.position + startOffset + perpendicular * Mathf.Lerp(0, weapon.shapeRange, j / (float)(weapon.count - 1));
                        var bulletEntity = new BulletEntity(new BulletRuntime
                        {
                            config = weapon.bullet,
                            team = Runtime.team,
                            guid = Guid.NewGuid(),
                            position = bulletPosition,
                            direction = direction,
                            shotBy = Runtime
                        });
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
                        var bulletEntity = new BulletEntity(new BulletRuntime
                        {
                            config = weapon.bullet,
                            team = Runtime.team,
                            guid = Guid.NewGuid(),
                            position = Runtime.position,
                            direction = Vector3.Slerp(startDirection, endDirection, j / (float)(weapon.count - 1)),
                            shotBy = Runtime
                        });
                        bulletEntity.Init(Pool<Trigger2D>.Get(weapon.bullet.name));
                    }
                }

                Runtime.cds[i] = weapon.cd;
            }
        }

        public bool TakeDamage(int damage)
        {
            if (Runtime.curHp <= 0)
            {
                return false;
            }

            Runtime.curHp -= damage;
            if (Runtime.curHp > 0)
            {
                return false;
            }

            Runtime.curHp = 0;
            Destroy();
            return true;
        }
        
        private void Destroy()
        {
            _fighterObject.OnTriggerEnter2DHandler -= OnTriggerEnter2D;
            Pool<Trigger2D>.Collect(Runtime.config.name, _fighterObject);
            _fighterObject = null;
            OnDestroy?.Invoke(Runtime.guid);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var trigger = other.GetComponent<Trigger2D>();
            if (!trigger)
            {
                Destroy();
            }
        }
    }
}