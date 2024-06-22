using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using YAPCG.Application.UserInterface;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Common.DOTS;
using YAPCG.Engine.Components;
using YAPCG.Engine.Render;
using YAPCG.Engine.SystemGroups;
using YAPCG.Engine.Time.Components;
using YAPCG.Simulation.OrbitalMechanics;

namespace YAPCG.Application.Render.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(RenderSystemGroup))]
    [BurstCompile]
    internal partial class BodyRenderSystem : SystemBase
    {
        private EntityQuery _planetQuery, _sunQuery, _orbitQuery;
        private RenderUtils.PositionScaleAlternativeMaterialRender _planetRenderer, _sunRenderer;

        protected override void OnCreate()
        {
            _planetQuery = SystemAPI.QueryBuilder()
                .WithAll<Position, ScaleComponent, FadeStartTimeComponent, AlternativeColorRatio, Body.PlanetTag>()
                .Build();
            _sunQuery = SystemAPI.QueryBuilder()
                .WithAll<Position, ScaleComponent, FadeStartTimeComponent, AlternativeColorRatio, Body.SunTag>()
                .Build();
            _orbitQuery = SystemAPI.QueryBuilder().WithAll<Body.Orbit, Position>().Build();

            RequireForUpdate<MeshesSingleton>();

            _planetRenderer = new RenderUtils.PositionScaleAlternativeMaterialRender();
            _sunRenderer = new RenderUtils.PositionScaleAlternativeMaterialRender();
        }

        protected override void OnUpdate()
        {
            if (SystemAPI.GetSingleton<ApplicationSettings>().DisableBodyRender)
                return;
            // OPTIMIZATION: SET COMPONENT FLAG THAT WHENEVER ANOTHER OBJECT SPAWNS, THEN CHANGE COMMAND, OTHERWISE DONT
            RenderPlanets();
            RenderSuns();
            RenderOrbits();
        }


        protected override void OnDestroy()
        {
            _planetRenderer.Dispose();
            _sunRenderer.Dispose();
        }

        private void RenderPlanets()
        {
            var meshes = SystemAPI.GetSingleton<MeshesSingleton>();

            if (!meshes.Planet.LoadStarted)
            {
                meshes.Planet.LoadAsync();
                SystemAPI.SetSingleton(meshes);
                return;
            }

            if (!meshes.Planet.Loaded())
                return;

            _planetRenderer.Render(_planetQuery, meshes.Planet.Mesh.Result, meshes.Planet.Material.Result,
                WorldUpdateAllocator);
        }


        private void RenderSuns()
        {
            var meshes = SystemAPI.GetSingleton<MeshesSingleton>();

            if (!meshes.Sun.LoadStarted)
            {
                meshes.Sun.LoadAsync();
                SystemAPI.SetSingleton(meshes);
                return;
            }

            if (!meshes.Sun.Loaded())
                return;

            _sunRenderer.Render(_sunQuery, meshes.Sun.Mesh.Result, meshes.Sun.Material.Result, WorldUpdateAllocator);
        }

        private static readonly ProfilerMarker RENDER_ORBITS_MARKER = new("Render Orbits");


        private void RenderOrbits()
        {
            var meshes = SystemAPI.GetSingleton<MeshesSingleton>();
            float ticksF = SystemAPI.GetSingleton<Tick>().TicksF;

            if (!meshes.Orbit.LoadStarted)
            {
                meshes.Orbit.LoadAsync();
                SystemAPI.SetSingleton(meshes);
                return;
            }

            if (!meshes.Orbit.Loaded())
                return;

            Mesh mesh = meshes.Orbit.Mesh.Result;
            Material material = meshes.Orbit.Material.Result;
            RenderParams renderParams = new RenderParams(material)
                { worldBounds = new Bounds(float3.zero, new float3(1000)) };

//using (RENDER_ORBITS_MARKER.Auto())
            {
                NativeArray<Body.Orbit> orbits = _orbitQuery.ToComponentDataArray<Body.Orbit>(WorldUpdateAllocator);
                NativeArray<Position> positions = _orbitQuery.ToComponentDataArray<Position>(WorldUpdateAllocator);
#if FALSE
    NativeArray<float4x4> matricies1 =
 new NativeArray<float4x4>(orbits.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
    GetOrbitMatricies(new float3(0), orbits, ticksF, matricies1);
    Graphics.RenderMeshInstanced(renderParams, mesh, 0, matricies1.Reinterpret<Matrix4x4>());
#else
                Matrix4x4[] matricies = GetOrbitMatricies(new float3(0), orbits, positions, ticksF);
                Graphics.RenderMeshInstanced(renderParams, mesh, 0, matricies);
#endif
            }
        }



        public static void GetOrbitMatricies(float3 orbitposition, NativeArray<Body.Orbit> orbits,
            NativeArray<Position> positions, float ticksF, NativeArray<float4x4> matricies)
        {
            const float MULTIPLIER = consts.DISTANCE_MULTIPLIER * 2 / 0.9f; // 0.9f = shader diameter

            if (BurstDebug.IsBurst())
                Debug.Log("True");
            else
                Debug.Log("False");

            int n = orbits.Length;
            for (int i = 0; i < n; i++)
            {
                Body.Orbit orbit = orbits[i];
                float meanAnomaly =
                    EllipseMechanics.CalculateMeanAnomaly(orbit.Period.Days, orbit.PeriodOffsetTicksF, ticksF);
                float trueAnomaly = EllipseMechanics.MeanAnomalyToTrueAnomaly(meanAnomaly, orbit.Eccentricity);

                quaternion rotation = quaternion.Euler(math.PIHALF, 0, math.PIHALF + trueAnomaly);
                float scale = orbit.AU * MULTIPLIER;
                matricies[i] = float4x4.TRS(orbitposition, rotation, new float3(scale));
            }
        }


        Matrix4x4[] GetOrbitMatricies(float3 orbitposition, NativeArray<Body.Orbit> orbits,
            NativeArray<Position> positions, float ticksF)
        {
            int n = orbits.Length;
            Matrix4x4[] matricies = new Matrix4x4[n];

            for (int i = 0; i < n; i++)
            {
                Body.Orbit orbit = orbits[i];
                float meanAnomaly =
                    EllipseMechanics.CalculateMeanAnomaly(orbit.Period.Days, orbit.PeriodOffsetTicksF, ticksF);
                float trueAnomaly = EllipseMechanics.MeanAnomalyToTrueAnomaly(meanAnomaly, orbit.Eccentricity);
#if FALSE
    quaternion rotation1 = quaternion.LookRotation(positions[i].Value, new float3(0, 1, 0));
#else
                quaternion rotation1 = quaternion.Euler(math.PIHALF, 0, math.PIHALF + trueAnomaly);
                quaternion lookRotation = quaternion.LookRotation(positions[i].Value, new float3(0, 1, 0));
                quaternion additionalRotation = quaternion.Euler(math.PIHALF, 0, math.PI);
                quaternion rotation = math.mul(lookRotation, additionalRotation);
#endif
                const float MULTIPLIER = consts.DISTANCE_MULTIPLIER * 2 / 0.9f; // 0.9f = shader diameter
                float scale = orbit.AU * MULTIPLIER;
                matricies[i] = Matrix4x4.TRS(orbitposition, rotation, new float3(scale));
            }

            return matricies;
        }
/*
[BurstCompile]
public partial struct OrbitMatricesJob : IJobEntity
{
[WriteOnly][NativeDisableParallelForRestriction] public NativeArray<float4x4> Matricies;
public void Execute(int i, in Body.Orbit orbit)
{

}
}*/
    }
}