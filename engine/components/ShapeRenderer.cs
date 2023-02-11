using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using alice.engine.graphics;

using MonoGame.Extended.Sprites;

namespace alice.engine.components
{
    public class ShapeRenderer : Component
    {
        public Shape shape;

        public ShapeRenderer()
        {

        }

        public ShapeRenderer(Shape shape)
        {
            this.shape = shape;
        }

        internal override void Draw(Sprites spritesBatch)
        {
            shape?.Draw(spritesBatch, transform);
        }

        internal override void DestroyComponent()
        {

        }


    }

    public class Shape
    {
        internal virtual void Draw(Sprites spritesBatch, Transform transform) { }
    }

    public class ShapeRectangle : Shape
    {

        public Color color;
        public Gradient gradient;

        public ShapeRectangle(Color color)
        {
            this.color = color;
        }

        public ShapeRectangle(Gradient gradient)
        {
            this.gradient = gradient;
        }

        internal override void Draw(Sprites spritesBatch, Transform transform)
        {
            if(color != null)
            {
                spritesBatch.shapes.DrawFilledRectangle(
                new Microsoft.Xna.Framework.Rectangle((int)transform.position.X, (int)transform.position.Y,
                (int)transform.scale.X, (int)transform.scale.Y), color);
            }
            else if(gradient != null)
            {
                spritesBatch.shapes.DrawFilledRectangleGradient(
                new Microsoft.Xna.Framework.Rectangle((int)transform.position.X, (int)transform.position.Y,
                (int)transform.scale.X, (int)transform.scale.Y), gradient);
            }
        }
    }


}
