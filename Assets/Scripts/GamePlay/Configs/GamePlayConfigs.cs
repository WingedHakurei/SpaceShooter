using System.Collections.Generic;
using GamePlay.Events;
using UnityEngine;
using Utils;
using XLua;

namespace GamePlay.Configs
{
    public class GamePlayConfigs
    {
        #region basic
        public Dictionary<string, Bullet> bullets;
        public Dictionary<string, Weapon> weapons;
        public Dictionary<string, Fighter> fighters;
        public Behavior[] behaviors;
        public Wave[] waves;
        public Stage[] stages;
        public Fighter player;
        public Vector2 playerPosition;
        #endregion
        
        #region events
        public Dictionary<string, GameAction> actions;
        public Dictionary<string, GameEvent> events;
        #endregion
        
        // TODO: Refactory on lua loading
        public GamePlayConfigs(LuaTable basicLua, LuaTable luaActions, LuaTable luaEvents)
        {
            var luaBullets = basicLua.Get<LuaTable>(nameof(bullets));
            bullets = new Dictionary<string, Bullet>();
            for (var i = 0; i < luaBullets.Length; i++)
            {
                var bullet = new Bullet(luaBullets.Get<int, LuaTable>(i + 1));
                bullets[bullet.name] = bullet;
            }
            
            var luaWeapons = basicLua.Get<LuaTable>(nameof(weapons));
            weapons = new Dictionary<string, Weapon>();
            for (var i = 0; i < luaWeapons.Length; i++)
            {
                var weapon = new Weapon(luaWeapons.Get<int, LuaTable>(i + 1), bullets.GetValue);
                weapons[weapon.name] = weapon;
            }
            
            var luaFighters = basicLua.Get<LuaTable>(nameof(fighters));
            fighters = new Dictionary<string, Fighter>();
            for (var i = 0; i < luaFighters.Length; i++)
            {
                var fighter = new Fighter(luaFighters.Get<int, LuaTable>(i + 1), weapons.GetValue);
                fighters[fighter.name] = fighter;
            }
            
            var luaBehaviors = basicLua.Get<LuaTable>(nameof(behaviors));
            behaviors = new Behavior[luaBehaviors.Length];
            for (var i = 0; i < behaviors.Length; i++)
            {
                behaviors[i] = new Behavior(luaBehaviors.Get<int, LuaTable>(i + 1));
            }
            
            var luaWaves = basicLua.Get<LuaTable>(nameof(waves));
            waves = new Wave[luaWaves.Length];
            for (var i = 0; i < waves.Length; i++)
            {
                waves[i] = new Wave(luaWaves.Get<int, LuaTable>(i + 1), fighters.GetValue, behaviors.GetValue);
            }
            
            var luaStages = basicLua.Get<LuaTable>(nameof(stages));
            stages = new Stage[luaStages.Length];
            for (var i = 0; i < stages.Length; i++)
            {
                stages[i] = new Stage(luaStages.Get<int, LuaTable>(i + 1), waves.GetValue);
            }

            player = fighters[basicLua.Get<string>(nameof(player))];
            var luaPlayerPosition = basicLua.Get<LuaTable>(nameof(playerPosition));
            playerPosition = new Vector2(luaPlayerPosition.Get<int, float>(1), luaPlayerPosition.Get<int, float>(2));
            
            actions = new Dictionary<string, GameAction>();
            foreach (var key in luaActions.GetKeys<string>())
            {
                var gameAction = new GameAction(luaActions.Get<LuaTable>(key));
                actions[key] = gameAction;
            }
            events = new Dictionary<string, GameEvent>();
            foreach (var key in luaEvents.GetKeys<string>())
            {
                var gameEvent = new GameEvent(luaEvents.Get<LuaTable>(key));
                events[key] = gameEvent;
            }
        }
    }
}