using System;
using System.Collections.Generic;
using GamePlay.Configs;
using GamePlay.Entities;
using UnityEngine;

namespace GamePlay.Controllers
{
    public class GamePlayControllers
    {
        public readonly PlayerController playerController;
        public readonly Dictionary<Guid, EnemyController> enemyControllers = new();
        private readonly Stage[] _stages;
        public int currentStage;
        public int currentWave;
        public int currentEnemy;
        public float waveCd;
        public float enemyCd;
        public int remainingEnemies;
        public SpawnState spawnState;

        public GamePlayControllers(GamePlayConfigs configs)
        {
            FighterEntity.OnDestroy += DoOnFighterDestroy;
            playerController = new PlayerController(configs.player, configs.playerPosition);
            _stages = configs.stages;
            remainingEnemies = _stages[currentStage].waves[currentWave].fighters.Length;
        }

        public void Update(float delta)
        {
            playerController.Update(delta);
            foreach (var enemy in enemyControllers.Values)
            {
                enemy.Update(delta);
            }
            
            Spawn(delta);
        }

        private void Spawn(float delta)
        {
            if (currentStage >= _stages.Length)
            {
                return;
            }
            var stage = _stages[currentStage];
            
            switch (spawnState)
            {
                case SpawnState.WaveReady:
                    currentEnemy = 0;
                    remainingEnemies = 0;
                    if (currentWave < stage.waves.Length)
                    {
                        remainingEnemies = stage.waves[currentWave].fighters.Length;
                    }
                    spawnState = remainingEnemies > 0 ? SpawnState.EnemyReady : SpawnState.Idle;
                    break;
                case SpawnState.EnemyReady:
                    var wave = stage.waves[currentWave];
                    var enemyController = new EnemyController(
                        wave.fighters[currentEnemy],
                        wave.behaviors[currentEnemy]);
                    enemyControllers[enemyController.guid] = enemyController;
                    enemyCd = wave.intervals[currentEnemy];
                    spawnState = SpawnState.EnemyWaiting;
                    break;
                case SpawnState.EnemyWaiting:
                    if (enemyCd > 0f)
                    {
                        enemyCd -= delta;
                    }
                    else
                    {
                        currentEnemy++;
                        spawnState = currentEnemy < stage.waves[currentWave].fighters.Length ? SpawnState.EnemyReady : SpawnState.Idle;
                    }
                    break;
                case SpawnState.Idle:
                    if (remainingEnemies <= 0)
                    {
                        waveCd = stage.intervals[currentWave];
                        spawnState = SpawnState.WaveWaiting;
                    }
                    break;
                case SpawnState.WaveWaiting:
                    if (waveCd > 0f)
                    {
                        waveCd -= delta;
                    }
                    else
                    {
                        currentWave++;
                        spawnState = SpawnState.WaveReady;
                        if (currentWave >= stage.waves.Length)
                        {
                            // stage {currentStage} cleared
                            currentStage++;
                            currentWave = 0;
                            if (currentStage >= _stages.Length)
                            {
                                // all stages cleared
                                spawnState = SpawnState.End;
                            }
                        }
                    }
                    break;
                case SpawnState.End:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
        
        private void DoOnFighterDestroy(Guid guid)
        {
            if (guid == playerController.guid)
            {
                playerController.isControllable = false;
            }
            else if (enemyControllers.Remove(guid))
            {
                remainingEnemies--;
            }
        }

        public enum SpawnState
        {
            WaveReady,
            EnemyReady,
            EnemyWaiting,
            Idle,
            WaveWaiting,
            End
        }
    }
}