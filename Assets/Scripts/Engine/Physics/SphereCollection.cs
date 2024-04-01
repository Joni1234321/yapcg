using Unity.Collections;
using Unity.Mathematics;

namespace YAPCG.Engine.Physics
{
    public struct SphereCollection
    {
        public float Radius;
        public NativeArray<float3> Positions;
    }
    
    public struct TriangleCollection
    {
        public NativeArray<float3> Positions;
    }
}