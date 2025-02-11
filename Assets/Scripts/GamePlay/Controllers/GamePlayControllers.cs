using System;
using System.Collections.Generic;
using GamePlay.Configs;
using UnityEngine;

namespace GamePlay.Controllers
{
    public class GamePlayControllers
    {
        public readonly PlayerController playerController;
        public readonly Dictionary<Guid, EnemyController> enemyControllers = new();
        public readonly Behavior[] debugBehaviors = new Behavior[]
        {
            new ()
            {
                startPosition = new Vector2(-7f, 3.5f),
                commands = new Command[]
                {
                    new() { type = CommandType.None },
                    new() { type = CommandType.Move, position = new Vector2(0f, 0f) },
                    new() { type = CommandType.Attack },
                    new() { type = CommandType.Attack },
                    new() { type = CommandType.Move, position = new Vector2(7f, 3.5f) },
                },
                nextIndices = new [] { 1, 2, 3, 4, -1 },
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
                nextIndices = new [] { 1, 2, 3, 0 },
                intervals = new [] { 0f, 0f, 0f, 0f }
            }
        };

        public GamePlayControllers(Fighter debugFighter, Vector2 debugPosition)
        {
            playerController = new PlayerController(debugFighter, debugPosition);
            EnemyController enemy;
            enemy = new EnemyController(debugFighter, debugBehaviors[0]);
            enemyControllers[enemy.guid] = enemy;
            enemy = new EnemyController(debugFighter, debugBehaviors[1]);
            enemyControllers[enemy.guid] = enemy;
        }

        public void Update(float delta)
        {
            playerController.Update(delta);
            foreach (var enemy in enemyControllers.Values)
            {
                enemy.Update(delta);
            }
        }
    }
}