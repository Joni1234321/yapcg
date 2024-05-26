using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.NUTS.SpawnConfigs;

namespace YAPCG.Domain.NUTS
{
    public struct Hub
    {
        public struct HubTag : IComponentData { }
        
        [InternalBufferCapacity(0)]
        public struct HubSpawnConfig : IBufferElementData, ISpawnConfig
        {
            public float3 Position;
            public Size Size;
        }

        public enum Size
        {
            Small, Medium, Big
        }
    }
}