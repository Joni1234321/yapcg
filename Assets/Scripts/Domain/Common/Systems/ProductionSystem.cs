using Unity.Burst;
using Unity.Entities;
using YAPCG.Domain.Common.Components;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Time.Systems;

namespace YAPCG.Domain.Common.Systems
{
    [UpdateInGroup(typeof(TickWeeklyGroup))]
    internal partial struct ProductionSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new ReserveClearJob().Run();
            new ProductionJob().Run();
            
        }

        [BurstCompile]
        partial struct ReserveClearJob : IJobEntity
        {
            void Execute(ref Deposit.Reserves reserves) => reserves.Value = 0;
        }
        
        [BurstCompile]
        partial struct ProductionJob : IJobEntity
        {
            void Execute(ref Deposit.Reserves reserves, in Deposit.Sizes sizes, in Labor labor)
            {
                reserves.Value += sizes.Open * 1;
                reserves.Value += sizes.S * 2;
                reserves.Value += sizes.M * 4;
                reserves.Value += sizes.L * 8;

                reserves.Value *= labor.Population;
            }
        }
    }
    

}