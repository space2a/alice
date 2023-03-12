namespace alice.engine
{
    public class Point
    {

        public int X, Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point(int value)
        {
            X = value;
            Y = value;
        }

        internal Point(Microsoft.Xna.Framework.Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        internal Microsoft.Xna.Framework.Point ToXna()
        {
            return new Microsoft.Xna.Framework.Point(X, Y);
        }

        public override string ToString()
        {
            return X + ";" + Y;
        }

    }
}
