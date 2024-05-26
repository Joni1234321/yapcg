using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Engine.Common.DOTS.Factory
{
    public class FactoryUtility
    {
        public static void CallFactory<T>(EntityCommandBuffer ecb, EntityManager _, Entity factoryEntity, IFactory<T> factory, ref Random random, out NativeList<Entity> spawned) 
            where T : unmanaged, IFactoryParams, IBufferElementData
        {
            var configs = _.GetBuffer<T>(factoryEntity);
            spawned = new NativeList<Entity>(Allocator.Temp);
            foreach(T config in configs)
                factory.Spawn(ecb, config, ref random, ref spawned);
            
            configs.Clear();
        }

    }
}