using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.Common.Components;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Systems;

namespace YAPCG.Domain.NUTS.Factories.Systems
{
    [UpdateInGroup(typeof(TickDailyGroup))]
    public partial struct NutsSpawnerSystem : ISystem
    {
        private Entity _entity;
        public void OnCreate(ref SystemState state)
        {
            _entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddBuffer<Hub.HubSpawnConfig>(_entity);
            state.EntityManager.AddBuffer<Deposit.DepositSpawnConfig>(_entity);

            SpawnWorld(ref state);
            
            state.RequireForUpdate<SharedRandom>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Random random = SystemAPI.GetSingleton<SharedRandom>().Random;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            var hubs = new NativeList<Entity>(Allocator.Temp);
            var deposits = new NativeList<Entity>(Allocator.Temp); 

            SpawnConfigs(ecb, state.EntityManager, new HubFactory(), ref random, ref hubs);
            SpawnConfigs(ecb, state.EntityManager, new DepositFactory(), ref random, ref deposits);
            
            foreach(var hub in hubs) ecb.SetComponent(hub, new  DiscoverProgress { Value = random.NextInt(0, 40), Progress = 1, MaxValue = 40});

            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            SystemAPI.SetSingleton(new SharedRandom { Random = random });
        }

        private void SpawnConfigs<T>(EntityCommandBuffer ecb, EntityManager _, IFactory<T> factory, ref Random random, ref NativeList<Entity> spawned) 
            where T : unmanaged, ISpawnConfig, IBufferElementData
        {
            var configs = _.GetBuffer<T>(_entity);

            factory.Spawn(ecb, configs, ref random, ref spawned);
            configs.Clear();
        }


        void SpawnWorld(ref SystemState state)
        {
            // Level
            state.EntityManager.AddComponentData(state.EntityManager.CreateEntity(), new LevelQuad {Size = new float2(100, 100)});

            // Hubs
            SystemAPI.GetSingletonBuffer<Hub.HubSpawnConfig>(false).Add(new Hub.HubSpawnConfig
            {
                Position = float3.zero,
                Big = 1,
                Medium = 100,
                Small = 1000
            });

            // Deposits
            SystemAPI.GetSingletonBuffer<Deposit.DepositSpawnConfig>(false).Add(new Deposit.DepositSpawnConfig
            {
                Big = 2,
                Medium = 1,
                Small = 1,
            });

        }
        
    }
}