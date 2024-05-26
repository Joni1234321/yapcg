using System;
using Unity.Collections;
using Unity.Entities;
using YAPCG.Domain.NUTS.SpawnConfigs;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Domain.NUTS.Factories
{
    public interface IFactory<T> 
        where T : unmanaged, ISpawnConfig, IBufferElementData
    {
        public void Spawn(EntityCommandBuffer ecb, T config, ref Random random, ref NativeList<Entity> spawned);
    }
}