using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Engine.Common.DOTS.Factory;

namespace YAPCG.Domain.NUTS
{
    public struct Hub
    {
        public struct HubTag : IComponentData { }
        
        [InternalBufferCapacity(0)]
        public struct HubFactoryParams : IBufferElementData, IFactoryParams
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