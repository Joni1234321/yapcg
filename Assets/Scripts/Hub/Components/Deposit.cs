using Unity.Entities;

namespace YAPCG.Hub.Components
{
    
    
    public struct Deposit
    {
        public struct Tag : IComponentData
        {
        }

        [InternalBufferCapacity(8)]
        public struct Sizes : IBufferElementData
        {
            public byte Size;
        }

        public struct Reserves : IComponentData
        {
            public long Value;
        }
    }

    public struct Mine : IComponentData
    {
        
    }
}