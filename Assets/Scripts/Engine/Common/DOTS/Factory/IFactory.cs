using Unity.Collections;
using Unity.Entities;

namespace YAPCG.Engine.Common.DOTS.Factory
{
    public interface IFactory<T> where T : unmanaged, IBufferElementData, IFactoryParams
    {
        public void Spawn(EntityCommandBuffer ecb, T config, ref Unity.Mathematics.Random random, ref NativeList<Entity> spawned);
    }
}