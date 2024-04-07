using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.NUTS.Factories;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Systems;

namespace YAPCG.Domain.Hub.Systems
{
    [UpdateInGroup(typeof(TickWeeklyGroup))]
    public partial struct DepositSpawner : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SharedRandom>();
            state.EntityManager.CreateSingletonBuffer<DepositSpawnConfig>();

            SystemAPI.GetSingletonBuffer<DepositSpawnConfig>(false).Add(new DepositSpawnConfig
            {
                Position = float3.zero,
                Big = 2,
                Medium = 1,
                Small = 1,
            });
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Random random = SystemAPI.GetSingleton<SharedRandom>().Random;

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var spawnConfigs = SystemAPI.GetSingletonBuffer<DepositSpawnConfig>(false);
            
            foreach (var config in spawnConfigs)
            {
                for (int i = 0; i < config.Small; i++)
                    DepositFactory.CreateBigDeposit(ecb, config.Position, HubNamingGenerator.Get(ref random));

                for (int i = 0; i < config.Medium; i++)
                    DepositFactory.CreateMediumDeposit(ecb, config.Position, HubNamingGenerator.Get(ref random));

                for (int i = 0; i < config.Big; i++)    
                    DepositFactory.CreateSmallDeposit(ecb, config.Position + float3.zero, HubNamingGenerator.Get(ref random));
            }
            spawnConfigs.Clear();
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            
            SystemAPI.SetSingleton(new SharedRandom { Random = random });
        }
    }

    [InternalBufferCapacity(0)]
    public struct DepositSpawnConfig : IBufferElementData
    {
        public float3 Position;
        public int Big, Medium, Small;
    }
}