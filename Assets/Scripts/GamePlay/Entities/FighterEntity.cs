using GamePlay.Configs;
using UnityEngine;

namespace GamePlay.Entities
{
    public class FighterEntity : IEntity
    {
        public GameObject Target { get; set; }
        public Fighter config;
        public int id;
        public int team;
        public Vector2 position;
        public Vector2 targetPosition;
        public float[] cds;
        public int curHp;
        
        public void Start()
        {
        }

        public void Update(float delta)
        {
            if (position != targetPosition)
            {
                var newPosition = Vector2.MoveTowards(position, targetPosition, config.speed * delta);
                position = newPosition;
                Target.transform.position = newPosition;
            }
        }

        public void OnDestroy()
        {
        }
    }
}