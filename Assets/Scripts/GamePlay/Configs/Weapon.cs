using System;
using XLua;

namespace GamePlay.Configs
{
    public class Weapon
    {
        public string name;
        public Bullet bullet;
        public int count;
        public float angle;
        public Shape shape;
        public float shapeRange;
        public float cd;

        public Weapon(LuaTable lua, Func<string, Bullet> nameToBullet)
        {
            name = lua.Get<string>(nameof(name));
            bullet = nameToBullet(lua.Get<string>(nameof(bullet)));
            count = lua.Get<int>(nameof(count));
            angle = lua.Get<float>(nameof(angle));
            shape = Enum.Parse<Shape>(lua.Get<string>(nameof(shape)));
            shapeRange = lua.Get<float>(nameof(shapeRange));
            cd = lua.Get<float>(nameof(cd));
        }
        

        public enum Shape
        {
            Parallel,
            Sector,
        }
    }
}