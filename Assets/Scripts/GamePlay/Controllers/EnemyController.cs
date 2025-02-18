using System;
using GamePlay.Configs;
using GamePlay.Entities;
using GamePlay.Runtimes;
using Utils;

namespace GamePlay.Controllers
{
    public class EnemyController
    {
        private readonly FighterEntity _fighter;
        private readonly Behavior _behavior;
        public Guid guid;
        public int currentCommand;
        public float cd;
        public EnemyState enemyState;

        public EnemyController(Fighter config, Behavior behavior)
        {
            _behavior = behavior;
            guid = Guid.NewGuid();
            _fighter = new FighterEntity(new FighterRuntime
            {
                config = config,
                team = 2,
                guid = guid,
                position = _behavior.startPosition,
                targetPosition = _behavior.startPosition,
                cds = new float[config.weapons.Length],
                curHp = config.hp
            });
            _fighter.Init(Pool<Trigger2D>.Get(config.name));
        }

        public void Update(float delta)
        {
            if (_behavior.commands.Length == 0)
            {
                return;
            }
            
            if (currentCommand == -1)
            {
                return;
            }
            
            var command = _behavior.commands[currentCommand];

            switch (enemyState)
            {
                case EnemyState.Ready:
                    switch (command.type)
                    {
                        case CommandType.None:
                            break;
                        case CommandType.Move:
                            _fighter.Runtime.targetPosition = command.position;
                            break;
                        case CommandType.Attack:
                            _fighter.Attack();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    enemyState = EnemyState.Processing;
                    break;
                case EnemyState.Processing:
                    if (command.type != CommandType.Move || _fighter.Runtime.position == _fighter.Runtime.targetPosition)
                    {
                        cd = command.cd;
                        enemyState = EnemyState.Waiting;
                    }
                    break;
                case EnemyState.Waiting:
                    if (cd > 0f)
                    {
                        cd -= delta;
                    }
                    else
                    {
                        currentCommand = command.next;
                        enemyState = currentCommand >= 0 && currentCommand < _behavior.commands.Length
                            ? EnemyState.Ready
                            : EnemyState.End;
                    }

                    break;
                case EnemyState.End:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum EnemyState
        {
            Ready,
            Processing,
            Waiting,
            End
        }
    }
}