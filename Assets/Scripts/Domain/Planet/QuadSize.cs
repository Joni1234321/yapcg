using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Planet
{
    public struct LevelQuad : IComponentData
    {
        public float2 Position;
        public float2 Size;
    }
}
