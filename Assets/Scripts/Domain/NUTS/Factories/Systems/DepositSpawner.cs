using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Systems;

namespace YAPCG.Domain.NUTS.Factories.Systems
{
    [UpdateInGroup(typeof(TickWeeklyGroup))]
    public partial struct DepositSpawner : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SharedRandom>();
            state.EntityManager.CreateSingletonBuffer<Deposit.DepositSpawnConfig>();

            SystemAPI.GetSingletonBuffer<Deposit.DepositSpawnConfig>(false).Add(new Deposit.DepositSpawnConfig
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
            var spawnConfigs = SystemAPI.GetSingletonBuffer<Deposit.DepositSpawnConfig>(false);
            
            foreach (var config in spawnConfigs)
            {
                for (int i = 0; i < config.Small; i++)
                    DepositFactory.CreateBigDeposit(ecb, config.Position, NamingGenerator.Get(ref random));

                for (int i = 0; i < config.Medium; i++)
                    DepositFactory.CreateMediumDeposit(ecb, config.Position, NamingGenerator.Get(ref random));

                for (int i = 0; i < config.Big; i++)    
                    DepositFactory.CreateSmallDeposit(ecb, config.Position + float3.zero, NamingGenerator.Get(ref random));
            }
            spawnConfigs.Clear();
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            
            SystemAPI.SetSingleton(new SharedRandom { Random = random });
        }
    }


}