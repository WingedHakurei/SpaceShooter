using System;

namespace GamePlay.Events
{
    public class Event<T>
    {
        public Predicate<T> condition;
        public Action<T> effect;
    }
}