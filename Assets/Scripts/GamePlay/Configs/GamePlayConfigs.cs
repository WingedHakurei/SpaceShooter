using System.Collections.Generic;
using UnityEngine;
using Utils;
using XLua;

namespace GamePlay.Configs
{
    public class GamePlayConfigs
    {
        public Dictionary<string, Bullet> bullets;
        public Dictionary<string, Weapon> weapons;
        public Dictionary<string, Fighter> fighters;
        public Behavior[] behaviors;
        public Wave[] waves;
        public Stage[] stages;
        public Fighter player;
        public Vector2 playerPosition;

        public GamePlayConfigs(LuaTable lua)
        {
            var luaBullets = lua.Get<LuaTable>(nameof(bullets));
            bullets = new Dictionary<string, Bullet>();
            for (var i = 0; i < luaBullets.Length; i++)
            {
                var bullet = new Bullet(luaBullets.Get<int, LuaTable>(i + 1));
                bullets[bullet.name] = bullet;
            }
            
            var luaWeapons = lua.Get<LuaTable>(nameof(weapons));
            weapons = new Dictionary<string, Weapon>();
            for (var i = 0; i < luaWeapons.Length; i++)
            {
                var weapon = new Weapon(luaWeapons.Get<int, LuaTable>(i + 1), bullets.GetValue);
                weapons[weapon.name] = weapon;
            }
            
            var luaFighters = lua.Get<LuaTable>(nameof(fighters));
            fighters = new Dictionary<string, Fighter>();
            for (var i = 0; i < luaFighters.Length; i++)
            {
                var fighter = new Fighter(luaFighters.Get<int, LuaTable>(i + 1), weapons.GetValue);
                fighters[fighter.name] = fighter;
            }
            
            var luaBehaviors = lua.Get<LuaTable>(nameof(behaviors));
            behaviors = new Behavior[luaBehaviors.Length];
            for (var i = 0; i < behaviors.Length; i++)
            {
                behaviors[i] = new Behavior(luaBehaviors.Get<int, LuaTable>(i + 1));
            }
            
            var luaWaves = lua.Get<LuaTable>(nameof(waves));
            waves = new Wave[luaWaves.Length];
            for (var i = 0; i < waves.Length; i++)
            {
                waves[i] = new Wave(luaWaves.Get<int, LuaTable>(i + 1), fighters.GetValue, behaviors.GetValue);
            }
            
            var luaStages = lua.Get<LuaTable>(nameof(stages));
            stages = new Stage[luaStages.Length];
            for (var i = 0; i < stages.Length; i++)
            {
                stages[i] = new Stage(luaStages.Get<int, LuaTable>(i + 1), waves.GetValue);
            }

            player = fighters[lua.Get<string>(nameof(player))];
            var luaPlayerPosition = lua.Get<LuaTable>(nameof(playerPosition));
            playerPosition = new Vector2(luaPlayerPosition.Get<int, float>(1), luaPlayerPosition.Get<int, float>(2));
        }
    }
}