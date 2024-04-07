using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using YAPCG.Domain.Hub.Components;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Systems;

namespace YAPCG.Domain.Hub.Systems
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
        }


        
        [BurstCompile]
        partial struct HubJob : IJobEntity
        {
            [ReadOnly] 
            public float Time;
            
            const int DISCOVER_COST_INCREMENT = 30;
            
            void Execute(ref DiscoverProgress discoverProgress, ref BuildingSlotsLeft buildingSlotsLeft, ref AnimationComponent animationComponent)
            {
                discoverProgress.Value += discoverProgress.Progress;
                if (Unity.Burst.CompilerServices.Hint.Unlikely(discoverProgress.Value >= discoverProgress.MaxValue))
                {
                    discoverProgress.Value -= discoverProgress.MaxValue;
                    discoverProgress.MaxValue += DISCOVER_COST_INCREMENT;

                    buildingSlotsLeft.Medium++;
                    animationComponent.AnimationStart = Time;
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