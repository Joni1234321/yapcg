using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Systems;
using YAPCG.Hub.Components;
using YAPCG.Planets.Components;

namespace YAPCG.Planets.Systems
{
    [UpdateInGroup(typeof(TickDailyGroup))]
    public partial struct DiscoverSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new HubJob().Run();
        }


        
        [BurstCompile]
        partial struct HubJob : IJobEntity
        {
            private const int DISCOVER_COST_INCREMENT = 30;
            void Execute(ref DiscoverProgress discoverProgress, ref BuildingSlotsLeft buildingSlotsLeft, ref Anim anim)
            {
                discoverProgress.Value += discoverProgress.Progress;
                anim.Value = (anim.Value == 0) ? 0 : anim.Value - 1;
                if (Unity.Burst.CompilerServices.Hint.Unlikely(discoverProgress.Value >= discoverProgress.MaxValue))
                {
                    discoverProgress.Value -= discoverProgress.MaxValue;
                    discoverProgress.MaxValue += DISCOVER_COST_INCREMENT;

                    buildingSlotsLeft.Medium++;
                    anim.Value = 10;
                }
            }
        }
        
        [BurstCompile]
        partial struct DepositJob : IJobEntity
        {
            private const int DISCOVER_COST_INCREMENT = 10;
            public EntityCommandBuffer ECB;
            void Execute(in Entity e, ref DiscoverProgress discoverProgress, ref Deposit.Sizes sizes)
            {
                discoverProgress.Value += discoverProgress.Progress;
                
                if (discoverProgress.Value >= discoverProgress.MaxValue)
                {
                    discoverProgress.Value -= discoverProgress.MaxValue;
                    discoverProgress.MaxValue += DISCOVER_COST_INCREMENT;

                    sizes.Open++;
                }
            }
        }

    }
    
}