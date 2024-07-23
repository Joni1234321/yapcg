﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using YAPCG.Application.UserInterface;
using YAPCG.Domain;
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
        private RenderUtils.PositionScaleAlternativeMaterialRender _planetRenderer, _sunRenderer, _asteroidRenderer;

        protected override void OnCreate()
        {
            RequireForUpdate<MeshesSingleton>();

            _planetRenderer = new RenderUtils.PositionScaleAlternativeMaterialRender();
            _sunRenderer = new RenderUtils.PositionScaleAlternativeMaterialRender();
            _asteroidRenderer = new RenderUtils.PositionScaleAlternativeMaterialRender();
        }

        protected override void OnUpdate()
        {
            if (SystemAPI.GetSingleton<ApplicationSettings>().DisableBodyRender)
                return;
            // OPTIMIZATION: SET COMPONENT FLAG THAT WHENEVER ANOTHER OBJECT SPAWNS, THEN CHANGE COMMAND, OTHERWISE DONT
            RenderPlanets();
            RenderSuns();
            RenderOrbits();
            RenderAsteroids();
        }


        protected override void OnDestroy()
        {
            _planetRenderer.Dispose();
            _sunRenderer.Dispose();
            _asteroidRenderer.Dispose();
        } 

        private void RenderPlanets()
        {
            MeshesSingleton meshes = SystemAPI.GetSingleton<MeshesSingleton>();

            if (!meshes.Planet.LoadStarted)
            {
                meshes.Planet.LoadAsync();
                SystemAPI.SetSingleton(meshes);
                return;
            }

            if (!meshes.Planet.Loaded())
                return;

            EntityQuery planetQuery = SystemAPI.QueryBuilder().WithAll<Position, ScaleComponent, FadeStartTimeComponent, AlternativeColorRatio, Body.PlanetTag>().Build();
            _planetRenderer.Render(planetQuery, meshes.Planet.Mesh.Result, meshes.Planet.Material.Result, WorldUpdateAllocator);
        }


        private void RenderSuns()
        {
            MeshesSingleton meshes = SystemAPI.GetSingleton<MeshesSingleton>();

            if (!meshes.Sun.LoadStarted)
            {
                meshes.Sun.LoadAsync();
                SystemAPI.SetSingleton(meshes);
                return;
            }

            if (!meshes.Sun.Loaded())
                return;
            
            EntityQuery sunQuery = SystemAPI.QueryBuilder().WithAll<Position, ScaleComponent, FadeStartTimeComponent, AlternativeColorRatio, Body.SunTag>().Build();
            _sunRenderer.Render(sunQuery, meshes.Sun.Mesh.Result, meshes.Sun.Material.Result, WorldUpdateAllocator);
        }

        private static readonly ProfilerMarker RENDER_ORBITS_MARKER = new("Render Orbits");

        private void RenderOrbits()
        {
            MeshesSingleton meshes = SystemAPI.GetSingleton<MeshesSingleton>();

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

            EntityQuery orbitQuery = SystemAPI.QueryBuilder().WithAny<Body.SunTag, Body.PlanetTag>().WithAll<Body.Orbit, Position, Body.TrueAnomaly>().Build();
            NativeArray<Body.Orbit> orbits = orbitQuery.ToComponentDataArray<Body.Orbit>(WorldUpdateAllocator);
            NativeArray<Body.TrueAnomaly> trueAnomalies = orbitQuery.ToComponentDataArray<Body.TrueAnomaly>(WorldUpdateAllocator);
                
            NativeArray<float4x4> matricies = CollectionHelper.CreateNativeArray<float4x4>(orbits.Length, WorldUpdateAllocator);
            
            using (RENDER_ORBITS_MARKER.Auto())
                GetOrbitMatricies(new float3(0), orbits, trueAnomalies, ref matricies);
            
            if (matricies.Length == 0)
                return;
            
            Graphics.RenderMeshInstanced(renderParams, mesh, 0, matricies.Reinterpret<Matrix4x4>());
        }
        
        public void RenderAsteroids()
        {
            MeshesSingleton meshes = SystemAPI.GetSingleton<MeshesSingleton>();

            if (!meshes.Asteroid.LoadStarted)
            {
                meshes.Asteroid.LoadAsync();
                SystemAPI.SetSingleton(meshes);
                return;
            }

            if (!meshes.Asteroid.Loaded())
                return;
            
            EntityQuery asteroidQuery = SystemAPI.QueryBuilder().WithAll<Position, ScaleComponent, FadeStartTimeComponent, AlternativeColorRatio, Body.AsteroidTag>().Build();
            _asteroidRenderer.Render(asteroidQuery, meshes.Asteroid.Mesh.Result, meshes.Asteroid.Material.Result, WorldUpdateAllocator);
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