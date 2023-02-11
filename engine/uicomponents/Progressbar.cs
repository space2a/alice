﻿using alice.engine.graphics;
using alice.engine.internals;
using alice.engine.maths;

using Color = alice.engine.graphics.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace alice.engine.uicomponents
{
    public class Progressbar : UIComponent
    {

        private float _progress = 0;
        public float progress
        {
            get { return _progress; }
            set
            {
                if (value > maximum)
                    value = maximum;
                else if (value < 0)
                    value = 0;

                _progress = value;
                progressRectangle.Width = (int)(value * ElementRectangle.Width / maximum);
            }
        }

        public float maximum = 100;

        public bool isIndeterminate = false;

        public Texture2D backgroundTexture = LoadEmbeddedResources.LoadTexture("progressbar.png", "images");
        public Texture2D progressTexture;

        private Rectangle progressRectangle = new Rectangle();
        private Rectangle indeterminateRectangle = new Rectangle(0, 0, 20, 20);

        public bool useShape = false;
        public Color backgroundRectangleColor = new Color(30, 30, 30);
        public Color progressRectangleColor = new Color(66, 164, 245);

        public int indeterminateSpeed = 5;

        public Progressbar()
        {
            useShape = true;
        }

        public Progressbar(bool isIndeterminate)
        {
            this.isIndeterminate = isIndeterminate;
            useShape = true;
        }

        public Progressbar(Texture2D backgroundTexture, Texture2D progressTexture)
        {
            this.backgroundTexture = backgroundTexture;
            this.progressTexture = progressTexture;

            useShape = false;
        }


        internal override void DrawUI(Sprites spritesBatch)
        {

            if (useShape)
                spritesBatch.shapes.DrawFilledRectangle(ElementRectangle, backgroundRectangleColor);
            else
                spritesBatch.Draw(backgroundTexture.texture2D, ElementRectangle, Microsoft.Xna.Framework.Color.White);

            if (!isIndeterminate)
            {
                if (useShape)
                    spritesBatch.shapes.DrawFilledRectangle(progressRectangle, progressRectangleColor);
                else
                    spritesBatch.Draw(progressTexture.texture2D, progressRectangle, Microsoft.Xna.Framework.Color.White);
            }
            else
            {
                if (useShape)
                    spritesBatch.shapes.DrawFilledRectangle(CreateIndeterminateRectangle(), progressRectangleColor);
                else
                    spritesBatch.Draw(progressTexture.texture2D, CreateIndeterminateRectangle(), Microsoft.Xna.Framework.Color.White);
            }

            base.Draw(spritesBatch);
        }

        private int indeterminateProgress = 0;
        internal override void Update(GameTime gameTime)
        {
            if (isIndeterminate)
            {
                indeterminateProgress += (int)(100 * indeterminateSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            }
            //if (progress >= 100) progress = 0;
            //Console.WriteLine(progress + " / " + progressRectangle.Width);
        }

        public Rectangle CreateIndeterminateRectangle()
        {
            var erectangle = ElementRectangle;

            indeterminateRectangle.Width = MathU.CalculPourcent(erectangle.Width, 20);
            indeterminateRectangle.X += indeterminateProgress;

            if (indeterminateRectangle.X + indeterminateRectangle.Width >= erectangle.X + erectangle.Width)
                indeterminateProgress = 0;

            return indeterminateRectangle;
        }

        internal override Rectangle GetElementRectangle()
        {
            var rec = new Rectangle((int)transform.position.X,
            (int)transform.position.Y,
            (int)(backgroundTexture.texture2D.Width + 300 * transform.scale.X), (int)(backgroundTexture.texture2D.Height * transform.scale.Y));

            progressRectangle.X = rec.X;
            progressRectangle.Y = rec.Y;
            progressRectangle.Height = rec.Height;

            indeterminateRectangle = rec;
            indeterminateRectangle.Width = 100;

            return rec;
        }

        internal override void DestroyComponent()
        {
            if (progressTexture != null)
                progressTexture.Dispose();
            backgroundTexture.Dispose();
        }
    }
}