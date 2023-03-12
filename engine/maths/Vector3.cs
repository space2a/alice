namespace alice.engine
{

    public class Vector3
    {

        public float X, Y, Z;

        public Vector3(Vector2 vector2) 
        {
            X = vector2.X;
            Y = vector2.Y;
            Z = 0;
        }

        public Vector3(Vector2 vector2, float z)
        {
            X = vector2.X;
            Y = vector2.Y;
            Z = z;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        internal Microsoft.Xna.Framework.Vector3 ToXna()
        {
            return new Microsoft.Xna.Framework.Vector3(X, Y, Z);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
    }

}
