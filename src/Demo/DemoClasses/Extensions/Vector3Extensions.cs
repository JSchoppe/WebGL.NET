using System.Collections.Generic;
using WaveEngine.Common.Math;

namespace Demo.DemoClasses.Extensions
{
    /// <summary>
    /// Contains extensions for Wave Engine's Vector3 class.
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// Spreads a collection of coordinates into their ordered components.
        /// </summary>
        /// <param name="collection">The collection of vectors to spread.</param>
        /// <returns>An array containing all components.</returns>
        public static float[] Spread(this IList<Vector3> collection)
        {
            float[] expanded = new float[collection.Count * 3];
            // Get the components from each Vector3.
            for (int i = 0; i < collection.Count; i++)
            {
                expanded[i * 3] = collection[i].X;
                expanded[i * 3 + 1] = collection[i].Y;
                expanded[i * 3 + 2] = collection[i].Z;
            }
            return expanded;
        }
    }
}
