using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using alice.engine.graphics;

namespace alice.engine.uicomponents
{
    public class Image : UIComponent
    {
        public Sprite sprite;

        public Image()
        {

        }

        public Image(Sprite sprite)
        {
            this.sprite = sprite;
        }

        internal override void DrawUI(Sprites spritesBatch)
        {
            sprite?.Draw(spritesBatch, transform);
        }

        internal override void DestroyComponent()
        {
            sprite?.texture2D.Dispose();
        }

    }
}
