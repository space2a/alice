using alice.engine.maths;

using Microsoft.Xna.Framework;

using MonoGame.Extended.Collisions;

namespace alice.engine.components
{
    [UniqueComponent]
    public class Rigidbody2D : Component
    {
        public BodyType bodyType = BodyType.Kinematic;

        public enum BodyType
        {
            Dynamic,
            Kinematic,
            Static
        }

        public float gravity = 9.807f; 

        public Rigidbody2D() { }
        public Rigidbody2D(BodyType bodyType) { this.bodyType = bodyType; }

        public void OnAnyCollision(CollidingInput collidingInput)
        {
            if (!collidingInput.inputCollider.isTrigger)
            {
                if (bodyType == BodyType.Dynamic || bodyType == BodyType.Kinematic)
                    transform.position -= new maths.Vector2(collidingInput.force);
            } //if the other colliding object isTrigger is true this object does not react at all.
        }

        internal override void Update(GameTime gameTime)
        {
            //transform.position -= new maths.Vector2(0, (gravity * (float)(gameTime.ElapsedGameTime.TotalSeconds *
            //    (SceneLoader.currentScene.sceneCamera.boundingRectangle.Height / 10))));
        }

    }
}