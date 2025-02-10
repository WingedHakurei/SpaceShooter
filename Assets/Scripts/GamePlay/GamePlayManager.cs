using System;
using System.Linq;
using GamePlay.Configs;
using GamePlay.Controllers;
using GamePlay.Entities;
using UnityEngine;
using Utils;

namespace GamePlay
{
    // TODO: Enemy Controller & Controller Management
    // TODO: Wave Config & Enemy Spawn
    // TODO: xLua Game Config
    // TODO: Save & Load
    // TODO: xLua Events
    public class GamePlayManager : MonoBehaviour
    {
        private GamePlayConfigs _configs;
        private GamePlayEntities _entities;
        [SerializeField, Range(0, GamePlayConfigs.MaxGameSpeed)] 
        private int _debugGameSpeed;
        [SerializeField] private string[] _debugPrefabNames;
        [SerializeField] private GameObject[] _debugPrefabs;
        [SerializeField] private Transform _debugStartPosition;
        [SerializeField] private Transform _debugEnemyPosition;
        private PlayerController _debugPlayer;

        private void Start()
        {
            Pool.Instance = new Pool(_debugPrefabNames, _debugPrefabs);
            
            #region debug game play configs
            _configs = new GamePlayConfigs
            {
                gameSpeed = 0,
                gameTime = 0f,
                bullets = new Bullet[]
                {
                    new ()
                    {
                        name = "DefaultBullet",
                        damage = 10, 
                        speed = 10f
                    }
                }
            };
            _configs.weapons = new Weapon[]
            {
                new()
                {
                    name = "LeftWeapon",
                    bullet = _configs.bullets[0],
                    count = 3,
                    angle = 180f,
                    shape = Weapon.Shape.Sector,
                    shapeRange = 30f,
                    cd = 1f
                },
                new()
                {
                    name = "RightWeapon",
                    bullet = _configs.bullets[0],
                    count = 3,
                    angle = 0f,
                    shape = Weapon.Shape.Sector,
                    shapeRange = 30f,
                    cd = 1f
                },
                new()
                {
                    name = "ForwardWeapon",
                    bullet = _configs.bullets[0],
                    count = 5,
                    angle = 90f,
                    shape = Weapon.Shape.Parallel,
                    shapeRange = 2f,
                    cd = 0.2f
                }
            };
            _configs.fighters = new Fighter[]
            {
                new()
                {
                    name = "DefaultFighter",
                    speed = 5f,
                    hp = 100,
                    weapons = new[]
                    {
                        _configs.weapons[0],
                        _configs.weapons[1],
                        _configs.weapons[2],
                    }
                }
            };
            #endregion
            
            #region debug game play entities
            _entities = new GamePlayEntities();
            var fighter = new FighterEntity
            {
                config = _configs.fighters.First(f => f.name == "DefaultFighter"),
                team = 1,
                guid = Guid.NewGuid(),
                position = _debugStartPosition.position,
                targetPosition = _debugStartPosition.position,
            };
            fighter.cds = new float[fighter.config.weapons.Length];
            fighter.curHp = fighter.config.hp;
            fighter.Init(Pool.Get(fighter.config.name));
            
            var enemy = new FighterEntity
            {
                config = _configs.fighters.First(f => f.name == "DefaultFighter"),
                team = 2,
                guid = Guid.NewGuid(),
                position = _debugEnemyPosition.position,
                targetPosition = _debugEnemyPosition.position,
            };
            enemy.cds = new float[enemy.config.weapons.Length];
            enemy.curHp = enemy.config.hp;
            enemy.Init(Pool.Get(enemy.config.name));
            #endregion
            
            _debugPlayer = new PlayerController(fighter);
        }

        private void Update()
        {
            _configs.gameSpeed = _debugGameSpeed;
            var delta = Time.deltaTime * _configs.gameSpeed / GamePlayConfigs.MaxGameSpeed;
            _configs.gameTime += delta;

            _debugPlayer.Update(delta);
            foreach (var fighter in _entities.fighters.Values)
            {
                fighter.Update(delta);
            }

            foreach (var bullet in _entities.bullets.Values)
            {
                bullet.Update(delta);
            }
        }

        private void OnDestroy()
        {
            Pool.Instance.Dispose();
        }
    }
}