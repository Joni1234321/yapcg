using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
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
            _orbitQuery = SystemAPI.QueryBuilder().WithAll<Body.Orbit, Position, Body.TrueAnomaly>().Build();

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

            NativeArray<Body.Orbit> orbits = _orbitQuery.ToComponentDataArray<Body.Orbit>(WorldUpdateAllocator);
            NativeArray<Body.TrueAnomaly> trueAnomalies = _orbitQuery.ToComponentDataArray<Body.TrueAnomaly>(WorldUpdateAllocator);
                
            NativeArray<float4x4> matricies = CollectionHelper.CreateNativeArray<float4x4>(orbits.Length, WorldUpdateAllocator);
            
            using (RENDER_ORBITS_MARKER.Auto())
                GetOrbitMatricies(new float3(0), orbits, trueAnomalies, ref matricies);
            
            if (matricies.Length == 0)
                return;
            
            Graphics.RenderMeshInstanced(renderParams, mesh, 0, matricies.Reinterpret<Matrix4x4>());
        }

        [BurstCompile]
        private static void GetOrbitMatricies(in float3 orbitPosition, in NativeArray<Body.Orbit> orbits, in NativeArray<Body.TrueAnomaly> trueAnomalies, ref NativeArray<float4x4> matricies)
        {
            const float MULTIPLIER = consts.DISTANCE_MULTIPLIER * 2 / 0.9f; // 0.9f = shader diameter
            int n = orbits.Length;
            for (int i = 0; i < n; i++)
            {
                quaternion rotation = quaternion.Euler(math.PIHALF, 0, math.PIHALF + trueAnomalies[i].Value);
                float scale = orbits[i].AU * MULTIPLIER;
                matricies[i] = Matrix4x4.TRS(orbitPosition, rotation, new float3(scale));
            }
        }
    }
}