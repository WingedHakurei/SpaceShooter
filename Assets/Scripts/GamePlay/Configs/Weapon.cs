namespace GamePlay.Configs
{
    public class Weapon
    {
        public Bullet bullet;
        public int count;
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