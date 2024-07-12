using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.Common.Components;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Systems;

namespace YAPCG.Domain.Common.Systems
{
    [UpdateInGroup(typeof(TickWeeklyGroup))]
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
            new LandDiscoveryJob().Run();
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
                    fadeStartTimeComponent.FadeStartTime = Time;
                }
            }
        }
        
        [BurstCompile]
        partial struct LandDiscoveryJob : IJobEntity
        {
            private const uint MULT = 10;
            private const uint MAX_LAND = 1_000_000_000;
            private const uint MAX_ORBITS = MAX_LAND / (MULT * MULT * MULT);
            void Execute(ref LandDiscovery discovery)
            {
                discovery.People += math.min(discovery.People - discovery.Probes * MULT, discovery.PeopleThroughput);
                discovery.Probes += math.min(discovery.Probes - discovery.Orbit * MULT, discovery.ProbesThroughput);
                discovery.Orbit += math.min(discovery.Orbit - MAX_ORBITS, discovery.OrbitThroughput);
            }
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