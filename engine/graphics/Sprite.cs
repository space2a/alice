using System;

using alice.engine.maths;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace alice.engine.graphics
{

    public class Sprite
    {
        public Texture2D texture2D { get; protected set; }
        public float opacity = 1;

        public Flip flip = Flip.None;

        public alice.engine.maths.Vector2 origin = alice.engine.maths.Vector2.Zero;

        public int layer = 0;
        internal virtual void Draw(Sprites spritesBatch, Transform transform)
        {

        }

        public enum Flip
        {
            None,
            FlipVertically,
            FlipHorizontally
        }
        
        public void CalculateOrigin()
        {
            origin = new maths.Vector2(texture2D.width /2, texture2D.height /2);
        }

        internal SpriteEffects GetFlip()
        {
            switch (flip)
            {
                case Flip.None:
                    return SpriteEffects.FlipVertically;
                case Flip.FlipVertically:
                    return SpriteEffects.None;
                case Flip.FlipHorizontally:
                    return SpriteEffects.FlipHorizontally;
            }
            return SpriteEffects.FlipVertically;
        }

    }

    public class SingleSprite : Sprite
    {
        public SingleSprite(Texture2D texture2D)
        {
            this.texture2D = texture2D;
            CalculateOrigin();
        }

        public SingleSprite(Texture2D texture2D, float opacity)
        {
            this.texture2D = texture2D;
            this.opacity = opacity;
            CalculateOrigin();
        }


        internal override void Draw(Sprites spritesBatch, Transform transform)
        {
            spritesBatch.Draw(texture2D.texture2D, null, origin.ToXnaVector2(),
                (transform.position + texture2D.offset).ToXnaVector2(),
                transform.rotation, transform.scale.ToXnaVector2(), new Color(255, 255, 255, (byte)(opacity * 255)), GetFlip(), layer);
        }
    }

    public class MultipleSprite : Sprite
    {
        public int slicedBy { get; private set; }

        private MultipleSpriteCoords[] coords;

        public int currentSpriteIndex = 0;

        public int maxSpriteIndex { get; private set; }

        public MultipleSprite(Texture2D texture2D, int slicedBy)
        {
            this.texture2D = texture2D;
            this.slicedBy = slicedBy;

            CalculateOrigin();
            Slice();
        }

        public MultipleSprite(Texture2D texture2D, int slicedBy, float opacity)
        {
            this.texture2D = texture2D;
            this.slicedBy = slicedBy;
            this.opacity = opacity;

            CalculateOrigin();
            Slice();
        }

        private void Slice()
        {
            int w = texture2D.texture2D.Width;
            int h = texture2D.texture2D.Height;

            coords = new MultipleSpriteCoords[(w / slicedBy) * (h / slicedBy)];
            Console.WriteLine("coords" + coords.Length);
            int lastX = 0;
            lastX -= slicedBy;
            int lastY = 0;
            int i = 0;
            
            while (i != coords.Length)
            {
                int x = lastX + slicedBy;
                int y = lastY;
                lastX = x;
                lastY = y;

                if (x >= w) { lastY += slicedBy; lastX = 0; lastX -= slicedBy; continue; }

                coords[i++] = new MultipleSpriteCoords() { x = x, y = y };
                maxSpriteIndex++;
            }
            Console.WriteLine("sliced:" + maxSpriteIndex);
        }

        public MultipleSprite this[int index]
        {
            get { if (index < maxSpriteIndex && index >= 0) currentSpriteIndex = index; return this; }
        }

        internal override void Draw(Sprites spritesBatch, Transform transform)
        {
            spritesBatch.Draw(texture2D.texture2D,
                new Rectangle(coords[currentSpriteIndex].x, coords[currentSpriteIndex].y, slicedBy, slicedBy),
                origin.ToXnaVector2(), (transform.position + texture2D.offset).ToXnaVector2(),
                transform.rotation,
                transform.scale.ToXnaVector2(), new Color(255, 255, 255,(byte)(opacity * 255)), GetFlip(), layer);
        }
    }

    public class RegionSprite :Sprite
    {
        public alice.engine.maths.Rectangle region;

        public RegionSprite(Texture2D texture2D)
        {
            this.texture2D = texture2D;
            region = new maths.Rectangle(0, 0, texture2D.width, texture2D.height);
            CalculateOrigin();
        }

        public RegionSprite(Texture2D texture2D, alice.engine.maths.Rectangle region)
        {
            this.texture2D = texture2D;
            this.region = region;
            CalculateOrigin();
        }

        internal override void Draw(Sprites spritesBatch, Transform transform)
        {
            if (region == null) return;
            spritesBatch.Draw(texture2D.texture2D, region.ToXnaRectangle(), origin.ToXnaVector2(),
                            (transform.position + texture2D.offset).ToXnaVector2(),
                            transform.rotation, transform.scale.ToXnaVector2(), new Color(255, 255, 255, (byte)(opacity * 255)), GetFlip(), layer);

            spritesBatch.shapes.DrawCircleOutline(new MonoGame.Extended.CircleF(new MonoGame.Extended.Point2(origin.X, origin.Y), 5), Color.Magenta, 5);
            spritesBatch.shapes.DrawCircleOutline(new MonoGame.Extended.CircleF(new MonoGame.Extended.Point2(transform.position.X, transform.position.Y), 5), Color.Orange, 5);
        }

    }

    internal class MultipleSpriteCoords
    {
        public int x, y;
    }

}
