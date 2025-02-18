using System;
using System.Collections.Generic;
using GamePlay.Events;
using GamePlay.Runtimes;

namespace GamePlay.Controllers
{
    public class EventController: IDisposable
    {
        private Dictionary<string, GameAction> _actions;
        private Dictionary<string, GameEvent> _events;
        
        public EventController(Dictionary<string, GameAction> actions, Dictionary<string, GameEvent> events)
        {
            _actions = actions;
            _events = events;
        }

        public void InvokeAction(string name, RuntimeBase runtime)
        {
            if (!_actions.TryGetValue(name, out var gameAction))
            {
                return;
            }

            foreach (var eventName in gameAction.events)
            {
                if (!_events.TryGetValue(eventName, out var @event))
                {
                    continue;
                }

                if (@event.condition == null || @event.condition.Invoke(runtime))
                {
                    @event.effect?.Invoke(runtime);
                }
            }
        }

        public void InvokeEvent(string name, RuntimeBase runtime)
        {
            if (!_events.TryGetValue(name, out var @event))
            {
                return;
            }

            if (@event.condition == null || @event.condition.Invoke(runtime))
            {
                @event.effect?.Invoke(runtime);
            }
        }

        public void Dispose()
        {
            foreach (var @event in _events.Values)
            {
                @event.Dispose();
            }
            _actions.Clear();
            _events.Clear();
        }
    }
}