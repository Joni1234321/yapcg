using NUnit.Framework;
using Unity.Burst;
using Unity.Entities;
using Unity.PerformanceTesting;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using YAPCG.Domain;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Components;
using YAPCG.Simulation.Units;
using YAPCG.Simulation.OrbitalMechanics;

namespace YAPCG.Benchmarks.Test
{
    [BurstCompile]
    public class OrbitObjectMatrix : MonoBehaviour
    {
        private const int N = 5000;
        private float[] anomalies = new float[N];
        private Body.Orbit[] orbits = new Body.Orbit[N];
        private Position[] positions = new Position[N];
        private float ticksF;

        private static readonly float3 ROTATION_ANGLE = new float3(math.PIHALF, 0, math.PI);
        private static readonly quaternion ADDITIONAL_ROTATION = quaternion.Euler(ROTATION_ANGLE);
        string[] markers =
        {
            "Instantiate",
            "Instantiate.Copy",
            "Instantiate.Produce",
            "Instantiate.Awake"
        };
        [SetUp]
        public void SetUp()
        {
            ticksF = 10;
            for (int i = 0; i < N; i++)
            {
                orbits[i] = new Body.Orbit()
                {
                    Eccentricity = 0.4f,
                    AU = 0.2f * i,
                    Parent = Entity.Null,
                    PeriodOffsetTicksF = 10 * i / 2f,
                    Period = new SiTime(3_556_952 * i)
                };
                positions[i] = new Position() { Value = new float3(0, i, 2*i)};
                anomalies[i] = i * 2 / math.PI;
            }
        }
        
        [Test, Performance]
        public void Rotation_LookAt_Calculation()
        {

            Measure.Method(() =>
                {
                    for (int i = 0; i < orbits.Length; i++)
                        OrbitToMatrixLookAt(positions[i], orbits[i]);
                })
                .WarmupCount(20)
                .MeasurementCount(100)
                .Run();

        }
        [Test, Performance]
        public void Rotation_LookAtFast_Calculation()
        {

            Measure.Method(() =>
                {
                    for (int i = 0; i < orbits.Length; i++)
                        OrbitToMatrixLookAtFast(positions[i], orbits[i], out float4x4 matrix); 
                })
                .WarmupCount(20)
                .MeasurementCount(100)
                .Run();
        }

        private string[] markers2 = new[]
        {
            "fish",
            "fish1",
            "fish2",
            "EllipseMechanics.CalculateMeanAnomaly",
            "MeanAnomalyToTrueAnomaly",
            "euler"
        };
        [Test, Performance]
        public void Rotation_Anomaly_Calculation()
        {
            Measure.ProfilerMarkers("MainProfilerMarker");
            using (Measure.ProfilerMarkers(markers2))
            {
                using (Measure.Scope())
                {
                    Measure.Method(() =>
                        {
                            for (int i = 0; i < orbits.Length; i++)
                                OrbitToMatrixAnomaly(orbits[i], ticksF);
                        })
                        .MeasurementCount(10)
                        .ProfilerMarkers("fish")
                        .Run();
                    Measure.Method(() =>
                        {
                            for (int i = 0; i < orbits.Length; i++)
                                OrbitToMatrixAnomaly(orbits[i], ticksF * 2);
                        })
                        .MeasurementCount(10)
                        .ProfilerMarkers("fish1")

                        .Run();
                    Measure.Method(() =>
                        {
                            for (int i = 0; i < orbits.Length; i++)
                                OrbitToMatrixAnomaly(orbits[i], 2);
                        })
                        .MeasurementCount(10)
                        .ProfilerMarkers("fish2")

                        .Run();
                }
            }
        }


        [Test, Performance]
        public void Instantiate_CreateCubes()
        {
            using (Measure.ProfilerMarkers(markers))
            {
                using(Measure.Scope())
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    for (var i = 0; i < 5000; i++)
                    {
                        UnityEngine.Object.Instantiate(cube);
                    }
                }
            }
        }

        
        [Test, Performance]
        public void Rotation_WithGivenAnomaly_Calculation()
        {

            Measure.Method(() =>
                {
                    for (int i = 0; i < orbits.Length; i++)
                    {
                        float scale = orbits[i].AU * MULTIPLIER;
                        quaternion rotation = quaternion.Euler(ROTATION_ANGLE + new float3(anomalies[i]));
                        float4x4.TRS(new float3(0), rotation, new float3(scale));
                    }
                })
                .WarmupCount(20)
                .MeasurementCount(100)

                .Run();
        }
        
        const float MULTIPLIER = 4;

        public static Matrix4x4 OrbitToMatrixLookAt(in Position position, in Body.Orbit orbit)
        {
            quaternion lookRotation = quaternion.LookRotation(position.Value, new float3(0, 1, 0));
            quaternion rotation = math.mul(lookRotation, ADDITIONAL_ROTATION);
            float scale = orbit.AU * MULTIPLIER;
            return float4x4.TRS(new float3(0), rotation, new float3(scale));
        }

        [BurstCompile]
        public static void OrbitToMatrixLookAtFast(in Position position, in Body.Orbit orbit, out float4x4 matrix)
        {
            quaternion lookRotation = quaternion.LookRotation(position.Value, new float3(0, 1, 0));
            quaternion rotation = math.mul(lookRotation, ADDITIONAL_ROTATION);
            float scale = orbit.AU * MULTIPLIER;
            matrix = float4x4.TRS(new float3(0), rotation, new float3(scale));
        } 
        
        public static Matrix4x4 OrbitToMatrixAnomaly(in Body.Orbit orbit, float ticksF)
        { 
            float meanAnomaly = EllipseMechanics.CalculateMeanAnomaly(orbit.Period.Days, orbit.PeriodOffsetTicksF, ticksF);
            float trueAnomaly = EllipseMechanics.MeanAnomalyToTrueAnomaly(meanAnomaly, orbit.Eccentricity);
            quaternion rotation = quaternion.Euler(math.PIHALF, 0, math.PIHALF + trueAnomaly);
            float scale = orbit.AU * MULTIPLIER;
            return float4x4.TRS(new float3(0), rotation, new float3(scale));
        } 
    }
}