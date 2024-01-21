﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using YAPCG.Engine.Time.Systems;
using YAPCG.Hub.Components;

namespace YAPCG.Hub.Systems
{
    [UpdateInGroup(typeof(TickWeeklyGroup))]
    public partial struct DepositIncomeSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {  
            var incomeJob = new IncomeJob() {  };
            incomeJob.Run();
        }

        
        [BurstCompile]
        partial struct IncomeJob : IJobEntity
        {
            void Execute( in Entity entity, ref Deposit.Reserves reserves, in Deposit.Sizes sizes)
            {
                reserves.Value += sizes.Open * 1;
                reserves.Value += sizes.S * 2;
                reserves.Value += sizes.M * 4;
                reserves.Value += sizes.L * 8;
            }
        }
    }
    

}