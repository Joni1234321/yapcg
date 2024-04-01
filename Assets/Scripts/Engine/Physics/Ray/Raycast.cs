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

        public struct Hit
        {
            /// <summary>
            /// index of hit object
            /// </summary>
            public int hit;
            
            /// <summary>
            /// t from ray 
            /// </summary>
            public float t;

            /// <summary>
            /// Returns true when nothing is hit
            /// </summary>
            public bool Nothing() => hit == -1;
        }
        
        /// <summary>
        /// Returns the sphere that collides with first
        /// </summary>
        /// <param name="origin">ray origin</param>
        /// <param name="direction">ray direction normalized</param>
        /// <param name="spheres">the collection of spheres</param>
        public static Hit CollisionSphere(float3 origin, float3 direction, SphereCollection spheres)
        {
            Hit hit = new Hit { hit = -1, t = float.MaxValue};

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
                float bhalf = math.dot(direction, oc);
                float c = math.dot(oc, oc) - radius;

                // b * b - 4 * a * c = bhalf * bhalf * 4 - 4 * c
                // quarter = bhalf * bhalf - c
                float discriminantQuarter = bhalf * bhalf - c; 

                // used when iterating through all in collection
                if (Hint.Likely(discriminantQuarter < 0)) // no intersection
                    continue;
                
                // t = (sqrt(d) - b) / 2a = sqrt(d) * 0.5 - bhalf = sqrt(dq) * sqrt(4) * 0.5 - bhalf = sqrt(dq) - bhalf
                float dqsqrt = math.sqrt(discriminantQuarter);
                
                // solution 1
                float intersection = dqsqrt - bhalf;
                if (intersection > 0)
                {
                    if (intersection > hit.t)
                        continue;
                }
                else
                {
                    // solution 2
                    intersection = dqsqrt + bhalf; 
                    if (intersection < 0 || intersection > hit.t) 
                        continue;
                }

                hit = new Hit { hit = i, t = intersection };
            }
            
            return hit;
        }

        public static void CollidingAxisAlignedPlane()
        {
            // Solving 1 degree equation see pdf
            
        }
        
        
        // https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
        static void MollerTromboneIntersection()
        {
            
        }
    }
}