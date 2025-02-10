using System;
using GamePlay.Configs;
using GamePlay.Controllers;
using GamePlay.Entities;
using UnityEngine;
using Utils;

namespace GamePlay
{
    public class GamePlayManager : MonoBehaviour
    {
        private Pool _pool;
        private GamePlayConfigs _configs;
        private GamePlayEntities _entities;
        [SerializeField, Range(0, GamePlayConfigs.MaxGameSpeed)] 
        private int _debugGameSpeed = GamePlayConfigs.MaxGameSpeed;
        [SerializeField] private string[] _debugPrefabNames;
        [SerializeField] private GameObject[] _debugPrefabs;
        [SerializeField] private Transform _debugStartPosition;
        private PlayerController _debugPlayer;

        private void Start()
        {
            _pool = new Pool(_debugPrefabNames, _debugPrefabs);
            
            #region debug game play configs
            _configs = new GamePlayConfigs
            {
                gameSpeed = 0,
                gameTime = 0f,
                bullets = new Bullet[]
                {
                    new ()
                    {
                        damage = 10, 
                        speed = 10f
                    }
                }
            };
            _configs.weapons = new Weapon[]
            {
                new()
                {
                    bullet = _configs.bullets[0],
                    count = 3,
                    shape = Weapon.Shape.Sector,
                    shapeRange = 60f,
                    cd = 5f
                }
            };
            _configs.fighters = new Fighter[]
            {
                new()
                {
                    name = "DefaultFighter",
                    speed = 5f,
                    hp = 100,
                    weapons = new[] { _configs.weapons[0] }
                }
            };
            #endregion
            
            #region debug game play entities
            _entities = new GamePlayEntities();
            var fighter = new FighterEntity
            {
                config = _configs.fighters[0],
                id = 1,
                team = 1,
                position = _debugStartPosition.position,
                targetPosition = _debugStartPosition.position,
            };
            fighter.cds = new float[fighter.config.weapons.Length];
            fighter.curHp = fighter.config.hp;
            fighter.Target = _pool.Get(fighter.config.name);
            fighter.Target.SetActive(true);
            fighter.Target.transform.position = _debugStartPosition.position;
            _entities.fighters[fighter.id] = fighter;
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
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }
    }
}