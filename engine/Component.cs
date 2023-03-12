namespace alice.engine
{
    public abstract class Component
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
