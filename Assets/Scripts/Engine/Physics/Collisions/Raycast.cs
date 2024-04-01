using Unity.Burst.CompilerServices;
using Unity.Mathematics;

namespace YAPCG.Engine.Physics.Collisions
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

        public struct hit
        {
            /// <summary>
            /// index of hit object
            /// </summary>
            public int index;
            
            /// <summary>
            /// intersection point
            /// </summary>
            public float3 point;
        }


        /// <summary>
        /// normalized direction
        /// </summary>
        public struct ray
        {
            public float3 origin { get; set; }

            public float3 direction
            {
                get => _direction;
                set => _direction = math.normalize(value);
            }
            private float3 _direction;
        }

        /// <summary>
        /// Returns the sphere that collides with first
        /// </summary>
        /// <param name="ray">ray</param>
        /// <param name="spheres">the collection of spheres</param>
        /// <param name="hit"></param>
        public static bool CollisionSphere(ray ray, SphereCollection spheres, out hit hit)
        {
            float t = float.MaxValue;
            hit = new hit { index = -1, point = ray.origin };
            
            for (int i = 0; i < spheres.Positions.Length; i++)
                if (RayCollidesWithCloserSphere(ray, spheres.Positions[i], spheres.Radius, ref t))
                    hit.index = i;

            hit.point = ray.origin + ray.direction * t;
            return hit.index != -1;
        }

        static bool RayCollidesWithCloserSphere(ray ray, float3 center, float radius, ref float t)
        {
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
                
            // solution 1
            float intersection = dqsqrt - bhalf;
            if (intersection > 0)
            {
                if (intersection > t)
                    return false;
            }
            else
            {
                // solution 2
                intersection = dqsqrt + bhalf; 
                if (intersection < 0 || intersection > t) 
                    return false;
            }

            t = intersection;
            return true;
        }

        public static void CollidingAxisAlignedPlane()
        {
            // Solving 1 degree equation see pdf
            
        }
        
        
        // https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
        static bool MollerTromboneIntersection(ray ray, float3 v0, float3 v1, float3 v2, ref float t)
        {
            // three equations
            // u, v, t for the boyocentric coordinates
            // 
            
            // this uses the carmers rule to solve the linear algebra
            // |Ai| / |A| where Ai is the ith row replaced by the constant of the equations
            
            const float EPSILON = float.Epsilon;
            float3 e1 = v1 - v0;
            float3 e2 = v2 - v0;
            float3 o  = ray.origin;
            float3 d  = ray.direction;

            // determinant can also be found with
            // |A B C| = -(A x C) . B = -(C x B).A
            // with this reuse (p and s) saves time
            float3 p = math.cross(d, e2);
            float det = math.dot(p, e1);
            
            // test if parallel
            if (det is > -EPSILON and < EPSILON)
                return false;

            float invDet = 1.0f / det;
            float3 s = o - v0;
            
            // calculate u and test bounds
            float u = invDet * math.dot(s, p);
            if (u is < 0 or > 1)
                return false;
            
            // calculate v and test bounds
            float3 q = math.cross(s, e1);
            float v = invDet * math.dot(q, q);
            if (v < 0 || u + v > 1)
                return false;
            
            // calculate intersection
            float intersection = invDet * math.dot(q, e2);
            return true;

        }
    }
}