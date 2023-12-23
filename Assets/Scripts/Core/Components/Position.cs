using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Core.Components
{
    public struct Position : IComponentData
    {
        public float3 Value;
    }
}