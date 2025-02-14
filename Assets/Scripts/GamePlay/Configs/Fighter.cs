using System;
using XLua;

namespace GamePlay.Configs
{
    public class Fighter
    {
        public string name;
        public float speed;
        public int hp;
        public Weapon[] weapons;

        public Fighter(LuaTable lua, Func<string, Weapon> nameToWeapon)
        {
            name = lua.Get<string>(nameof(name));
            speed = lua.Get<float>(nameof(speed));
            hp = lua.Get<int>(nameof(hp));
            var luaWeapons = lua.Get<LuaTable>(nameof(weapons));
            weapons = new Weapon[luaWeapons.Length];
            for (var i = 0; i < weapons.Length; i++)
            {
                weapons[i] = nameToWeapon(luaWeapons.Get<int, string>(i + 1));
            }
        }
    }
}