using System.Linq;
using GamePlay.Configs;
using GamePlay.Controllers;
using GamePlay.Entities;
using UnityEngine;
using Utils;

namespace GamePlay
{
    // TODO: Load lua configs and parse them to _configs
    // TODO: Save & Load
    // TODO: xLua Events
    public class GamePlayManager : MonoBehaviour
    {
        private GamePlayConfigs _configs;
        private GamePlayEntities _entities;
        private GamePlayControllers _controllers;
        private int _gameSpeed = 0;
        private float _gameTime = 0;
        private const int MaxGameSpeed = 5;
        
        [SerializeField, Range(0, MaxGameSpeed)] private int _debugGameSpeed;
        [SerializeField] private string[] _debugPrefabNames;
        [SerializeField] private Trigger2D[] _debugPrefabs;
        [SerializeField] private Transform _debugStartPosition;

        private void Start()
        {
            Pool<Trigger2D>.Instance = new Pool<Trigger2D>(_debugPrefabNames, _debugPrefabs);
            
            _configs = LoadConfigs();

            _entities = new GamePlayEntities();
            _controllers = new GamePlayControllers(_configs);
            
        }

        private GamePlayConfigs LoadConfigs()
        {
            #region debug game play configs
            var configs = new GamePlayConfigs
            {
                playerPosition = _debugStartPosition.position,
                bullets = new Bullet[]
                {
                    new ()
                    {
                        name = "DefaultBullet",
                        damage = 10, 
                        speed = 10f
                    }
                },
                behaviors = new Behavior[]
                {
                    new ()
                    {
                        startPosition = new Vector2(-7f, 3.5f),
                        commands = new Command[]
                        {
                            new() { type = CommandType.Move, position = new Vector2(0f, 0f), next = 1 },
                            new() { type = CommandType.Move, position = new Vector2(7f, 3.5f), next = 2 },
                            new() { type = CommandType.Move, position = new Vector2(-7f, 3.5f), next = 0 },
                        }
                    },
                    new ()
                    {
                        startPosition = new Vector2(-7f, -3.5f),
                        commands = new Command[]
                        {
                            new() { type = CommandType.Move, position = new Vector2(0f, 0f), next = 1 },
                            new() { type = CommandType.Move, position = new Vector2(7f, -3.5f), next = 2 },
                            new() { type = CommandType.Move, position = new Vector2(-7f, -3.5f), next = 0 },
                        },
                    },
                    new ()
                    {
                        startPosition = new Vector2(-7f, 3.5f),
                        commands = new Command[]
                        {
                            new() { type = CommandType.None, cd = 1f, next = 1 },
                            new() { type = CommandType.Move, position = new Vector2(0f, 0f), next = 2  },
                            new() { type = CommandType.Attack, cd = 2f, next = 3  },
                            new() { type = CommandType.Attack, cd = 1f, next = 4  },
                            new() { type = CommandType.Move, position = new Vector2(70f, 35f), next = -1 },
                        }
                    },
                    new ()
                    {
                        startPosition = new Vector2(0f, 3.5f),
                        commands = new Command[]
                        {
                            new() { type = CommandType.Move, position = new Vector2(0f, -3.5f), next = 1 },
                            new() { type = CommandType.Attack, next = 2 },
                            new() { type = CommandType.Move, position = new Vector2(0f, 3.5f), next = 3 },
                            new() { type = CommandType.Attack, next = 0 },
                        }
                    }
                }
            };
            configs.weapons = new Weapon[]
            {
                new()
                {
                    name = "LeftWeapon",
                    bullet = configs.bullets[0],
                    count = 3,
                    angle = 180f,
                    shape = Weapon.Shape.Sector,
                    shapeRange = 30f,
                    cd = 1f
                },
                new()
                {
                    name = "RightWeapon",
                    bullet = configs.bullets[0],
                    count = 3,
                    angle = 0f,
                    shape = Weapon.Shape.Sector,
                    shapeRange = 30f,
                    cd = 1f
                },
                new()
                {
                    name = "ForwardWeapon",
                    bullet = configs.bullets[0],
                    count = 5,
                    angle = 90f,
                    shape = Weapon.Shape.Parallel,
                    shapeRange = 2f,
                    cd = 0.2f
                }
            };
            configs.fighters = new Fighter[]
            {
                new()
                {
                    name = "DefaultFighter",
                    speed = 5f,
                    hp = 100,
                    weapons = new[]
                    {
                        configs.weapons[0],
                        configs.weapons[1],
                        configs.weapons[2],
                    }
                }
            };
            configs.waves = new Wave[]
            {
                new()
                {
                    fighters = new[] { configs.fighters[0], configs.fighters[0], configs.fighters[0] },
                    behaviors = new[] { configs.behaviors[0], configs.behaviors[1], configs.behaviors[2] },
                    intervals = new[] { 0f, 1f, 0f }
                },
                new()
                {
                    fighters = new[] { configs.fighters[0], configs.fighters[0] },
                    behaviors = new[] { configs.behaviors[2], configs.behaviors[3] },
                    intervals = new[] { 1f, 0f }
                }
            };
            configs.stages = new Stage[]
            {
                new()
                {
                    waves = new[] { configs.waves[0] },
                    intervals = new[] { 0f }
                },
                new()
                {
                    waves = new[] { configs.waves[0], configs.waves[1] },
                    intervals = new[] { 5f, 0f }
                }
            };
            configs.player = configs.fighters[0];
            #endregion

            return configs;
        }

        private void Update()
        {
            _gameSpeed = _debugGameSpeed;
            var delta = Time.deltaTime * _gameSpeed / MaxGameSpeed;
            _gameTime += delta;

            _controllers.Update(delta);
            
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
            Pool<Trigger2D>.Instance.Dispose();
        }
    }
}