using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Components;
using YAPCG.Engine.SystemGroups;
using YAPCG.Engine.Time.Components;
using YAPCG.Simulation.OrbitalMechanics;

namespace YAPCG.Domain.Common.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    internal partial struct OrbitalMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DeltaTick>();
            state.RequireForUpdate<Ticks>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float ticksF = SystemAPI.GetSingleton<Ticks>().Value + SystemAPI.GetSingleton<DeltaTick>().Value;
            new OrbitMovement
            {
                TicksF = ticksF
            }.Run();
        }

        [BurstCompile]
        partial struct OrbitMovement : IJobEntity
        {
            [ReadOnly] 
            public float TicksF;
            
            void Execute(in Body.Orbit orbit, ref Position position)
            {
                if (orbit.Distance == 0)
                {
                    position.Value = new float3(0);
                    return;
                }
                
                ExtraMechanics.OrbitData orbitData = new ExtraMechanics.OrbitData
                {
                    SemiMajorAxis = orbit.Distance,
                    SemiMinorAxis = orbit.Distance,
                };
                
                float meanAnomaly = math.PI2 * TicksF / orbit.Period.Days;
                float trueAnomaly = EllipseMechanics.MeanAnomalyToTrueAnomaly(meanAnomaly, orbit.Eccentricity);
                position.Value = ExtraMechanics.GetPositionOnOrbit(orbitData, trueAnomaly) * 10f;
            }
        }
    }
    
}