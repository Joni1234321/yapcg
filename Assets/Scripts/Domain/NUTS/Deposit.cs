using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Domain.NUTS
{
    public struct Deposit
    {
        [InternalBufferCapacity(0)]
        public struct DepositSpawnConfig : IBufferElementData, ISpawnConfig
        {
            public int Big, Medium, Small;
            
            public int Total => Big + Medium + Small;
        }
        
        public struct Tag : IComponentData
        {
        }

        public struct Sizes : IComponentData
        {
            public byte Open, S, M, L;
        }
        
        public enum RGO
        {
            Coal, 
            Iron,
            Aluminum,
            Gas, 
            Oil,
            Rare,
            COUNT
        }

        public struct RGOType : IComponentData
        {
            public RGO RGO;
        }
    /*

        [InternalBufferCapacity(8)]
        public struct Sizes : IBufferElementData
        {
            public byte Size;
        }

        public unsafe struct Levels : IComponentData
        {
            public fixed byte Values[8];
        }

        public struct L2 : IComponentData
        {
            public NativeArray<byte> Values;
        }
        [StructLayout(LayoutKind.Explicit)]
        public struct L3 : IComponentData
        {
            [FieldOffset(0)]
            public NativeArray<byte> Bytes;
            [FieldOffset(0)]
            public long Long;
        }-*
        
        */
        public struct Reserves : IComponentData
        {
            public long Value;
        }
    }

    public struct Mine : IComponentData
    {
        
    }
}