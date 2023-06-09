using UnityEngine;

namespace Extensions
{
    public static class TransformExtension
    {
        public static Transform GetLastChild(this Transform transform)
        {
            var current = transform;

            while (current.childCount > 0)
            {
                current = current.GetChild(0);
            }

            return current;
        }
    }
}