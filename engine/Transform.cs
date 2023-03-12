namespace alice.engine

{
    public class Transform
    {

        private Vector2 _position = new Vector2(0,0); //object position
        public Vector2 position
        {
            get 
            {
                if (gameObject.parent == null)
                    return _position;
                else
                    return gameObject.parent.transform.position + _position;
            }
            set { _position = value; }
        }

        public Vector2 scale = new Vector2(1,1);

        public float rotation = 0;

        public GameObject gameObject;

        public void MoveToward(Transform transform, float speed)
        {
            MoveToward(transform.position, speed);
        }

        public void MoveToward(engine.Vector2 position, float speed)
        {
            engine.Vector2 direction = position - this.position;
            direction.Normalize();
            if (!direction.isNaN())
            {
                this.position += direction * speed;
                //this.rotation = (float)Math.Atan2(direction.Y, direction.X);
            }

        }

    }
}