using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;

namespace YAPCG.Hub.Components
{
    public struct Deposit
    {
        public struct Tag : IComponentData
        {
        }

        public struct Sizes : IComponentData
        {
            public byte Open, S, M, L;
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