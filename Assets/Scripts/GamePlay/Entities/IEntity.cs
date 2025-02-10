using UnityEngine;

namespace GamePlay.Entities
{
    public interface IEntity
    {
        public GameObject Target { get; set; }
        public void Start();
        public void Update(float delta);
        public void OnDestroy();
    }
}