using alice.engine.graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace alice.engine
{
    public class Component
    {
        internal GameObject gameObject;
        internal Transform transform
        {
            get
            {
                if (gameObject != null)
                    return gameObject.transform;
                else return null;
            }
        }

        internal virtual void Start()
        {

        }

        internal virtual void Update(GameTime gameTime)
        {

        }

        internal virtual void Draw(Sprites spritesBatch)
        {

        }

        internal virtual void PreDrawUI(Sprites spritesBatch)
        {
            DrawUI(spritesBatch);
        }

        internal virtual void DrawUI(Sprites spritesBatch)
        {

        }

        internal virtual void DestroyComponent()
        {

        }

        public void Destroy()
        {
            DestroyComponent();
        }

    }


    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class UniqueComponent : System.Attribute { }
}
