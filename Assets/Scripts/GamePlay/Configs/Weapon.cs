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

        public enum Shape
        {
            Parallel,
            Sector,
        }
    }
}