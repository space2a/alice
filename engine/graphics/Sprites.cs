using alice.engine.components;
using alice.engine.maths;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using MonoGame.Extended;
using System.Reflection.Metadata.Ecma335;
using System.Drawing;
using Microsoft.Xna.Framework.Input;

namespace alice.engine.graphics
{
    internal class Sprites : IDisposable
    {
        private SpriteBatch sprites;

        private bool isDisposed;

        internal BasicEffect basicEffect;

        public Shapes shapes;

        public Core core;
        public Camera camera;

        public RasterizerState rasterizerStateEmpty = new RasterizerState() { CullMode = CullMode.None };
        public RasterizerState rasterizerState = new RasterizerState()
        { MultiSampleAntiAlias = false, ScissorTestEnable = true, CullMode = CullMode.None };

        public Sprites(Core core)
        {
            sprites = new SpriteBatch(core.GraphicsDevice);
            shapes = new Shapes(core);
            shapes.spriteBatch = sprites;
            this.core = core;

            basicEffect = new BasicEffect(core.GraphicsDevice)
            {
                FogEnabled = false,
                TextureEnabled = true,
                LightingEnabled = false,
                VertexColorEnabled = true,
                Alpha = 1,
                World = Matrix.Identity,
                Projection = Matrix.Identity,
                View = Matrix.Identity,
            };
        }

        private int deltaScrollWheelValue = 0;
        private int currentScrollWheelValue = 0;
        private int size = 200;

        public void Begin(Camera camera, bool isTextureFilteringEnabled, bool unset = false)
        {
            SamplerState samplerState = SamplerState.PointClamp;

            var rasterizer = rasterizerStateEmpty;

            if (isTextureFilteringEnabled)
                samplerState = SamplerState.LinearClamp;

            if (camera == null)
            {
                Viewport vp = Launcher.core.GraphicsDevice.Viewport;
                basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0, 1);
                basicEffect.View = Matrix.Identity;
            }
            else
            {
                this.camera = camera;
                camera.UpdateMatrices();
                basicEffect.View = camera.view;
                basicEffect.Projection = camera.proj;
            }

            if (unset)
            {
                rasterizer = rasterizerState;
            }

            sprites.Begin(blendState: BlendState.AlphaBlend, samplerState: samplerState, rasterizerState: rasterizer, effect: basicEffect, sortMode: SpriteSortMode.Immediate);

            if (unset)
            {
                deltaScrollWheelValue = Inputs.MouseState.currentState.ScrollWheelValue - currentScrollWheelValue;
                currentScrollWheelValue += deltaScrollWheelValue;

                if (deltaScrollWheelValue > 0) size += 5;
                else if (deltaScrollWheelValue < 0) size -= 5;

                Console.WriteLine("delta : " + deltaScrollWheelValue);
                Console.WriteLine("current : " + currentScrollWheelValue);
                

                //Rectangle scissorRectangle = new Rectangle((int)Inputs.MouseState.windowMousePosition.X + size, (int)Inputs.MouseState.windowMousePosition.Y, 
                //    size, size);
                //
                //SetScissorRectangle(scissorRectangle);
                
                //Draw(new Texture2D(@"C:\Users\Aaron\Desktop\jxpchez13oaa1.png").texture2D, Vector2.Zero, Vector2.Zero, Color.White);
                //
                //ResetScissorRectangle();
            }
        }

        public void BeginUI()
        {
            sprites.Begin(rasterizerState: rasterizerState, effect: basicEffect, sortMode: SpriteSortMode.Immediate);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.Texture2D texture2D, Vector2 origin, Vector2 position, Color color, SpriteEffects flip = SpriteEffects.FlipVertically, int layer = 0)
        {
            sprites.Draw(texture2D, position, null, color.color, 0f, origin, 1f, flip, layer);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.Texture2D texture2D, Rectangle? sourceRectangle, Vector2 origin, Vector2 position, float rotation, Vector2 scale, Color color, SpriteEffects flip = SpriteEffects.FlipVertically, int layer = 0)
        {
            sprites.Draw(texture2D, position, sourceRectangle, color.color, rotation, origin, scale, flip, layer);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.Texture2D texture2D, Rectangle? sourceRectangle, Rectangle destinationRectangle, Color color, SpriteEffects flip = SpriteEffects.FlipVertically, int layer = 0)
        {
            sprites.Draw(texture2D, destinationRectangle, sourceRectangle, color.color, 0f, Vector2.Zero, flip, layer);
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.Texture2D texture2D, Rectangle destinationRectangle, Microsoft.Xna.Framework.Color color, SpriteEffects flip = SpriteEffects.FlipVertically, int layer = 0)
        {
            sprites.Draw(texture2D, destinationRectangle, null, color, 0, Vector2.Zero, flip, layer);
        }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Microsoft.Xna.Framework.Color color, SpriteEffects flip = SpriteEffects.FlipVertically, int layer = 0, float scale = 1)
        {
            sprites.DrawString(spriteFont, text, position, color, 0, Vector2.Zero, scale, flip, layer);
        }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Microsoft.Xna.Framework.Color color, float rotation, Vector2 origin, float scale, SpriteEffects flip = SpriteEffects.FlipVertically, int layer = 0)
        {
            sprites.DrawString(spriteFont, text, position, color, rotation, origin, scale, flip, layer);
        }

        public void SetScissorRectangle(Rectangle rect)
        {
            Rectangle r = new Rectangle(
                rect.X, core.windowProfile.renderedResolution.height - rect.Y - rect.Height,
                rect.Width, rect.Height);

            core.GraphicsDevice.ScissorRectangle = r;
        }

        public void ResetScissorRectangle()
        {
            core.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, core.GraphicsDevice.Viewport.Width, core.GraphicsDevice.Viewport.Height);
        }

        public void End()
        {
            //shapes.End();
            sprites.End();
        }

        public void Dispose()
        {
            if (isDisposed) return;
            shapes.Dispose();
            sprites.Dispose();
            basicEffect.Dispose();
            isDisposed = true;
            Console.WriteLine("sprites disposed");
        }

    }
}
