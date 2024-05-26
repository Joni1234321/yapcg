using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Engine.Common.DOTS.Factory
{
    public interface IWorldFactory : IComponentData
    {
        void Setup(ref SystemState state);
        void Spawn(ref EntityCommandBuffer ecb, ref SystemState state, ref Random random);
    }
}