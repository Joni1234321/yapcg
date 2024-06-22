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
        public unsafe void OnUpdate(ref SystemState state)
        {
            _needsBufferLookup.Update(ref state);
            _laborLookup.Update(ref state);


            Random* random;
            fixed (Random* ptr = &SystemAPI.GetSingletonRW<SharedRandom>().ValueRW.Random) { random = ptr; }
            
            new PopulationGrowthJob { Random = random, MinGrowth = MIN_GROWTH, MaxGrowth = MAX_GROWTH }.Run();
            new PopulationNeedsJob { NeedsBufferLookup = _needsBufferLookup }.Run();
            new MigrationAttractionJob {}.Run();

            NativeArray<Entity> destinations = SystemAPI.QueryBuilder().WithAll<LaborMigration>().Build().ToEntityArray(state.WorldUpdateAllocator);
            new MigrationDestinationJob { Random = random , Destinations = destinations, LaborLookup = _laborLookup }.Run();
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
            migration.EmigratingPopulation = (long)math.log(migration.Out * EMIGRATION_RATE * labor.Population);
        }
    }
    
    [BurstCompile]
    partial struct MigrationDestinationJob : IJobEntity
    {
        [NativeDisableUnsafePtrRestriction]
        public unsafe Random* Random; 
        
        [ReadOnly] 
        public NativeArray<Entity> Destinations;
        public ComponentLookup<Labor> LaborLookup;
        
        unsafe void Execute(in Entity e, ref LaborMigration migration)
        {
            migration.Target = Destinations[Random->NextInt(Destinations.Length)];

            LaborLookup.GetRefRW(e).ValueRW.Population -= migration.EmigratingPopulation;
            LaborLookup.GetRefRW(migration.Target).ValueRW.Population += migration.EmigratingPopulation;
        }
    }
    
    [BurstCompile]
    partial struct PopulationGrowthJob : IJobEntity
    {
        [NativeDisableUnsafePtrRestriction]
        public unsafe Random* Random; 
        [ReadOnly]
        public float MinGrowth, MaxGrowth;

        unsafe void Execute(ref Labor labor, ref LaborExtras extras)
        {
            float growth = Random->NextFloat(MinGrowth, MaxGrowth);
            float housing = randomutils.tanh_diff((float)labor.Population / labor.Ceiling);
            
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