using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.Common.Components;
using YAPCG.Engine.Common.DOTS.Factory;
using YAPCG.Engine.Components;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Domain.NUTS.Factories.Systems
{
    public interface IWorldGet<T> where T : unmanaged
    {
        NativeArray<T> InitGet();
    }

    internal struct SampleWorldFactory : IWorldFactory
    {
        public void Setup(ref SystemState state)
        {
            state.EntityManager.AddComponentData(state.EntityManager.CreateEntity(), new LevelQuad {Size = new float2(100, 100)});

            state.EntityManager.InitFactoryAndDispose(GetHubs());
            state.EntityManager.InitFactoryAndDispose(GetDeposits());
            state.EntityManager.InitFactoryAndDispose(GetSolarSystem());
        }

        public void Spawn(ref EntityCommandBuffer ecb, ref SystemState state, ref Random random)
        {
            ecb.CallFactory(state.EntityManager, ref random, new HubFactory(), out var hubs);
            ecb.CallFactory(state.EntityManager, ref random, new DepositFactory(), out _);
            ecb.CallFactory(state.EntityManager, ref random, new SolarSystemFactory(), out _);

            foreach(var hub in hubs) ecb.SetComponent(hub, new  DiscoverProgress { Value = random.NextInt(0, 40), Progress = 1, MaxValue = 40});
        }

        public NativeArray<SolarySystemFactoryParams> GetSolarSystem() =>
            new(new SolarySystemFactoryParams[] {new () { Planets = 7 } }, Allocator.Persistent);

        private NativeArray<Deposit.DepositFactoryParams> GetDeposits() => new(0, Allocator.Persistent);
            //new(new Deposit.DepositFactoryParams[] { new(){ Big = 2, Medium = 1, Small = 1} }, Allocator.Persistent);
        private NativeArray<Hub.HubFactoryParams> GetHubs()
        {
            return new(0, Allocator.Persistent);
            const int W = 2, H = 2;
            NativeList<Hub.HubFactoryParams> hubs = new(Allocator.Temp);
            for (int y = -H; y <= H; y++)
                for (int x = -W; x <= W; x++)
                    hubs.Add(new Hub.HubFactoryParams { Position = new float3(x, 0, y) * 10, Size = Hub.Size.Big });
            return hubs.ToArray(Allocator.Persistent);
        }
    }
}