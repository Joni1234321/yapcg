using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using YAPCG.Engine.Time.Systems;
using YAPCG.Hub.Components;
using YAPCG.Planets.Components;

namespace YAPCG.Planets.Systems
{
    [UpdateInGroup(typeof(TickDailyGroup))]
    public partial struct DiscoverSystem : ISystem
    {
        private BufferLookup<Deposit.Sizes> _depositSizesLookup;
        
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _depositSizesLookup = SystemAPI.GetBufferLookup<Deposit.Sizes>(false);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _depositSizesLookup.Update(ref state);
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            new HubJob().Run();
            new DepositJob() { ECB = ecb, DepositSizesLookup = _depositSizesLookup }.Run();
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }


        
        [BurstCompile]
        partial struct HubJob : IJobEntity
        {
            private const int DISCOVER_COST_INCREMENT = 30;
            void Execute(ref DiscoverProgress discoverProgress, ref BuildingSlotsLeft buildingSlotsLeft)
            {
                discoverProgress.Value += discoverProgress.Progress;
                if (Unity.Burst.CompilerServices.Hint.Unlikely(discoverProgress.Value >= discoverProgress.MaxValue))
                {
                    discoverProgress.Value -= discoverProgress.MaxValue;
                    discoverProgress.MaxValue += DISCOVER_COST_INCREMENT;

                    buildingSlotsLeft.Medium++;
                }
            }
        }
        
        [WithAll(typeof(Deposit.Sizes))]
        [BurstCompile]
        partial struct DepositJob : IJobEntity
        {
            private const int DISCOVER_COST_INCREMENT = 10;
            public EntityCommandBuffer ECB;
            public BufferLookup<Deposit.Sizes> DepositSizesLookup;
            void Execute(in Entity e, ref DiscoverProgress discoverProgress)
            {
                discoverProgress.Value += discoverProgress.Progress;
                
                if (discoverProgress.Value >= discoverProgress.MaxValue)
                {
                    discoverProgress.Value -= discoverProgress.MaxValue;
                    discoverProgress.MaxValue += DISCOVER_COST_INCREMENT;

                    DepositSizesLookup[e].ElementAt(0).Size++;
                }
            }
        }

    }
    
}