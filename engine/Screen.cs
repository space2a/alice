﻿using System;


using Microsoft.Xna.Framework.Graphics;

namespace alice.engine
{
    internal class Screen : IDisposable
    {
         
        private RenderTarget2D target2D;
        private bool isDisposed;

        private bool isSet = false;

        public readonly int width, height;

        public Screen(int width, int height)
        {
            width = MathU.Clamp(width, 64, 8196);
            height = MathU.Clamp(height, 64, 8196);

            this.width = width;
            this.height = height;

            target2D = new RenderTarget2D(Launcher.core.GraphicsDevice, width, height);
        }

        public void Dispose()
        {
            if(isDisposed) return;
            target2D.Dispose();
            isDisposed = true;
            Console.WriteLine("screen disposed");
        }

        public void Set()
        {
            Launcher.core.GraphicsDevice.SetRenderTarget(target2D);
            isSet = true;
        }

        public void UnSet(Sprites spritesBatch, Camera activeCamera)
        {
            spritesBatch.Begin(null, true, true);

            engine.Debugging.DrawUI(spritesBatch);
            activeCamera.DrawCanvas(spritesBatch);

            spritesBatch.shapes.DrawRectangleOutline(UIComponent.windowMouseRectangle, engine.Color.Random, 5);

            spritesBatch.End();

            Launcher.core.GraphicsDevice.SetRenderTarget(null);
            isSet = false;
        }

        public void Present(Sprites spritesBatch)
        {
            Launcher.core.GraphicsDevice.Clear(new Microsoft.Xna.Framework.Color(50, 0, 0));
            spritesBatch.Begin(null, true);
            spritesBatch.Draw(target2D, null, CalculateDestinationRectangle(), Color.White);
            spritesBatch.End();
        }

        internal Microsoft.Xna.Framework.Rectangle CalculateDestinationRectangle()
        {
            Microsoft.Xna.Framework.Rectangle backbufferBounds = Launcher.core.GraphicsDevice.PresentationParameters.Bounds;
            float backbufferAspectRatio = (float)backbufferBounds.Width / backbufferBounds.Height;
            float screenAspectRatio = (float)width / height;

            float rx = 0f;
            float ry = 0f;
            float rw = backbufferBounds.Width;
            float rh = backbufferBounds.Height;


            if (backbufferAspectRatio > screenAspectRatio) 
            {
                rw = rh * screenAspectRatio;
                rx = ((float)backbufferBounds.Width - rw) / 2f;
            }
            else if(backbufferAspectRatio < screenAspectRatio)
            {
                rh = rw / screenAspectRatio;
                ry = ((float)backbufferBounds.Height - rh) /2;
            }

            Microsoft.Xna.Framework.Rectangle result = new Microsoft.Xna.Framework.Rectangle((int)rx, (int)ry, (int)rw, (int)rh);
            return result;
        }

    }
}
