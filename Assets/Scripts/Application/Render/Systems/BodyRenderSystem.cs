using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Application.UserInterface;
using YAPCG.Domain.NUTS;
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
        [BurstCompile]
        protected override void OnCreate()
        {
            _planetQuery = SystemAPI.QueryBuilder().WithAll<Position, ScaleComponent, FadeStartTimeComponent, AlternativeColorRatio, Body.PlanetTag>().Build();
            _sunQuery = SystemAPI.QueryBuilder().WithAll<Position, ScaleComponent, FadeStartTimeComponent, AlternativeColorRatio, Body.SunTag>().Build();
            _orbitQuery = SystemAPI.QueryBuilder().WithAll<Body.Orbit>().Build();
            
            RequireForUpdate<MeshesSingleton>();
           
            _planetRenderer = new RenderUtils.PositionScaleAlternativeMaterialRender();
            _sunRenderer = new RenderUtils.PositionScaleAlternativeMaterialRender();
        }

        protected override void OnUpdate()
        {
            // OPTIMIZATION: SET COMPONENT FLAG THAT WHENEVER ANOTHER OBJECT SPAWNS, THEN CHANGE COMMAND, OTHERWISE DONT
            RenderPlanets();
            RenderSuns();
            RenderOrbits();
        }

        
        [BurstCompile]
        protected override void OnDestroy()
        {
            _planetRenderer.Dispose();
            _sunRenderer.Dispose();
        }

        [BurstDiscard]
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

            _planetRenderer.Render(_planetQuery, meshes.Planet.Mesh.Result, meshes.Planet.Material.Result, WorldUpdateAllocator);
        }
        
        
        [BurstDiscard]
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

        [BurstDiscard]
        private void RenderOrbits()
        {
            var meshes = SystemAPI.GetSingleton<MeshesSingleton>();

#if UNITY_EDITOR
            float ticksF = 0;
#else
            float ticksF = SystemAPI.GetSingleton<Tick>().TicksF;
#endif
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
            RenderParams renderParams = new RenderParams(material) { worldBounds = new Bounds(float3.zero, new float3(1000))};

            NativeArray<Body.Orbit> orbits = _orbitQuery.ToComponentDataArray<Body.Orbit>(Allocator.Temp);
            Matrix4x4[] matricies = GetOrbitMatricies(new float3(0), orbits, ticksF);

            foreach (var matrix in matricies)
                Graphics.RenderMesh(renderParams, mesh, 0, matrix);
        }

        Matrix4x4[] GetOrbitMatricies(float3 orbitposition, NativeArray<Body.Orbit> orbits, float ticksF)
        {
            int n = orbits.Length;
            Matrix4x4[] matricies = new Matrix4x4[n];
            
            for (int i = 0; i < n; i++)
            {
                Body.Orbit orbit = orbits[i];
                float meanAnomaly = EllipseMechanics.CalculateMeanAnomaly(orbit.Period.Days, orbit.PeriodOffsetTicksF, ticksF);
                float trueAnomaly = EllipseMechanics.MeanAnomalyToTrueAnomaly(meanAnomaly, orbit.Eccentricity);

                Quaternion rotation = Quaternion.Euler(90, 0, (math.PIHALF + trueAnomaly) * math.TODEGREES);
                const float MULTIPLIER = consts.DISTANCE_MULTIPLIER * 2 / 0.9f; // 0.9f = shader diameter
                float scale = orbit.Distance * MULTIPLIER;
                matricies[i] = Matrix4x4.TRS(orbitposition, rotation, new float3(scale));
            }

            return matricies;

        }

    }
}