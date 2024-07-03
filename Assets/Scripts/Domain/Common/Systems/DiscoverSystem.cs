using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using YAPCG.Domain.Common.Components;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Systems;

namespace YAPCG.Domain.Common.Systems
{
    [UpdateInGroup(typeof(TickDailyGroup))]
    internal partial struct DiscoverSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new HubJob { Time = (float)SystemAPI.Time.ElapsedTime }.Run();
            new BodyJob { Time = (float)SystemAPI.Time.ElapsedTime }.Run();
        }

        [BurstCompile]
        partial struct HubJob : IJobEntity
        {
            [ReadOnly] 
            public float Time;
            
            const int DISCOVER_COST_INCREMENT = 30;
            
            void Execute(ref DiscoverProgress discoverProgress, ref BuildingSlotsLeft buildingSlotsLeft, ref FadeStartTimeComponent fadeStartTimeComponent)
            {
                discoverProgress.Value += discoverProgress.Progress;
                if (Unity.Burst.CompilerServices.Hint.Unlikely(discoverProgress.Value >= discoverProgress.MaxValue))
                {
                    discoverProgress.Value -= discoverProgress.MaxValue;
                    discoverProgress.MaxValue += DISCOVER_COST_INCREMENT;

                    buildingSlotsLeft.Medium++;
                    fadeStartTimeComponent.FadeStartTime = Time;
                }
            }
        }
        
        [BurstCompile]
        partial struct BodyJob : IJobEntity
        {
            [ReadOnly] 
            public float Time;
            
            const int DISCOVER_COST_INCREMENT = 10;
            
            void Execute(ref DiscoverProgress discoverProgress, ref FadeStartTimeComponent fadeStartTimeComponent)
            {
                discoverProgress.Value += discoverProgress.Progress;
                if (Unity.Burst.CompilerServices.Hint.Unlikely(discoverProgress.Value >= discoverProgress.MaxValue))
                {
                    discoverProgress.Value -= discoverProgress.MaxValue;
                    discoverProgress.MaxValue += DISCOVER_COST_INCREMENT;

                    fadeStartTimeComponent.FadeStartTime = Time;
                }
            }
        }
        [BurstCompile]
        partial struct DepositJob : IJobEntity
        {
            private const int DISCOVER_COST_INCREMENT = 10;
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