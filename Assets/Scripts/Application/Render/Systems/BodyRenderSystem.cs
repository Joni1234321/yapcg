using System;
using System.Runtime.InteropServices;
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

namespace YAPCG.Application.Render.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    [BurstCompile]
    internal partial class BodyRenderSystem : SystemBase
    {
        private EntityQuery _planetQuery, _sunQuery;
        private RenderUtils.PositionScaleAlternativeMaterialRender _planetRenderer, _sunRenderer;
        [BurstCompile]
        protected override void OnCreate()
        {
            _planetQuery = SystemAPI.QueryBuilder().WithAll<Position, ScaleComponent, FadeStartTimeComponent, AlternativeColorRatio, Body.PlanetTag>().Build();
            _sunQuery = SystemAPI.QueryBuilder().WithAll<Position, ScaleComponent, FadeStartTimeComponent, AlternativeColorRatio, Body.SunTag>().Build();
            RequireForUpdate<MeshesSingleton>();
            _planetRenderer = new RenderUtils.PositionScaleAlternativeMaterialRender();
            _sunRenderer = new RenderUtils.PositionScaleAlternativeMaterialRender();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            // OPTIMIZATION: SET COMPONENT FLAG THAT WHENEVER ANOTHER OBJECT SPAWNS, THEN CHANGE COMMAND, OTHERWISE DONT
            RenderPlanets();
            RenderSuns();
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
    }
}