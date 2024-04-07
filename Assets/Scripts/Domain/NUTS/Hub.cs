using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Domain.NUTS
{
    public struct Hub
    {
        [InternalBufferCapacity(0)]
        public struct HubSpawnConfig : IBufferElementData
        {
            public float3 Position;
            public int Big, Medium, Small;

            public int Total => Big + Medium + Small;
        }
        
        public struct HubTag : IComponentData
        {
        };
    }
}