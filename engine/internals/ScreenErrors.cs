using alice.engine.graphics;

using Microsoft.Xna.Framework;

using Color = Microsoft.Xna.Framework.Color;

namespace alice.engine.internals
{
    internal static class ScreenErrors
    {

        public static void LoadingSceneNull(Sprites spritesBatch)
        {
            spritesBatch.DrawString(FontManager.defaultFont.font, "loading scene...", new Vector2(0, 0), Color.Red);
            Launcher.core.GraphicsDevice.Clear(Color.Black);
        }

        public static void NoMainCamera(Sprites spritesBatch)
        {
            spritesBatch.DrawString(FontManager.defaultFont.font, "Missing a camera, set a camera in SceneLoader.currentScene.sceneCamera", new Vector2(0, 0), Color.Red);
            Launcher.core.GraphicsDevice.Clear(Color.Black);
        }

    }
}
