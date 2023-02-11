using System.Collections.Generic;

using alice.engine.graphics;
using alice.engine.maths;

namespace alice.engine.debugging
{
    public static class Debugging
    {
        public static List<DrawingObject> drawingObjects = new List<DrawingObject>();
        public static List<DrawingObject> drawingObjectsUI = new List<DrawingObject>();

        private static Sprites spriteBatch
        {
            get
            {
                return Launcher.core.spritesBatch;
            }
        }

        public static void DrawRectangle(Rectangle rectangle, Color color)
        {
            drawingObjects.Add(new DrawingObject(rectangle, color));
        }

        public static void DrawRectangleUI(Rectangle rectangle, Color color)
        {
            drawingObjectsUI.Add(new DrawingObject(rectangle, color));
        }


        internal static void Draw(Sprites spriteBatch)
        {
            for (int i = 0; i < drawingObjects.Count; i++)
            {
                spriteBatch.shapes.DrawRectangleOutline(drawingObjects[i].rect, drawingObjects[i].color, 10);
                drawingObjects.RemoveAt(i);
            }
        }

        internal static void DrawUI(Sprites spriteBatch)
        {
            for (int i = 0; i < drawingObjectsUI.Count; i++)
            {
                spriteBatch.shapes.DrawFilledRectangle(drawingObjectsUI[i].rect, drawingObjectsUI[i].color);
                drawingObjectsUI.RemoveAt(i);
            }
        }

    }

    public class DrawingObject
    {
        public Microsoft.Xna.Framework.Rectangle rect;
        public Color color;

        public DrawingObject(Rectangle rect, Color color)
        {
            if (rect == null) rect = new Rectangle(0, 0, 0, 0);
            this.rect = rect.ToXnaRectangle();
            this.color = color;
        }
    }

}
