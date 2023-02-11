using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alice.engine.internals
{
    public static class Utils
    {

        public static T[] ResizeStaticArray<T>(T[] array, int size)
        {
            Array.Resize(ref array, size);
            return array;
        }

        public static bool AreEventsSubscribed(EventHandler[] eventHandlers)
        {
            bool result = false;

            for (int i = 0; i < eventHandlers.Length; i++)
            {
                if (eventHandlers[i] != null) result = true;
            }

            return result;
        }

    }
}
