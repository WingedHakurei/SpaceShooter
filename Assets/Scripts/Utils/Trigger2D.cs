using System;
using UnityEngine;

namespace Utils
{
    public class Trigger2D : MonoBehaviour
    {
        public event Action<Collider2D> OnTriggerEnter2DHandler;
        public event Action<Collider2D> OnTriggerExit2DHandler;

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTriggerEnter2DHandler?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnTriggerExit2DHandler?.Invoke(other);
        }
    }
}