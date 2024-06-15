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

        public static void CollidingAxisAlignedPlane()
        {
            // Solving 1 degree equation see pdf
            
        }
        
    }
}