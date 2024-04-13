using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.Common.Components;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Systems;

namespace YAPCG.Domain.Common.Systems
{
    [UpdateInGroup(typeof(TickWeeklyGroup))]
    [UpdateAfter(typeof(ProductionSystem))]
    public partial struct PopulationSystem : ISystem
    {
        private const float MIN_GROWTH = 0.01f, MAX_GROWTH = 0.02f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SharedRandom>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Random random = SystemAPI.GetSingleton<SharedRandom>().Random;
            new MigrationJob().Run();
            new PopulationGrowthJob { Random = random, MinGrowth = MIN_GROWTH, MaxGrowth = MAX_GROWTH }.Run();
        }
    }
    
    [BurstCompile]
    partial struct MigrationJob : IJobEntity
    {
        void Execute(in Deposit.Reserves reserves, ref Labor labor, ref LaborExtras extras)
        {
            extras.MigrationValue = (int)(reserves.Value / labor.Population);
            extras.EmigrationValue = (int)(reserves.Value / labor.Population);
        }
    }

    [BurstCompile]
    partial struct PopulationGrowthJob : IJobEntity
    {
        public Random Random;
        public float MinGrowth, MaxGrowth;

        void Execute(ref Labor labor, ref LaborExtras extras)
        {
            float growth = Random.NextFloat(MinGrowth, MaxGrowth);
            float modifier = mathutils.tanh_diff(labor.Population / labor.Ceiling);
            
            extras.LaborDifference = (long)math.ceil(labor.Population * growth * modifier);
            labor.Population += extras.LaborDifference;
        }
    }
}