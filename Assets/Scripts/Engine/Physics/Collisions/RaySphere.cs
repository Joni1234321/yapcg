using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using static YAPCG.Engine.Physics.Collisions.Raycast;

namespace YAPCG.Engine.Physics.Collisions
{
    public struct RaySphere
    {
        /// <summary>
        /// Returns the sphere that collides with first
        /// </summary>
        /// <param name="ray">ray</param>
        /// <param name="spheres">the collection of spheres</param>
        /// <param name="hit"></param>
        public static bool CheckCollision(ray ray, SphereCollection spheres, out hit hit)
        {
            float closest = float.MaxValue;
            hit = new hit { index = -1, point = ray.origin };
            
            int n = spheres.Positions.Length;
            for (int i = 0; i < n; i++)
            {
                if (!RayCollideWithSphere(ray, spheres.Positions[i], spheres.Radius, out float t))
                    continue;
                if (t > closest) // t is farther away than closest
                    continue;

                closest = t;
                hit.index = i;
            }
            
            hit.point = ray.origin + ray.direction * closest;
            return hit.index != -1;
        }
        
        
        private static bool RayCollideWithSphere(ray ray, float3 center, float radius, out float t)
        {
            t = 0;

            // solving 2degree equation 
            // a = dot(d, d) = 1 (since d is normalized)
            // b = dot(2 (o - c), d)
            // c = dot 
            float3 oc = ray.origin - center;
            //   a = 1
            float bhalf = math.dot(ray.direction, oc);
            float c = math.dot(oc, oc) - radius;

            // b * b - 4 * a * c = bhalf * bhalf * 4 - 4 * c
            // quarter = bhalf * bhalf - c
            float discriminantQuarter = bhalf * bhalf - c; 

            // used when iterating through all in collection
            if (Hint.Likely(discriminantQuarter < 0)) // no intersection
                return false;
                
            // t = (sqrt(d) - b) / 2a = sqrt(d) * 0.5 - bhalf = sqrt(dq) * sqrt(4) * 0.5 - bhalf = sqrt(dq) - bhalf
            float dqsqrt = math.sqrt(discriminantQuarter);
                
            // solve quadratic
            t = dqsqrt - bhalf;
            
            float t2 = dqsqrt + bhalf;
            if (t2 >= 0)
                t = math.min(t, t2);
            
            return t >= 0;
        }

 
    }
}