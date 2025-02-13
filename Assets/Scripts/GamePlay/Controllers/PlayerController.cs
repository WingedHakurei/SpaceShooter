using System;
using GamePlay.Configs;
using GamePlay.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace GamePlay.Controllers
{
    public class PlayerController
    {
        private readonly FighterEntity _fighter;
        private readonly InputAction _moveAction;
        private readonly InputAction _slowAction;
        private readonly InputAction _attackAction;

        public Guid guid;
        public bool isControllable = true;

        public PlayerController(Fighter config, Vector2 position)
        {
            guid = Guid.NewGuid();
            _fighter = new FighterEntity(config)
            {
                team = 1,
                guid = guid,
                position = position,
                targetPosition = position,
                curHp = config.hp,
                cds = new float[config.weapons.Length]
            };
            _fighter.Init(Pool<Trigger2D>.Get(config.name));
            
            _moveAction = InputSystem.actions["Move"];
            _slowAction = InputSystem.actions["Slow"];
            _attackAction = InputSystem.actions["Attack"];
        }

        public void Update(float delta)
        {
            if (!isControllable)
            {
                return;
            }
            var move = _moveAction.ReadValue<Vector2>();
            var slow = _slowAction.ReadValue<float>() > 0f;
            var attack = _attackAction.ReadValue<float>() > 0f;
            if (move != Vector2.zero)
            {
                _fighter.MoveDirection(delta * (slow ? 0.5f : 1f) * move);
            }

            if (attack)
            {
                _fighter.Attack();
            }
        }
    }
}