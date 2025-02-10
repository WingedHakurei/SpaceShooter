using GamePlay.Configs;
using UnityEngine;

namespace GamePlay.Entities
{
    public class BulletEntity : IEntity
    {
        public GameObject Target { get; set; }
        public Bullet config;
        public int id;
        public int team;
        public Vector2 position;
        public Vector2 targetPosition;
        
        public void Start()
        {
        }

        public void Update(float delta)
        {
        }

        public void OnDestroy()
        {
        }
    }
}