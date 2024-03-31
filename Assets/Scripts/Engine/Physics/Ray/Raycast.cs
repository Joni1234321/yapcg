using Unity.Mathematics;
using Unity.Burst.CompilerServices;

namespace YAPCG.Engine.Physics.Ray
{
    public static class Raycast
    {
        // https://gfxcourses.stanford.edu/cs348b/spring22content/media/intersection/rt1_ZWjhy3k.pdf
        // Stanford ray tracing basics pdf
        
        // ray given as 
        // r = o + td 
        
        // plane as where p' is any point
        // dot((p' - p), n) = 0  
        
        // sphere as 
        // (p - c)(p - c) - r^2 = 0
        
        /// <summary>
        /// Returns the sphere that collides with first
        /// </summary>
        /// <param name="origin">ray origin</param>
        /// <param name="direction">ray direction normalized</param>
        /// <param name="spheres">the collection of spheres</param>
        public static int CollisionSphere(float3 origin, float3 direction, SphereCollection spheres)
        {
            int hit = -1;
            float t = -1;
            
            // solving 2degree equation 
            // a = dot(d, d) = 1 (since d is normalized)
            // b = dot(2 (o - c), d)
            // c = dot 

            for (int i = 0; i < spheres.Positions.Length; i++)
            {
                float3 center = spheres.Positions[i];
                float  radius = spheres.Radius;
                
                float3 oc = origin - center;
                //   a = 1
                float b = 2 * math.dot(direction, oc);
                float c = math.dot(oc, oc) - radius;

                float discriminant = b * b - 4 * c; // b * b - 4 * a * c

                if (Hint.Likely(discriminant < 0)) // no intersection
                    continue;
                
                // t = (sqrt(d) - b) / 2a
                float intersection = (math.sqrt(discriminant) - b) / 2; 
                if (intersection > 0 && intersection < t)
                {
                    t = intersection;
                    hit = i;
                    continue;
                }
                
                // t = (sqrt(d) + b) / 2a = (sqrt(d) + b) / 2 = (sqrt(d) - b) / 2 + b
                intersection += b; 
                if (intersection > 0 && intersection < t)
                {
                    t = intersection;
                    hit = i;
                    continue;
                }
            }


            return hit;
        }

        public static void CollidingAxisAlignedPlane()
        {
            // Solving 1 degree equation see pdf
            
        }
    }
}