using MonoGame.Extended;

using Newtonsoft.Json.Linq;

namespace alice.engine
{
    public class Rectangle
    {

        public int X, Y, Width, Height;

        public int Left => X;

        public int Right => X + Width;

        public int Top => Y;

        public int Bottom => Y + Height;

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        internal Rectangle(Microsoft.Xna.Framework.Rectangle rectangle)
        {
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        internal Rectangle(RectangleF rectangle)
        {
            X = (int)rectangle.Left;
            Y = (int)rectangle.Top;
            Width = (int)rectangle.Width;
            Height = (int)rectangle.Height;
        }

        internal Microsoft.Xna.Framework.Rectangle ToXnaRectangle()
        {
            return new Microsoft.Xna.Framework.Rectangle(X, Y, Width, Height);
        }

        public bool Intersects(Rectangle rectangle)
        {
            if (rectangle.Left < Right && Left < rectangle.Right && rectangle.Top < Bottom)
            {
                return Top < rectangle.Bottom;
            }
            return false;
        }

        public int XYWH()
        {
            return X + Y + Width + Height;
        }

        public override string ToString()
        {
            return X + "," + Y + ";" + Width + "," + Height;
        }
    }
}
