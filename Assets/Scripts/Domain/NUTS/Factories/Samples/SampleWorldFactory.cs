using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.Common.Components;
using YAPCG.Engine.Common.DOTS.Factory;
using YAPCG.Engine.Components;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Domain.NUTS.Factories.Samples
{
    internal struct SampleWorldFactory : IWorldFactory
    {
        public void Setup(ref SystemState state)
        {
            state.RequireForUpdate<FactoryUtility.FactoryReadyTag>();
        }

        public void Spawn(ref EntityCommandBuffer ecb, ref SystemState state, ref Random random)
        {
            ecb.CallFactory(state.EntityManager, ref random, new HubFactory(), out var hubs);
            ecb.CallFactory(state.EntityManager, ref random, new DepositFactory(), out _);
            ecb.CallFactory(state.EntityManager, ref random, new SolarSystemFactory(), out _);

            foreach(var hub in hubs) ecb.SetComponent(hub, new DiscoverProgress { Value = random.NextInt(0, 40), Progress = 1, MaxValue = 40});
        }
    }
    
}