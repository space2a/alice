using System;
using System.Text;

using alice.engine;
using alice.engine.components;
using alice.engine.graphics;
using alice.engine.internals;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.Collisions;

using GameTime = Microsoft.Xna.Framework.GameTime;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace alice
{
    internal class Core : Game
    {

        internal CollisionComponent _collisionComponent;

        internal GraphicsDeviceManager _graphics;

        internal Sprites spritesBatch;

        internal bool loadingScene = true;
        private bool isInitialized = false;

        public Scene loadedScene;

        public WindowProfile windowProfile { get; private set; }
        public Screen screen;

        private Camera noCameraCam;
        private Camera getMainCamera
        {
            get
            {
                if (loadedScene == null)
                    return noCameraCam;
                else if (loadedScene.sceneCamera != null)
                    return loadedScene.sceneCamera;
                else if (SceneLoader.loadingScene != null && SceneLoader.loadingScene.sceneCamera != null && loadingScene)
                    return SceneLoader.loadingScene.sceneCamera;
                return noCameraCam;
            }
        }

        public Core(WindowProfile windowProfile)
        {
            this.windowProfile = windowProfile;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.SynchronizeWithVerticalRetrace = true;
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            IsFixedTimeStep = true;

            CreateCollisionComponent();

            this.Deactivated += Core_Deactivated;
            this.Activated += Core_Activated;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private void Core_Activated(object sender, EventArgs e)
        {
            Inputs.MouseState.safeLock = true;
        }

        private void Core_Deactivated(object sender, EventArgs e)
        {
            Inputs.MouseState.safeLock = false;
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;

            //changing the window size
            _graphics.PreferredBackBufferWidth = (int)windowProfile.windowResolution.width;
            _graphics.PreferredBackBufferHeight = (int)windowProfile.windowResolution.height;
            _graphics.ApplyChanges();

            //viewportAdapter = new BoxingViewportAdapter
            //    (Window, GraphicsDevice,
            //    (int)windowProfile.renderedResolution.X, 
            //    (int)windowProfile.renderedResolution.Y);
            //
            Window.AllowAltF4 = windowProfile.authorizeALTF4;
            Window.AllowUserResizing = windowProfile.authorizeResizing;
            Window.AllowUserResizing = true;


            screen = new Screen(windowProfile.renderedResolution.width, windowProfile.renderedResolution.height);

            spritesBatch = new Sprites(this);

            noCameraCam = new Camera();

            isInitialized = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (Launcher.coreState != Launcher.CoreState.Playing) return;

            var gt = new engine.GameTime(gameTime);
            Camera cam = getMainCamera;

            if (debuggingEnabled)
                Debugging();

            Inputs.Update();
            Inputs.MouseState.UpdateScreenPosition(screen, cam);

            try
            {
                _collisionComponent.Update(gameTime); //update the collision.
            }
            catch (Exception)
            {
                Console.WriteLine("collision engine bug");
            }

            if (loadedScene == null && SceneLoader.loadingScene != null)
            {
                CallStartsIfNeeded(SceneLoader.loadingScene);
                CallUpdates(gt, cam, SceneLoader.loadingScene);
                base.Update(gameTime); return;
            }

            if (loadingScene) return;

            CallStartsIfNeeded(loadedScene);

            CallUpdates(gt, cam, loadedScene);

            base.Update(gameTime);
        }


        private void CallUpdates(engine.GameTime gt, Camera cam, Scene scene)
        {
            for (int i = 0; i < scene.gameObjects.Count; i++)
                scene.gameObjects[i].PreUpdate(gt);

            cam.canvas?.CallUpdate(gt);
        }

        private void CallStartsIfNeeded(Scene scene)
        {
            if (!scene.startedCalled)
            {
                scene.startedCalled = true;
                for (int i = 0; i < scene.gameObjects.Count; i++)
                    scene.gameObjects[i].Start();

                scene.sceneCamera?.canvas?.CallStart();
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            if (Launcher.coreState != Launcher.CoreState.Playing) return;

            Scene currentScene = null;
            StartDrawBatchs(out Camera cam);

            if (cam == noCameraCam && loadedScene != null)
            { ScreenErrors.NoMainCamera(spritesBatch); EndDrawBatchs(currentScene, cam); return; }
            
            if (loadingScene || !loadedScene.startedCalled)
            {
                if (SceneLoader.loadingScene != null)
                    CallDraws(spritesBatch, SceneLoader.loadingScene, out currentScene); else ScreenErrors.LoadingSceneNull(spritesBatch);
            }
            else if (loadedScene != null && loadedScene.startedCalled) 
                CallDraws(spritesBatch, loadedScene, out currentScene);

            

            //spritesBatch.shapes.DrawRectangleOutline(windowProfile.boundingRectangle.ToXnaRectangle(), new engine.graphics.Color(255, 0, 0, 1), 5);
            //spritesBatch.shapes.DrawRectangleOutline(cam.XNABoundingRectangle, new engine.graphics.Color(0, 255, 0, 1), 10);
            
            //spritesBatch.shapes.DrawFilledRectangle(UIComponent.worldMouseRectangle, alice.engine.graphics.Color.Random);

            //spritesBatch.shapes.DrawFilledRectangleGradient(UIComponent.worldMouseRectangle, new Gradient(
                //alice.engine.graphics.Color.Random, alice.engine.graphics.Color.Random, alice.engine.graphics.Color.Random, alice.engine.graphics.Color.Random));

            //spritesBatch.shapes.DrawRectangleOutline(new Rectangle((windowProfile.renderedResolution.width * 10) /2 * -1,
                //(windowProfile.renderedResolution.height * 10) /2 * -1,
                //windowProfile.renderedResolution.width * 10,
                //windowProfile.renderedResolution.height * 10),
                //new engine.graphics.Color(30, 200, 200, 20), 10);

            EndDrawBatchs(currentScene, cam);

            base.Draw(gameTime);
        }

        private void CallDraws(Sprites spritesBatch, Scene scene, out Scene currentScene)
        {
            currentScene = scene;
            for (int i = 0; i < scene.gameObjects.Count; i++)
            {                scene.gameObjects[i].Draw(spritesBatch);
            }
        }

        private void CallUIDraws(Sprites spritesBatch, Scene scene)
        {
            for (int i = 0; i < scene.gameObjects.Count; i++)
            {
                scene.gameObjects[i].DrawUI(spritesBatch);
            }
        }


        public void StartDrawBatchs(out Camera cam)
        {
            //Console.Clear();
            cam = getMainCamera;

            screen.Set();

            //spritesBatch.shapes.basicEffect = spritesBatch.basicEffect;
            loadedScene?.scenery.ApplyScenery(spritesBatch, cam);
            spritesBatch.Begin(cam, false);
            spritesBatch.shapes.Begin(cam);
        }

        private void EndDrawBatchs(Scene scene, Camera cam)
        {
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine("END BATCH");
            //Console.ResetColor();

            engine.debugging.Debugging.Draw(spritesBatch);
            spritesBatch.End();

            //draws UIs
            if(scene != null)
            {
                spritesBatch.BeginUI();
                CallUIDraws(spritesBatch, scene);
                spritesBatch.End(); //ends both ui and non-ui batchs.
            }

            spritesBatch.shapes.isDrawingInCanvas = true;
            spritesBatch.shapes.SetMatrixToDefault();

            screen.UnSet(spritesBatch, cam);
            spritesBatch.shapes.End();
            spritesBatch.shapes.isDrawingInCanvas = false;
            screen.Present(spritesBatch);
        }

        public void LoadScene(Scene scene)
        {
            DateTime dateTime = DateTime.Now;
            loadingScene = true;
            CreateCollisionComponent();

            if (loadedScene != null)
                loadedScene.Destroy();

            loadedScene = null;
            //Content.Unload();

            //if (_spriteBatch != null)
            //    _spriteBatch.Dispose();


            if (scene == null) { throw new Exception("no scene to load content from."); }
            Console.WriteLine("Core.LoadScene(..):" + scene.sceneName + " with : " + scene.gameObjects.Count + "bad gobj(s)");

            loadedScene = scene;
            scene.OnCreationCall(dateTime); // <--- OnCreationCall() will set loadingScene to true when done.
        }

        public void LoadLoadingScene(Scene loadingScene)
        {
            LoadContent();
            if (SceneLoader.loadingScene != null)
                SceneLoader.loadingScene.Destroy();

            SceneLoader.loadingScene = loadingScene;
            SceneLoader.loadingScene.OnCreation();

        }

        internal void CreateCollisionComponent()
        {
            _collisionComponent = new CollisionComponent(
                new RectangleF((windowProfile.renderedResolution.width * 10) /2 * -1, (windowProfile.renderedResolution.height * 10) /2 * -1,
                windowProfile.renderedResolution.width * 10, windowProfile.renderedResolution.height * 10));
        }

        private int deltaScrollWheelValue = 0;
        private int currentScrollWheelValue = 0;
        private bool debuggingEnabled = true;
        private void Debugging()
        {
            Camera cam = getMainCamera;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            deltaScrollWheelValue = mouseState.ScrollWheelValue - currentScrollWheelValue;
            currentScrollWheelValue += deltaScrollWheelValue;

            if (keyboardState.IsKeyDown(Keys.LeftAlt))
            {
                //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                //    Exit();
                //
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    cam.Move(alice.engine.maths.Vector2.Up * 5);
                    Inputs.KeyboardState.SetBusyForXCycle(2);
                }
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    cam.Move(alice.engine.maths.Vector2.Down * 5);
                    Inputs.KeyboardState.SetBusyForXCycle(2);
                }
                if (keyboardState.IsKeyDown(Keys.D))
                {
                    cam.Move(alice.engine.maths.Vector2.Left * 5);
                    Inputs.KeyboardState.SetBusyForXCycle(2);
                }
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    cam.Move(alice.engine.maths.Vector2.Right * 5);
                    Inputs.KeyboardState.SetBusyForXCycle(2);
                }

                //if (keyboardState.IsKeyDown(Keys.Up))
                //{
                    //cam.rotation = new Vector3(cam.rotation.X, cam.rotation.Y += 50, cam.rotation.Z);
                    //Console.WriteLine(cam.rotation);
                //}

                //if (keyboardState.IsKeyDown(Keys.Down))
                //{
                    //cam.rotation = new Vector3(cam.rotation.X, cam.rotation.Y -= 50, cam.rotation.Z);
                    //Console.WriteLine(cam.rotation);
                //}

                //if (keyboardState.IsKeyDown(Keys.Left))
                //{
                    //cam.rotation = new Vector3(cam.rotation.X -= 50, cam.rotation.Y, cam.rotation.Z);
                    //Console.WriteLine(cam.rotation);
                //}
                //if (keyboardState.IsKeyDown(Keys.Right))
                //{
                    //cam.rotation = new Vector3(cam.rotation.X += 50, cam.rotation.Y, cam.rotation.Z);
                    //Console.WriteLine(cam.rotation);
                //}

                //if (keyboardState.IsKeyDown(Keys.X))
                //{
                    //cam.rotation = new Vector3(cam.rotation.X, cam.rotation.Y, cam.rotation.Z += 50);
                    //Console.WriteLine(cam.rotation);
                //}
                //if (keyboardState.IsKeyDown(Keys.C))
                //{
                    //cam.rotation = new Vector3(cam.rotation.X, cam.rotation.Y, cam.rotation.Z -= 50);
                    //Console.WriteLine(cam.rotation);
                //}


                if (keyboardState.IsKeyDown(Keys.V))
                {
                    cam.z -= 20;
                }


                if (keyboardState.IsKeyDown(Keys.B))
                {
                    cam.z += 20;
                }



                //
                if (deltaScrollWheelValue > 0) cam.ZoomIn();
                else if (deltaScrollWheelValue < 0) cam.ZoomOut();

                //if (keyboardState.IsKeyDown(Keys.Delete))
                //{
                //    _camera.Zoom = 1;
                //    _camera.Position = Vector2.Zero;
                //    Inputs.KeyboardState.SetBusyForXCycle(2);
                //    loadedScene.scenery.clearingColor = alice.engine.Color.Random;
                //}
                //else if (keyboardState.IsKeyDown(Keys.Up))
                //{
                //    Inputs.KeyboardState.SetBusyForXCycle(2);
                //}
            }
        }
    }

}