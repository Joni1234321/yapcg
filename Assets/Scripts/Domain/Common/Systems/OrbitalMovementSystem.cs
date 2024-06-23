using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Components;
using YAPCG.Engine.SystemGroups;
using YAPCG.Engine.Time.Components;
using YAPCG.Simulation.OrbitalMechanics;

namespace YAPCG.Domain.Common.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(RenderSystemGroup))]
    internal partial struct OrbitalMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Tick>();
        }
        private static readonly ProfilerMarker RENDER_ORBITS_MARKER = new ("Render Orbits FAST");

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float ticksF = SystemAPI.GetSingleton<Tick>().TicksF;
            new OrbitMovement
            {
                TicksF = ticksF
            }.Run();

            using (RENDER_ORBITS_MARKER.Auto())
            {
                NativeArray<Body.Orbit> orbits = SystemAPI.QueryBuilder().WithAll<Body.Orbit>().Build().ToComponentDataArray<Body.Orbit>(state.WorldUpdateAllocator);
                NativeArray<float4x4> matricies1 = CollectionHelper.CreateNativeArray<float4x4>(orbits.Length, state.WorldUpdateAllocator, NativeArrayOptions.UninitializedMemory);
                float3 orbit = new float3(0);
                GetOrbitMatricies(in orbit, in orbits, in ticksF, ref matricies1); 
            }
        }

        [BurstDiscard]
        private static void SetIfManaged(ref bool b) => b = false;

        private static bool IsBurst()
        {
            var b = true;
            SetIfManaged(ref b);
            return b;
        }
        
        [BurstCompile]
        public static void GetOrbitMatricies(in float3 orbitposition, in NativeArray<Body.Orbit> orbits, in float ticksF, ref NativeArray<float4x4> matricies)
        {
            const float MULTIPLIER = consts.DISTANCE_MULTIPLIER * 2 / 0.9f; // 0.9f = shader diameter
 
            if (IsBurst())
                Debug.Log("True");
            else
                Debug.Log("False");
            
            int n = orbits.Length;
            for (int i = 0; i < n; i++)
            {
                Body.Orbit orbit = orbits[i];
                float meanAnomaly = EllipseMechanics.CalculateMeanAnomaly(orbit.Period.Days, orbit.PeriodOffsetTicksF, ticksF);
                float trueAnomaly = EllipseMechanics.MeanAnomalyToTrueAnomaly(meanAnomaly, orbit.Eccentricity);
                quaternion rotation = quaternion.Euler(math.PIHALF, 0, math.PIHALF + trueAnomaly);
                float scale = orbit.AU * MULTIPLIER;
                matricies[i] = float4x4.TRS(orbitposition, rotation, new float3(scale));
            }
        }
        
        [BurstCompile]
        partial struct OrbitMovement : IJobEntity
        {
            [ReadOnly] 
            public float TicksF;
            
            void Execute(in Body.Orbit orbit, ref Position position)
            {
                if (orbit.AU == 0)
                {
                    position.Value = new float3(0);
                    return;
                }
                
                float meanAnomaly = EllipseMechanics.CalculateMeanAnomaly(orbit.Period.Days, orbit.PeriodOffsetTicksF, TicksF);
                float trueAnomaly = EllipseMechanics.MeanAnomalyToTrueAnomaly(meanAnomaly, orbit.Eccentricity);
                
                ExtraMechanics.OrbitData orbitData = new ExtraMechanics.OrbitData
                {
                    SemiMajorAxis = orbit.AU,
                    SemiMinorAxis = orbit.AU,
                };
                position.Value = ExtraMechanics.CalculatePositionOnOrbit(orbitData, trueAnomaly) * consts.DISTANCE_MULTIPLIER;
            }
        }
    }
    
}