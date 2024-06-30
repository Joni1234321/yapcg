using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Engine.Common.DOTS.Factory
{
    public interface IFactory<T> where T : unmanaged, IBufferElementData, IFactoryParams
    {
        public void Spawn(ref Random random, ref EntityCommandBuffer ecb, ref NativeList<Entity> spawned, in T config);
    }
}