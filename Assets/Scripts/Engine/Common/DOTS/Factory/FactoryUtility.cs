using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Engine.Common.DOTS.Factory
{
    public static class FactoryUtility
    {
        public static void CallFactory<T>(this EntityCommandBuffer ecb, EntityManager _, Entity factoryEntity, ref Random random, IFactory<T> factory, out NativeList<Entity> spawned) 
            where T : unmanaged, IFactoryParams, IBufferElementData
        {
            DynamicBuffer<T> factoryParams = _.GetBuffer<T>(factoryEntity);
            spawned = new NativeList<Entity>(Allocator.Temp);
            foreach(T config in factoryParams)
                factory.Spawn(ecb, config, ref random, ref spawned);
            
            factoryParams.Clear();
        }

    }
}