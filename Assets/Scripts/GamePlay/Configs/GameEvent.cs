using System;
using GamePlay.Runtimes;

namespace GamePlay.Events
{
    public class GameEvent
    {
        public Predicate<RuntimeBase> condition;
        public Action<RuntimeBase> effect;
    }
}