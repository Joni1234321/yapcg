using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Domain.NUTS
{
    public struct Hub
    {
        public enum Size
        {
            Small, Medium, Big
        }
        [InternalBufferCapacity(0)]
        public struct HubSpawnConfig : IBufferElementData, ISpawnConfig
        {
            public float3 Position;
            public Size Size;
        }
        
        public struct HubTag : IComponentData
        {
        };
    }
}