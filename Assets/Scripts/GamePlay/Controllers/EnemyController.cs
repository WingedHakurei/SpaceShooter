using System;
using GamePlay.Configs;
using GamePlay.Entities;
using UnityEngine;
using Utils;

namespace GamePlay.Controllers
{
    public class EnemyController
    {
        private readonly FighterEntity _fighter;
        private readonly Behavior _behavior;
        public Guid guid;
        public int currentCommand;
        public bool isProcessing;
        public float cd;

        public EnemyController(Fighter config, Behavior behavior)
        {
            _behavior = behavior;
            guid = Guid.NewGuid();
            _fighter = new FighterEntity
            {
                config = config,
                team = 2,
                guid = guid,
                position = _behavior.startPosition,
                targetPosition = _behavior.startPosition,
                cds = new float[config.weapons.Length],
                curHp = config.hp
            };
            _fighter.Init(Pool<Trigger2D>.Get(config.name));
        }

        public void Update(float delta)
        {
            if (_behavior.commands.Length == 0)
            {
                return;
            }

            if (cd > 0f)
            {
                cd -= delta;
                if (cd <= 0f)
                {
                    currentCommand = _behavior.nextIndices[currentCommand];
                }
                return;
            }

            if (currentCommand == -1)
            {
                return;
            }
            
            var command = _behavior.commands[currentCommand];
            if (isProcessing)
            {
                if (command.type == CommandType.Move)
                {
                    if (_fighter.position == _fighter.targetPosition)
                    {
                        isProcessing = false;
                    }
                }
                else
                {
                    isProcessing = false;
                }

                if (isProcessing)
                {
                    return;
                }

                cd = _behavior.intervals[currentCommand];
                if (cd <= 0f)
                {
                    currentCommand = _behavior.nextIndices[currentCommand];
                }
            }
            else
            {
                switch (command.type)
                {
                    case CommandType.None:
                        break;
                    case CommandType.Move:
                        _fighter.targetPosition = command.position;
                        break;
                    case CommandType.Attack:
                        _fighter.Attack();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                isProcessing = true;
            }
        }
    }
}