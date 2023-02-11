﻿using System;

using alice.engine.graphics;

using Microsoft.Xna.Framework;

using Color = alice.engine.graphics.Color;

namespace alice.engine.components
{
    public class SpriteRenderer : Component
    {
        public Sprite sprite;

        public SpriteRenderer()
        {

        }

        public SpriteRenderer(Sprite sprite)
        {
            this.sprite = sprite;
        }

        internal override void Draw(Sprites spritesBatch)
        {
            //Console.WriteLine(DateTime.Now.Millisecond.ToString("000")  + " SPRITE DRAW");
            if (sprite != null)
                sprite.Draw(spritesBatch, transform);
        }

        internal override void DestroyComponent()
        {
            sprite?.texture2D.Dispose();
        }

    }
}
