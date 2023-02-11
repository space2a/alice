using System;
using System.Collections.Generic;
using System.Drawing;

using alice.engine.components;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;

using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace alice.engine.graphics
{
    internal class Shapes : IDisposable
    {
        private bool isDisposed;
        internal BasicEffect basicEffect;

        private VertexPositionColor[] vertices;
        private int[] indices;

        private int shapeCount;
        private int vertexCount;
        private int indexCount;

        public bool isStarted { get; private set; }

        private Core core;
        public SpriteBatch spriteBatch;

        public BasicEffect emptyEffect;

        public List<DebuggingAction> debuggingActions = new List<DebuggingAction>();

        internal Shapes(Core core)
        {
            this.core = core;

            basicEffect = new BasicEffect(core.GraphicsDevice);
            basicEffect.TextureEnabled = false;
            basicEffect.FogEnabled = false;
            basicEffect.LightingEnabled = false;
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.Identity;

            emptyEffect = new BasicEffect(core.GraphicsDevice)
            {
                Projection = Matrix.Identity,
                View = Matrix.Identity,
                World = Matrix.Identity,
                VertexColorEnabled = true,
                LightingEnabled = false,
                TextureEnabled = false,
                FogEnabled = false,
                Alpha= 1
            };

            const int maxVertexCount = 2048;
            const int MaxIndexCount = maxVertexCount * 3;
            vertices = new VertexPositionColor[maxVertexCount];
            indices = new int[MaxIndexCount];

            shapeCount = 0;
            vertexCount = 0;
            indexCount = 0;

            isStarted = false;
        }

        public void Dispose()
        {
            if (isDisposed) return;
            basicEffect?.Dispose();
            emptyEffect?.Dispose();
            isDisposed = true;
        }

        public void Begin(Camera camera)
        {
            if (isStarted) throw new Exception("already started");

            basicEffect.View = camera.view;

            basicEffect.Projection = camera.proj;

            isStarted = true;
        }

        public void End()
        {
            //Flush();

            for (int i = 0; i < debuggingActions.Count; i++)
            {
                debuggingActions[i].action(debuggingActions[i].parameter, debuggingActions[i].parameter2);
            }

            isStarted = false;
        }

        public bool isDrawingInCanvas = false;

        public void Flush()
        {
            BasicEffect be = basicEffect;
            if (isDrawingInCanvas) be = emptyEffect;
            if (shapeCount == 0) { Console.WriteLine("unable to flush shapeCount is zero " + DateTime.Now); return; }
            EnsureStarted();

            foreach (EffectPass pass in be.CurrentTechnique.Passes)
            {
                pass.Apply();
                core.GraphicsDevice.DrawUserIndexedPrimitives
                    (PrimitiveType.TriangleList, vertices, 0, vertexCount, indices, 0, indexCount / 3);
                //Console.WriteLine(DateTime.Now.Millisecond.ToString("000")  + " SHAPE FLUSH (" + shapeCount + ")");
            }

            shapeCount = 0;
            vertexCount = 0;
            indexCount = 0;
        }

        public void EnsureStarted()
        {
            //if (!isStarted) throw new Exception("not started");
        }

        public void EnsureSpace(int vertexCount, int indexCount)
        {
            if (vertexCount > vertices.Length) throw new Exception("max shape vertex count is " + vertices.Length);
            if (indexCount > indices.Length) throw new Exception("max shape index count is " + indices.Length);

            if (this.vertexCount + vertexCount > vertices.Length
                || this.indexCount + indexCount > vertices.Length) { Console.WriteLine("EnsureSpace Flush"); Flush(); }
        }

        public void SetMatrixToDefault()
        {
            emptyEffect.Projection = 
                Matrix.CreateOrthographicOffCenter(0, core.GraphicsDevice.Viewport.Width, 
                0, core.GraphicsDevice.Viewport.Height, 0, 10);
        }

        public void DrawFilledRectangleGradient(Rectangle rectangle, Gradient gradient)
        {
            EnsureStarted();
            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;
            EnsureSpace(shapeVertexCount, shapeIndexCount);

            float left = rectangle.X;
            float right = rectangle.X + rectangle.Width;

            float top = rectangle.Y + rectangle.Height;
            float bottom = rectangle.Y;

            Vector2 a = new Vector2(left, top);
            Vector2 b = new Vector2(right, top);
            Vector2 c = new Vector2(right, bottom);
            Vector2 d = new Vector2(left, bottom);

            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 1 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 3 + vertexCount;

            vertices[vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), gradient[0].color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), gradient[1].color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), gradient[2].color);
            vertices[vertexCount++] = new VertexPositionColor(new Vector3(d, 0f), gradient[3].color);

            shapeCount++;
            basicEffect.Alpha = (float)gradient[0].color.A / 255;
            Flush();
        }

        public void DrawFilledRectangle(Rectangle rectangle, Color color)
        {
            basicEffect.Alpha = (float)color.color.A / 255;
            DrawFilledRectangleGradient(rectangle, new Gradient(color, color, color, color));
        }

        public void DrawRectangleOutline(Rectangle rectangle, Color color, int thickness)
        {
            spriteBatch.DrawRectangle(rectangle, color.color, thickness, 0);
        }

        internal void DrawRectangleOutline(MonoGame.Extended.RectangleF rectangle, Color color, int thickness)
        {
            spriteBatch.DrawRectangle(rectangle, color.color, thickness, 0);
        }

        internal void DrawCircleOutline(CircleF circle, Color color, int thickness)
        {
            spriteBatch.DrawCircle(circle, 10, color.color, thickness, 0);
        }

    }

    internal class DebuggingAction
    {
        public Action<object, object> action;
        public Object parameter;
        public Object parameter2;
    }
}
