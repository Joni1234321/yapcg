using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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
        private BufferLookup<LaborNeed> _needsBufferLookup;
        private ComponentLookup<Labor> _laborLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _needsBufferLookup = state.GetBufferLookup<LaborNeed>();
            _laborLookup = state.GetComponentLookup<Labor>();
            
            state.RequireForUpdate<SharedRandom>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _needsBufferLookup.Update(ref state);
            _laborLookup.Update(ref state);

            RefRW<SharedRandom> sharedRandom = SystemAPI.GetSingletonRW<SharedRandom>();
            
            
            new PopulationGrowthJob { SharedRandom = sharedRandom, MinGrowth = MIN_GROWTH, MaxGrowth = MAX_GROWTH }.Run();
            new PopulationNeedsJob { NeedsBufferLookup = _needsBufferLookup }.Run();
            new MigrationAttractionJob {}.Run();

            NativeArray<Entity> destinations = SystemAPI.QueryBuilder().WithAll<LaborMigration>().Build().ToEntityArray(state.WorldUpdateAllocator);
            new MigrationDestinationJob { SharedRandom = sharedRandom, Destinations = destinations, LaborLookup = _laborLookup }.Run();

            //NativeArray<LaborMigration> migrations = SystemAPI.QueryBuilder().WithAll<LaborMigration>().Build().ToComponentDataArray<LaborMigration>(state.WorldUpdateAllocator);
            //migrations.Sort(new LaborMigration.InSorter());
        }

    }
    
    [BurstCompile]
    partial struct MigrationAttractionJob : IJobEntity
    {
        private const float EMIGRATION_RATE = 0.004f;
        void Execute(in Deposit.Reserves reserves, in Labor labor, in LaborExtras extras, ref LaborMigration migration)
        {
            migration.In = extras.NeedsMissing;
            migration.Out = extras.NeedsMissing;
            migration.EmigratingPopulation = (long)math.ceil(migration.Out * EMIGRATION_RATE * labor.Population);
        }
    }
    
    [BurstCompile]
    partial struct MigrationDestinationJob : IJobEntity
    {
        
        public readonly unsafe int* SharedRandom; 
        [ReadOnly] public NativeArray<Entity> Destinations;
        public ComponentLookup<Labor> LaborLookup;
        
        void Execute(ref Labor labor, ref LaborMigration migration)
        {
            migration.Target = Destinations[SharedRandom.ValueRW.Random.NextInt(Destinations.Length)];
            labor.Population -= migration.EmigratingPopulation;

            Labor updatedLabor = LaborLookup[migration.Target];
            updatedLabor.Population += migration.EmigratingPopulation;
            LaborLookup[migration.Target] = updatedLabor;
        }
    }
    
    [BurstCompile]
    partial struct PopulationGrowthJob : IJobEntity
    {
        public RefRW<SharedRandom> SharedRandom;
        public float MinGrowth, MaxGrowth;

        void Execute(ref Labor labor, ref LaborExtras extras)
        {
            float growth = SharedRandom.ValueRW.Random.NextFloat(MinGrowth, MaxGrowth);
            float housing = mathutils.tanh_diff((float)labor.Population / labor.Ceiling);
            
            extras.HousingModifier = housing;
            extras.LaborDifference = (long)math.ceil(labor.Population * growth * housing);
            labor.Population += extras.LaborDifference;
        }
    }


    [BurstCompile]
    [WithAll(typeof(LaborNeed))]
    partial struct PopulationNeedsJob : IJobEntity
    {
        public BufferLookup<LaborNeed> NeedsBufferLookup;

        void Execute(in Entity e, ref LaborExtras extras)
        {
            DynamicBuffer<LaborNeed> needs = NeedsBufferLookup[e];
            extras.NeedsMissing = 0;
            
            for (int i = 0; i < needs.Length; i++)
            {
                LaborNeed need = needs[i];
                need.Received = 5;

                extras.NeedsMissing += math.max(0, need.Need - need.Received);
                
                needs[i] = need;
            }
        }
    }
}