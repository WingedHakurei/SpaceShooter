using XLua;

namespace GamePlay.Configs
{
    public class Bullet
    {
        public string name;
        public int damage;
        public float speed;

        public Bullet(LuaTable lua)
        {
            name = lua.Get<string>(nameof(name));
            damage = lua.Get<int>(nameof(damage));
            speed = lua.Get<float>(nameof(speed));
        }
    }
}