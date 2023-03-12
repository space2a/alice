using System;
using System.Collections.Generic;
using System.Threading;

namespace alice.engine
{
    public class Scene
    {
        public string sceneName = "";
        public List<GameObject> gameObjects = new List<GameObject>();

        public Scenery scenery = new ColorizedScenery(Color.Black);

        public Camera sceneCamera = null;

        internal bool startedCalled = false;

        public Scene()
        {
            
        }

        public Scene(string sceneName)
        {
            CreateScene(sceneName);
        }
       
        public virtual void OnCreation()
        {

        }

        internal void OnCreationCall(DateTime dateTime)
        {
            int bad_gobjs = gameObjects.Count;
            new Thread(() =>
            {
                OnCreation();
                Launcher.core.loadingScene = false;
                Console.WriteLine("scene loaded in " + (DateTime.Now - dateTime).TotalSeconds + "s with " +
                (gameObjects.Count - bad_gobjs) + " properly added gobjs.");
            }).Start();
        }

        public void AddGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
            if (startedCalled)
                gameObject.Start();
        }

        public void AddGameObjects(List<GameObject> gameObjects)
        {
            Console.WriteLine("adding " + gameObjects.Count + " gobjs");
            gameObjects.AddRange(gameObjects);
        }

        public void InsertGameObject(GameObject gameObject, int index)
        {
            gameObjects.Insert(index, gameObject);
            if (startedCalled)
                gameObject.Start();
        }

        public bool CreateScene(string sceneName)
        {
            if (SceneManager.scenes.FindIndex(x => x.sceneName == sceneName) != -1)
            { throw new Exception("A scene with the same name already exists."); }
            this.sceneName = sceneName;
            SceneManager.scenes.Add(this);
            Console.WriteLine("new scene..." + sceneName);
            return true;
        }

        public GameObject GetGameObjectByName(string name)
        {
            return gameObjects.Find(x => x.name == name);
        }

        public GameObject[] GetGameObjectsByName(string name)
        {
            return gameObjects.FindAll(x => x.name == name).ToArray();
        }

        public GameObject GetGameObjectByTag(string tag)
        {
            return gameObjects.Find(x => x.tag == tag);
        }

        public GameObject[] GetGameObjectsByTag(string tag)
        {
            return gameObjects.FindAll(x => x.tag == tag).ToArray();
        }

        public void Destroy()
        {
            if (gameObjects != null)
                for (int i = 0; i < gameObjects.Count; i++)
                    gameObjects[i].PreDestroy();

            Console.WriteLine("scene destroy function");
            SceneManager.scenes.Remove(this);
        }
    }
}
