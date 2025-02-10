using GamePlay.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GamePlay.Controllers
{
    public class PlayerController
    {
        private readonly FighterEntity _fighter;
        private readonly InputAction _moveAction;
        private readonly InputAction _slowAction;
        private readonly InputAction _attackAction;

        public PlayerController(FighterEntity fighter)
        {
            _fighter = fighter;
            _moveAction = InputSystem.actions["Move"];
            _slowAction = InputSystem.actions["Slow"];
            _attackAction = InputSystem.actions["Attack"];
        }

        public void Update(float delta)
        {
            var move = _moveAction.ReadValue<Vector2>();
            var slow = _slowAction.ReadValue<float>() > 0f;
            var attack = _attackAction.ReadValue<float>() > 0f;
            if (move != Vector2.zero)
            {
                _fighter.targetPosition = _fighter.position + 
                                          _fighter.config.speed * delta * (slow ? 0.5f : 1f) * move;
            }

            if (attack)
            {
                _fighter.Attack();
            }
        }
    }
}