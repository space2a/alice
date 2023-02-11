using alice.engine.graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alice.engine.uicomponents
{
    public class Label : UIComponent
    {

        public int scale = 1;
        public graphics.Color TextColor = graphics.Color.White;

        public Font font = FontManager.defaultFont;

        private Vector2 textv2;
        private string _text = "Text";
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                textv2 = font.font.MeasureString(value);
            }
        }

        internal override Rectangle GetElementRectangle()
        {
            return new Rectangle((int)transform.position.X, (int)transform.position.Y, (int)textv2.X, (int)textv2.Y);
        }

        public Label(string text = null)
        {
            if (text != null)
                this.text = text;
        }

        internal override void DrawUI(Sprites spritesBatch)
        {
            spritesBatch.DrawString(font.font, text, transform.position.ToXnaVector2(), TextColor.color, 0, Vector2.Zero, scale);

            base.Draw(spritesBatch);
        }

    }
}
