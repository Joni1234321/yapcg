using System;
using Unity.Collections;
using Unity.Entities;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Domain.NUTS.Factories
{
    public interface IFactory<T> 
        where T : unmanaged, ISpawnConfig, IBufferElementData
    {
        public void Spawn(EntityCommandBuffer ecb, DynamicBuffer<T> configs, ref Random random, ref NativeList<Entity> spawned);
    }
}