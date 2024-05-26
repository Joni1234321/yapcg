using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.Common.Components;
using YAPCG.Engine.Common.DOTS;
using YAPCG.Engine.Common.DOTS.Factory;
using YAPCG.Engine.Components;

namespace YAPCG.Domain.NUTS.Factories.Systems
{
    internal struct SampleWorldFactory : IWorldFactory
    {
        private Entity _factoryEntity;

        public void Setup(ref SystemState state)
        {
            _factoryEntity = state.EntityManager.CreateEntity();
            state.EntityManager.SetName(_factoryEntity, "Factory Entity");

            state.EntityManager.AddComponentData(state.EntityManager.CreateEntity(), new LevelQuad {Size = new float2(100, 100)});

            state.EntityManager.AddBufferAndDispose(_factoryEntity, GetHubs());
            state.EntityManager.AddBufferAndDispose(_factoryEntity, GetDeposits());
            state.EntityManager.AddBuffer<SolarySystemFactoryParams>(_factoryEntity);
        }

        public void Spawn(ref EntityCommandBuffer ecb, ref SystemState state, ref Random random)
        {
            ecb.CallFactory(state.EntityManager, _factoryEntity, ref random, new HubFactory(), out var hubs);
            ecb.CallFactory(state.EntityManager, _factoryEntity, ref random, new DepositFactory(), out _);
            ecb.CallFactory(state.EntityManager, _factoryEntity, ref random, new SolarSystemFactory(), out _);

            foreach(var hub in hubs) ecb.SetComponent(hub, new  DiscoverProgress { Value = random.NextInt(0, 40), Progress = 1, MaxValue = 40});
        }

        private NativeList<Deposit.DepositFactoryParams> GetDeposits() => new(Allocator.Persistent) { new() { Big = 2, Medium = 1, Small = 1, } };
        
        private NativeList<Hub.HubFactoryParams> GetHubs()
        {
            NativeList<Hub.HubFactoryParams> hubs = new(Allocator.Persistent);
            const int W = 2, H = 2;
            for (int y = -H; y <= H; y++)
                for (int x = -W; x <= W; x++)
                    hubs.Add(new Hub.HubFactoryParams { Position = new float3(x, 0, y) * 10, Size = Hub.Size.Big });
            return hubs;
        }
    }
}