using UnityEngine;

namespace SecondOrderLinear
{
    public class VectorLinear
    {
        private Vector3 xp;
        private Vector3 y, yd;

        private float tCritical;
        private float k1, k2, k3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f">Speed of input changes</param>
        /// <param name="z">Damping</param>
        /// <param name="r">Accelerating</param>
        /// <param name="x0">Initial position</param>
        public VectorLinear(float f, float z, float r, Vector3 x0)
        {
            var PI = Mathf.PI;
            k1 = z / (PI * f);
            k2 = 1 / ((2 * PI * f) * (2 * PI * f));
            k3 = r * z / 2 * PI * f;
            
            tCritical = 0.8f * (Mathf.Sqrt(4 * k2 + k1 * k1) - k1);
            
            xp = x0;
            y = x0;
            yd = Vector3.zero;
        }
        
        /// <summary>
        /// Calculate new vector 
        /// </summary>
        /// <param name="t">Time</param>
        /// <param name="x">Target Vector</param>
        /// <returns></returns>
        public Vector3 Update(float t, Vector3 x)
        {
            var xd = (x - xp) / t;
            xp = x;
            
            var iterations = Mathf.CeilToInt(Time.deltaTime / tCritical);
            for (var i = 0; i < iterations; i++)
            {
                y += yd * t;
                yd += t * (x + k3 * xd - y - k1 * yd) / k2;
            }

            return y;
        }
    }
}