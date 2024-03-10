using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Systems;
using YAPCG.Hub.Components;
using YAPCG.Planets.Components;

namespace YAPCG.Hub.Systems
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
            const int ANIMATION_REDUCTION = 0b1 << 5;
            
            void Execute(ref DiscoverProgress discoverProgress, ref BuildingSlotsLeft buildingSlotsLeft, ref Anim anim, ref AnimStart animStart)
            {
                discoverProgress.Value += discoverProgress.Progress;
                anim.Value = anim.Value <= ANIMATION_REDUCTION ? 0 : anim.Value - ANIMATION_REDUCTION;
                if (Unity.Burst.CompilerServices.Hint.Unlikely(discoverProgress.Value >= discoverProgress.MaxValue))
                {
                    discoverProgress.Value -= discoverProgress.MaxValue;
                    discoverProgress.MaxValue += DISCOVER_COST_INCREMENT;

                    buildingSlotsLeft.Medium++;
                    anim.Value = Anim.MAX_VALUE;
                    animStart.Time = Time;
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