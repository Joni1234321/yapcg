using Unity.Collections;
using Unity.Entities;

namespace YAPCG.Engine.Common.DOTS
{
    public static class EntityExtensions
    {
        public static void AddBuffer<T>(this EntityManager _, Entity entity, NativeArray<T> values) where T : unmanaged, IBufferElementData => _.AddBuffer<T>(entity).AddRange(values);

        public static void AddBufferAndDispose<T>(this EntityManager _, Entity entity, NativeArray<T> values)
            where T : unmanaged, IBufferElementData
        {
            _.AddBuffer<T>(entity).AddRange(values);
            values.Dispose();
        } 
        public static void AddBufferAndDispose<T>(this EntityManager _, Entity entity, NativeList<T> values)
            where T : unmanaged, IBufferElementData
        {
            _.AddBuffer<T>(entity).AddRange(values.AsArray());
            values.Dispose();
        } 
    }
}