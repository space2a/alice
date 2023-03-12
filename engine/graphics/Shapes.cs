using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;

using System;
using System.Reflection;


namespace alice.engine
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

            const int maxVertexCount = 8192;
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

            isStarted = false;
        }

        public bool isDrawingInCanvas = false;

        public void Flush()
        {
            BasicEffect be = basicEffect;

            if (isDrawingInCanvas) be = emptyEffect;
            if (shapeCount == 0) { Console.WriteLine("unable to flush shapeCount is zero"); return; }

            EnsureStarted();

            foreach (EffectPass pass in be.CurrentTechnique.Passes)
            {
                pass.Apply();
                zcore.GraphicsDevice.DrawUserIndexedPrimitives
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

        public void DrawFilledRectangle(Microsoft.Xna.Framework.Rectangle rectangle, Color color)
        {
            basicEffect.Alpha = (float)color.color.A / 255;
            DrawFilledRectangleGradient(rectangle, new Gradient(color, color, color, color));
        }

        public void DrawFilledRectangleGradient(Microsoft.Xna.Framework.Rectangle rectangle, Gradient gradient)
        {
            EnsureStarted();
            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;
            EnsureSpace(shapeVertexCount, shapeIndexCount);

            float left = rectangle.X;
            float right = rectangle.X + rectangle.Width;

            float top = rectangle.Y + rectangle.Height;
            float bottom = rectangle.Y;

            var a = new Microsoft.Xna.Framework.Vector2(left, top);
            var b = new Microsoft.Xna.Framework.Vector2(right, top);
            var c = new Microsoft.Xna.Framework.Vector2(right, bottom);
            var d = new Microsoft.Xna.Framework.Vector2(left, bottom);

            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 1 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 3 + vertexCount;

            vertices[vertexCount++] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(a, 0f), gradient[0].color);
            vertices[vertexCount++] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(b, 0f), gradient[1].color);
            vertices[vertexCount++] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(c, 0f), gradient[2].color);
            vertices[vertexCount++] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(d, 0f), gradient[3].color);

            shapeCount++;
            basicEffect.Alpha = (float)gradient[0].color.A / 255;
            Flush();
        }

        public void DrawRectangleOutline(Microsoft.Xna.Framework.Rectangle rectangle, Color color, int thickness)
        {
            spriteBatch.DrawRectangle(rectangle, color.color, thickness, 0);
        }

        public void DrawLine(float ax, float ay, float bx, float by, float thickness, Color color, bool flush = true)
        {
            DrawLineGradient(ax, ay, bx, by, thickness, new Gradient(color, color, color, color), flush);
        }

        public void DrawLineGradient(float ax, float ay, float bx, float by, float thickness, Gradient gradient, bool flush = true)
        {
            EnsureStarted();
            const int shapeVertexCount = 4;
            const int shapeIndexCount = 6;
            EnsureSpace(shapeVertexCount, shapeIndexCount);

            if (thickness < 0) thickness = 0;

            float halfThickness = thickness / 2;

            float e1x = bx - ax;
            float e1y = by - ay;

            MathU.Normalize(ref e1x, ref e1y);

            e1x *= halfThickness;
            e1y *= halfThickness;

            float e2x = -e1x;
            float e2y = -e1y;

            float n1x = -e1y;
            float n1y = e1x;

            float n2x = -n1x;
            float n2y = -n1y;

            float q1x = ax + n1x + e2x;
            float q1y = ay + n1y + e2y;

            float q2x = bx + n1x + e1x;
            float q2y = by + n1y + e1y;

            float q3x = bx + n2x + e1x;
            float q3y = by + n2y + e1y;

            float q4x = ax + n2x + e2x;
            float q4y = ay + n2y + e2y;

            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 1 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 0 + vertexCount;
            indices[indexCount++] = 2 + vertexCount;
            indices[indexCount++] = 3 + vertexCount;

            vertices[vertexCount++] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(q1x, q1y, 0), gradient[0].color);
            vertices[vertexCount++] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(q2x, q2y, 0), gradient[1].color);
            vertices[vertexCount++] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(q3x, q3y, 0), gradient[2].color);
            vertices[vertexCount++] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(q4x, q4y, 0), gradient[3].color);


            shapeCount++;
            if(flush)
                Flush();
        }

        internal void DrawCircleOutline(CircleF circle, Color color, int thickness)
        {
            spriteBatch.DrawCircle(circle, 10, color.color, thickness, 0);
        }

        public void DrawCircle(float x, float y, float radius, int points, float thickness, Color color)
        {
            if (points < 3) points = 3;

            float rotation = MathHelper.TwoPi / (float)points;
            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            float ax = radius;
            float ay = 0f;

            float bx = 0;
            float by = 0;

            for (int i = 0; i < points; i++)
            {
                bx = cos * ax - sin * ay;
                by = sin * ax + cos * ay;

                this.DrawLine(ax + x, ay + y, bx + x, by + y, thickness, color);

                ax = bx;
                ay = by;
            }
            Flush();
        }

        public void DrawCircleFilled(float x, float y, float radius, int points, Color color)
        {
            if (points < 3) points = 3;

            EnsureStarted();

            int shapeVertexCount = points;
            int shapeTriangleCount = shapeVertexCount - 2;
            int shapeIndexCount = shapeTriangleCount * 3;
            EnsureSpace(shapeVertexCount, shapeIndexCount);

            int index = 1;

            for (int i = 0; i < shapeTriangleCount; i++)
            {
                this.indices[this.indexCount++] = 0 + this.vertexCount;
                this.indices[this.indexCount++] = index + this.vertexCount;
                this.indices[this.indexCount++] = index + 1 + this.vertexCount;

                index++;
            }

            float rotation = MathHelper.TwoPi / (float)points;
            float sin = MathF.Sin(rotation);
            float cos = MathF.Cos(rotation);

            float ax = radius;
            float ay = 0f;

            for (int i = 0; i < shapeVertexCount; i++)
            {
                float x1 = ax;
                float y1 = ay;

                this.vertices[this.vertexCount++] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(x1 + x, y1 + y, 0f), color.color);

                ax = cos * x1 - sin * y1;
                ay = sin * x1 + cos * y1;
            }

            this.shapeCount++;
            Flush();
        }

        public void DrawPolygon(Vector2[] vertices, float thickness, Color[] color, Vector2 origin = null)
        {
            if (origin == null) origin = Vector2.Zero;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 a = origin + vertices[i];
                Vector2 b = origin + vertices[(i + 1) % vertices.Length];

                this.DrawLine(a.X, a.Y, b.X, b.Y, thickness, color[i], false);
            }

            Flush();
        }

        public void DrawPolygonFilled(Vector2[] vertices, int[] triangleIndices, Color[] color)
        {
            if (vertices == null) throw new Exception("Vertices cannot be null.");

            for (int i = 0; i < triangleIndices.Length; i++)
            {
                this.indices[this.indexCount++] = triangleIndices[i] + this.vertexCount;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                this.vertices[this.vertexCount++] = new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(vertices[i].X, vertices[i].Y, 0f), color[i].color);
            }

            this.shapeCount++;
            //core.GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.WireFrame };
            Flush();
        }

    }
}
