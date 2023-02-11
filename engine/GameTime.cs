using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alice.engine
{
    public class GameTime
    {

        public TimeSpan ElapsedGameTime;
        public float ElapsedTotalSeconds;

        public GameTime(Microsoft.Xna.Framework.GameTime gameTime) 
        {
            ElapsedGameTime = gameTime.ElapsedGameTime;
            ElapsedTotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

    }
}
