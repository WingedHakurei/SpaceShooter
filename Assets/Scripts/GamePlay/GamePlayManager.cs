using System.Linq;
using GamePlay.Configs;
using GamePlay.Controllers;
using GamePlay.Entities;
using UnityEngine;
using Utils;

namespace GamePlay
{
    // TODO: xLua Game Config
    // TODO: Save & Load
    // TODO: xLua Events
    public class GamePlayManager : MonoBehaviour
    {
        private GamePlayConfigs _configs;
        private GamePlayEntities _entities;
        private GamePlayControllers _controllers;
        [SerializeField, Range(0, GamePlayConfigs.MaxGameSpeed)] 
        private int _debugGameSpeed;
        [SerializeField] private string[] _debugPrefabNames;
        [SerializeField] private Trigger2D[] _debugPrefabs;
        [SerializeField] private Transform _debugStartPosition;

        private void Start()
        {
            Pool<Trigger2D>.Instance = new Pool<Trigger2D>(_debugPrefabNames, _debugPrefabs);
            
            #region debug game play configs
            _configs = new GamePlayConfigs
            {
                gameSpeed = 0,
                gameTime = 0f,
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
                            new() { type = CommandType.Move, position = new Vector2(0f, 0f) },
                            new() { type = CommandType.Move, position = new Vector2(7f, 3.5f) },
                            new() { type = CommandType.Move, position = new Vector2(-7f, 3.5f) },
                        },
                        nexts = new [] { 1, 2, 0 },
                        intervals = new [] { 0f, 0f, 0f }
                    },
                    new ()
                    {
                        startPosition = new Vector2(-7f, -3.5f),
                        commands = new Command[]
                        {
                            new() { type = CommandType.Move, position = new Vector2(0f, 0f) },
                            new() { type = CommandType.Move, position = new Vector2(7f, -3.5f) },
                            new() { type = CommandType.Move, position = new Vector2(-7f, -3.5f) },
                        },
                        nexts = new [] { 1, 2, 0 },
                        intervals = new [] { 0f, 0f, 0f }
                    },
                    new ()
                    {
                        startPosition = new Vector2(-7f, 3.5f),
                        commands = new Command[]
                        {
                            new() { type = CommandType.None },
                            new() { type = CommandType.Move, position = new Vector2(0f, 0f) },
                            new() { type = CommandType.Attack },
                            new() { type = CommandType.Attack },
                            new() { type = CommandType.Move, position = new Vector2(70f, 35f) },
                        },
                        nexts = new [] { 1, 2, 3, 4, -1 },
                        intervals = new [] { 1f, 0f, 2f, 1f, 0f }
                    },
                    new ()
                    {
                        startPosition = new Vector2(0f, 3.5f),
                        commands = new Command[]
                        {
                            new() { type = CommandType.Move, position = new Vector2(0f, -3.5f) },
                            new() { type = CommandType.Attack },
                            new() { type = CommandType.Move, position = new Vector2(0f, 3.5f) },
                            new() { type = CommandType.Attack },
                        },
                        nexts = new [] { 1, 2, 3, 0 },
                        intervals = new [] { 0f, 0f, 0f, 0f }
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
            _configs.waves = new Wave[]
            {
                new()
                {
                    fighters = new[] { _configs.fighters[0], _configs.fighters[0], _configs.fighters[0] },
                    behaviors = new[] { _configs.behaviors[0], _configs.behaviors[1], _configs.behaviors[2] },
                    intervals = new[] { 0f, 1f, 0f }
                },
                new()
                {
                    fighters = new[] { _configs.fighters[0], _configs.fighters[0] },
                    behaviors = new[] { _configs.behaviors[2], _configs.behaviors[3] },
                    intervals = new[] { 1f, 0f }
                }
            };
            _configs.stages = new Stage[]
            {
                new()
                {
                    waves = new[] { _configs.waves[0] },
                    intervals = new[] { 0f }
                },
                new()
                {
                    waves = new[] { _configs.waves[0], _configs.waves[1] },
                    intervals = new[] { 5f, 0f }
                }
            };
            _configs.player = _configs.fighters[0];
            #endregion
            
            _entities = new GamePlayEntities();
            _controllers = new GamePlayControllers(_configs);
            
        }

        private void Update()
        {
            _configs.gameSpeed = _debugGameSpeed;
            var delta = Time.deltaTime * _configs.gameSpeed / GamePlayConfigs.MaxGameSpeed;
            _configs.gameTime += delta;

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