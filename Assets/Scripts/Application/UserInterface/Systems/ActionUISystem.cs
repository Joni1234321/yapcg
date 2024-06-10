using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using YAPCG.Domain.Common.Components;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Components;
using YAPCG.Engine.DebugDrawer;
using YAPCG.Engine.Input;
using YAPCG.Engine.Physics;
using YAPCG.Engine.Physics.Collisions;
using YAPCG.Engine.SystemGroups;
using YAPCG.Resources.View.Custom.Util;
using YAPCG.UI.Components;
using static Unity.Collections.Allocator;
using Position = YAPCG.Engine.Components.Position;

namespace YAPCG.Application.UserInterface.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    public partial struct ActionUISystem : ISystem
    {
        private EntityQuery _bodyQuery, _levelQuery;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SharedSizes>();
            state.RequireForUpdate<SharedRays>();
            _bodyQuery = SystemAPI.QueryBuilder().WithAll<Body.BodyTag, Position, ScaleComponent, DiscoverProgress, Name>().Build();
            _levelQuery = SystemAPI.QueryBuilder().WithAll<LevelQuad>().Build();
            
            state.EntityManager.CreateSingleton(new FocusedBody { Selected = Entity.Null });

            state.RequireForUpdate<ActionInput>(); 
            state.RequireForUpdate<FocusedBody>();
        }

        private int NegativeMod(int x, int m) => (x % m + m) % m;

        private int BodyEntityToIndex(Entity body, NativeArray<Entity> bodies)
        {
            for (int i = 0; i < bodies.Length; i++)
                if (bodies[i] == body)
                    return i;
            return -1;
        }
        
        public void OnUpdate(ref SystemState state)
        {
            ActionInput action = SystemAPI.GetSingleton<ActionInput>();

            Raycast.ray ray = SystemAPI.GetSingleton<SharedRays>().CameraMouseRay;
            
            if (action.ShouldBuildHub)
                BuildBodyOnMouse(ray);

            NativeArray<Entity> bodies = _bodyQuery.ToEntityArray(state.WorldUpdateAllocator);

            int hoveredIndex = GetHoverBodyIndex(ray);
            Entity hovered = hoveredIndex != -1 ? bodies[hoveredIndex] : Entity.Null;

            RefRW<FocusedBody> focusedBody = SystemAPI.GetSingletonRW<FocusedBody>();
            Entity selected = focusedBody.ValueRO.Selected;
            int selectedIndex = BodyEntityToIndex(selected, bodies);

            if (action.Next)
                selectedIndex = NegativeMod(selectedIndex + 1, bodies.Length);

            if (action.Previous)
                selectedIndex = NegativeMod(selectedIndex - 1, bodies.Length);

            if (bodies.Length != 0 && selectedIndex > 0 && selectedIndex < bodies.Length)
                selected = bodies[selectedIndex];
            
            
            if (action.LeftClickSelectBody && hovered != Entity.Null)
                selected = hovered;
            
            Debug.Log(EventSystem.current.IsPointerOverGameObject());

            if (action.DeselectBody)
                selected = Entity.Null;
            
            // Hovered
            if (focusedBody.ValueRO.Hovered != Entity.Null) 
                SystemAPI.SetComponent(focusedBody.ValueRO.Hovered, AlternativeColorRatio.Nothing);

            if (hovered != Entity.Null)
                SystemAPI.SetComponent(hovered, AlternativeColorRatio.Hovered);
            
            focusedBody.ValueRW.Hovered = hovered;

            // Selected
            if (focusedBody.ValueRO.Selected != Entity.Null) 
                SystemAPI.SetComponent(focusedBody.ValueRO.Selected, AlternativeColorRatio.Nothing);

            if (selected != Entity.Null)
               SystemAPI.SetComponent(selected, AlternativeColorRatio.Selected);

            NativeArray<float3> positions = _bodyQuery.ToComponentDataArray<Position>(state.WorldUpdateAllocator).Reinterpret<Position, float3>();
            NativeArray<FixedString64Bytes> names = _bodyQuery.ToComponentDataArray<Name>(state.WorldUpdateAllocator).Reinterpret<Name, FixedString64Bytes>();
            NativeArray<DiscoverProgress> discoveryProgress = _bodyQuery.ToComponentDataArray<DiscoverProgress>(state.WorldUpdateAllocator);
            NativeArray<StyleClasses.BorderColor> borderColors = CollectionHelper.CreateNativeArray<StyleClasses.BorderColor>(names.Length, state.WorldUpdateAllocator);
            
            for (int i = 0; i < borderColors.Length; i++)
                borderColors[i] = discoveryProgress[i].Progress == 0 ? StyleClasses.BorderColor.Impossible : StyleClasses.BorderColor.Valid;
            
            HUD.Instance.UpdateBodyUI(state.EntityManager, selected);
            HUD.Instance.WorldHUD.DrawPlanetNames(names, positions, borderColors, selectedIndex);
            //HUD.Instance.UpdateHubUI(state.EntityManager, selected);
            
            focusedBody.ValueRW.Selected = selected;

        }

        
        [BurstCompile]
        int GetHoverBodyIndex(Raycast.ray ray)
        {
            NativeArray<float3> positions = _bodyQuery.ToComponentDataArray<Position>(Temp).Reinterpret<Position, float3>();
            NativeArray<float> sizes = _bodyQuery.ToComponentDataArray<ScaleComponent>(Temp).Reinterpret<ScaleComponent, float>();
            SphereCollection spheres = new SphereCollection
            {
                Positions = positions, 
                Radius = sizes
            };
            
            if (!RaySphere.CheckCollision(ray, spheres, out Raycast.hit hit))
                return -1;

            return hit.index;
        }

        
        [BurstCompile]
        bool BuildBodyOnMouse(Raycast.ray ray)
        {
            TriangleCollection triangles = new TriangleCollection { Positions = GetLevelTriangles() };

            if (!RayTriangle.CheckCollision(ray, triangles, out Raycast.hit hit))
                return false;
            
            DebugDrawer.DrawTriangles(triangles);
            DebugDrawer.DrawRaycastHit(ray, hit);
 
            SystemAPI.GetSingletonBuffer<Hub.HubFactoryParams>(false).Add(new Hub.HubFactoryParams
            {
                Position = hit.point,  
                Size = Hub.Size.Medium,
            });
            
            //CLogger.LogInfo("Building body");
            return true;
        }
        
       
        
        [BurstCompile]
        NativeArray<float3> GetLevelTriangles()
        {
            NativeArray<LevelQuad> quads = _levelQuery.ToComponentDataArray<LevelQuad>(Temp);
            NativeArray<float3> positions = new NativeArray<float3>(6, Temp);
            for(int i = 0; i < quads.Length; i++)
            {
                LevelQuad quad = quads[i];
                int o = i * 6;
                
                float x0 = quad.Position.x - quad.Size.x;
                float x1 = quad.Position.x + quad.Size.x;
                float y0 = quad.Position.y - quad.Size.y;
                float y1 = quad.Position.y + quad.Size.y;
                
                float3 v00 = new float3(x0, 0, y0);
                float3 v01 = new float3(x0, 0, y1);
                float3 v11 = new float3(x1, 0, y1);
                float3 v10 = new float3(x1, 0, y0);

                positions[o + 0] = v00;
                positions[o + 1] = v11;
                positions[o + 2] = v10;
                
                positions[o + 3] = v00;
                positions[o + 4] = v01;
                positions[o + 5] = v11;
            }

            return positions;
        }

        


    }
}