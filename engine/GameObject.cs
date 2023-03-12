using System;
using System.Collections.Generic;

namespace alice.engine
{
    [Serializable]
    public class GameObject
    {
        public string name = "";
        public string tag = "";
            
        public bool isDisabled = false;
        public bool isRendering = true;

        public bool isInCanvas { get; internal set; }

        public readonly Transform transform = new Transform();

        private List<Component> components = new List<Component>();

        public List<GameObject> childrens { get; private set; }

        public GameObject parent { get; private set; }

        public static string[] tags { get; private set; }

        public GameObject()
        {
            transform.gameObject = this;
            GetNameByAttribute();       
        }

        public GameObject(string name)
        {
            transform.gameObject = this;
            this.name = name;
        }

        public GameObject(Component component)
        {
            transform.gameObject = this;
            GetNameByAttribute();
            AddComponent(component);
        }

        public GameObject(List<Component> components)
        {
            transform.gameObject = this;
            GetNameByAttribute();
            for (int i = 0; i < components.Count; i++)
                AddComponent(components[i]);
        }

        private void GetNameByAttribute()
        {
            GameObjectName goName =
            (GameObjectName)Attribute.GetCustomAttribute(this.GetType(), typeof(GameObjectName));
            if(goName != null) { this.name = goName.Name; }
        }

        public void AddChildren(GameObject gameObject)
        {
            if (childrens == null) childrens = new List<GameObject>();
            childrens.Add(gameObject);
            gameObject.parent = this;
            gameObject.Start();
        }

        public virtual void Start()
        {

        }

        internal void PreUpdate(GameTime gameTime)
        {
            if (isDisabled) return;
            foreach (var component in components) { component.Update(gameTime); } //for loop
            if(childrens != null)
                foreach (var child in childrens) { child.Update(gameTime); }
            Update(gameTime);
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        internal void PreDestroy()
        {
            foreach (var c in components)
            {
                c.Destroy();
            }

            if (childrens != null)
                foreach (var child in childrens) { child.PreDestroy(); }

            SceneManager.currentScene.gameObjects?.Remove(this);

            if (isDisabled) return;
            OnDestroy();
        }

        public virtual void OnDestroy()
        {

        }

        public void Destroy()
        {
            PreDestroy();
        }


        internal void Draw(Sprites spritesBatch)
        {
            if (!isRendering) return;

            for (int i = 0; i < components.Count; i++)
            {
                //Console.WriteLine("drawing comp " + components[i].GetType() + " from " + name);
                components[i].Draw(spritesBatch);
            }

            if(childrens != null)
            {
                for (int i = 0; i < childrens.Count; i++)
                {
                    childrens[i].Draw(spritesBatch);
                }
            }
        }

        internal void DrawUI(Sprites spritesBatch)
        {
            if (!isRendering) return;

            for (int i = 0; i < components.Count; i++)
            {
                components[i].PreDrawUI(spritesBatch);
            }

            if (childrens != null)
            {
                for (int i = 0; i < childrens.Count; i++)
                {
                    childrens[i].DrawUI(spritesBatch);
                }
            }
        }

        public virtual void OnCollisionStay(CollidingInput collidingInput) { }
        public virtual void OnCollisionEnter(CollidingInput collidingInput) { }
        public virtual void OnCollisionExit(CollidingInput collidingInput) { }

        public void AddComponent(Component component)
        {
            if (GetComponent<Component>() != null && Attribute.IsDefined(component.GetType(), typeof(UniqueComponent)))
            {
                throw new Exception("Unable to add a second component of type " + component.GetType() + ", this component is marked at unique.");
            }

            component.gameObject = this;
            component.Start();
            components.Add(component);
        }

        public void AddComponents(Component first, Component second, Component third)
        {
            AddComponent(first);
            AddComponent(second);
            AddComponent(third);
        }

        public void AddComponents(Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                AddComponent(components[i]);
            }
        }


        public void AddComponent(Component component, out Component outComponent)
        {
            AddComponent(component);
            outComponent = component;
        }

        public T GetComponent<T>() where T : Component
        {
            return (T)components.Find(x => x.GetType() == typeof(T));
        }

        public T[] GetComponents<T>() where T : Component
        {
            return (T[])components.FindAll(x => x.GetType() == typeof(T)).ToArray();
        }

        public Component[] GetComponents() 
        {
            return components.ToArray();
        }

        public bool RemoveComponent<Component>(bool destroy = false)
        {
            int index = components.FindIndex(x => x.GetType() == typeof(Component));
            if (index != -1)
            {
                if (destroy) { components[index].Destroy(); }
                components.RemoveAt(index);
                return true;
            }
            else return false;
        }

        public bool RemoveComponent(Component component, bool destroy = false)
        {
            int index = components.IndexOf(component);
            if (index != -1)
            {
                if(destroy) { components[index].Destroy(); }
                components.RemoveAt(index);
                return true;
            }
            else return false;
        }

        public void RemoveAllComponent(bool destroy = false)
        {
            if (destroy)
                for (int i = 0; i < components.Count; i++)
                    components[i].Destroy();

            components.Clear();
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class PersistentGameObject : System.Attribute { }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class GameObjectName : System.Attribute
    {
        public string Name;
        public GameObjectName(string name) { Name = name; }
    }
}
