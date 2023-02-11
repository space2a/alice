using System;
using alice.engine.graphics;
using alice.engine.maths;

using Microsoft.Xna.Framework;

using Vector2 = alice.engine.maths.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace alice.engine.components
{
    [UniqueComponent]
    public class Camera : Component
    {
        public readonly static float MinZ = 1f;
        public readonly static float MaxZ = 2048f;

        public Canvas canvas;

        public alice.engine.maths.Vector3 rotation = new alice.engine.maths.Vector3(0f, 1f, 0f);
        public Vector2 position { get { return _position; } set { _position = value; CreateBoundingRectangle(); } }

        private Vector2 _position;
        internal float _z, baseZ;
        public float z { get { return _z; } set { _z = value; CreateBoundingRectangle(); } }

        private float aspectRatio;
        private float fieldOfView;

        internal Matrix view, proj;

        private float _zoom = 1;
        public float zoom { get { return _zoom; } set { _zoom = value; z = baseZ / _zoom; CreateBoundingRectangle(); } }
        public float minZoom = 0.55f;
        public float maxZoom = 20;

        private bool needToRecalculteBR = true;

        public maths.Rectangle boundingRectangle { get; private set; }
        internal Microsoft.Xna.Framework.Rectangle XNABoundingRectangle { get; private set; }

        public Camera()
        {
            _position = new Vector2(0);

            aspectRatio = (float)Launcher.core.windowProfile.renderedResolution.width / Launcher.core.windowProfile.renderedResolution.height;
            fieldOfView = MathHelper.PiOver2;

            baseZ = GetZFromHeight(Launcher.core.windowProfile.renderedResolution.height);
            _z = baseZ;

            UpdateMatrices();
        }
        
        internal void UpdateMatrices()
        {
            view = Matrix.CreateLookAt(new Vector3(_position.X, _position.Y, _z), new Vector3(_position.X, _position.Y, 0) + rotation.ToXna(), Vector3.Up);
            proj = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, MinZ, MaxZ);
        }

        private float GetZFromHeight(float height)
        {
            return 0.5f * height / MathF.Tan(0.5f * fieldOfView);
        }

        public float GetHeightFromZ()
        {
            return z * MathF.Tan(0.5f * fieldOfView) * 2f;
        }

        private void MoveZ(float amount)
        {
            _z += amount;
            //clamp z between minz and maxz
        }

        public void ResetZ() { z = baseZ; }

        public void Move(Vector2 amout)
        {
            position += amout;
        }

        public void MoveTo(Vector2 position)
        {
            this.position = position;
        }

        public void ZoomIn()
        {
            zoom = MathU.Clamp(zoom += 0.025f, minZoom, maxZoom);
            z = baseZ / zoom;
        }

        public void ZoomOut()
        {
            zoom = MathU.Clamp(zoom -= 0.025f, minZoom, maxZoom);
            z = baseZ / zoom;
        }

        public void SetZoom(float zoom)
        {
            this.zoom = zoom;
            z = baseZ / zoom;
        }

        public void GetExtents(out float width, out float height)
        {
            height = GetHeightFromZ();
            width = height * aspectRatio;
        }

        public void GetExtents(out float left, out float right, out float bottom, out float top)
        {
            GetExtents(out float width, out float height);
            left = _position.X - width * 0.5f;
            right = left + width;
            bottom = _position.Y - height * 0.5f;
            top = bottom + height;
        }

        public void GetExtents(out Vector2 min, out Vector2 max)
        {
            GetExtents(out float left, out float right, out float bottom, out float top);
            min = new Vector2(left, bottom);
            max = new Vector2(right, top);
        }

        public void CreateBoundingRectangle()
        {
            if (!needToRecalculteBR) return;
            GetExtents(out float width, out float height);

            boundingRectangle = new maths.Rectangle((int)(position.X - width / 2), (int)(position.Y - height / 2), (int)width, (int)height);
            XNABoundingRectangle = boundingRectangle.ToXnaRectangle();
        }

        public void FollowTarget(Vector2 target, float smoothTime, float ellapsedTime)
        {
            float interpolation = smoothTime * ellapsedTime;

            Vector3 pos = position.ToXNAVector3();
            pos.X = MathHelper.Lerp(position.X, target.X, interpolation);
            pos.Y = MathHelper.Lerp(position.Y, target.Y, interpolation);

            position = new Vector2(pos.X, pos.Y);
        }

        internal void DrawCanvas(Sprites spriteBatch)
        {
            if (canvas == null) return;
            canvas?.CallDraw(spriteBatch);
            canvas?.CallDrawUI(spriteBatch);
        }

    }
}
