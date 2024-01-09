using Unity.Burst;
using Unity.Entities;
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
            new DiscoverProgressJob().Run();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
    
    [BurstCompile]
    public partial struct DiscoverProgressJob : IJobEntity
    {
        private const int DISCOVER_COST_INCREMENT = 10;

        public void Execute(ref DiscoverProgress discoverProgress, ref BuildingSlotsLeft buildingSlotsLeft)
        {
            discoverProgress.Value += discoverProgress.Progress;
            if (discoverProgress.Value >= discoverProgress.MaxValue)
            {
                discoverProgress.Value -= discoverProgress.MaxValue;
                discoverProgress.MaxValue += DISCOVER_COST_INCREMENT;

                buildingSlotsLeft.Medium++;
            }
        }
    }
}