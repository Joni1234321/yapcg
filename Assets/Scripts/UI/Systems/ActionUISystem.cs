using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Engine.Input;
using YAPCG.Engine.Physics;
using YAPCG.Engine.Render.Systems;
using YAPCG.Hub.Systems;
using YAPCG.Planets.Components;
using YAPCG.UI.Components;
using static Unity.Collections.Allocator;
using YAPCG.Engine.Physics.Collisions;
using YAPCG.Planet;

namespace YAPCG.UI.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    public partial struct ActionUISystem : ISystem
    {
        private EntityQuery _hubsQuery, _levelQuery;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SharedSizes>();
            state.RequireForUpdate<SharedRays>();
            _hubsQuery = SystemAPI.QueryBuilder().WithAll<HubTag, Position>().Build();
            _levelQuery = SystemAPI.QueryBuilder().WithAll<LevelQuad>().Build();
            
            state.EntityManager.CreateSingleton(new FocusedHub { Selected = Entity.Null });

            state.RequireForUpdate<ActionInput>(); 
            state.RequireForUpdate<FocusedHub>();
        }

        private int NegativeMod(int x, int m) => (x % m + m) % m;

        private Entity GetAdjacentHub (Entity currentHub, int distance)
        {

            NativeArray<Entity> hubs = _hubsQuery.ToEntityArray(Temp);

            if (hubs.Length == 0) return Entity.Null;
                
            int i;
            for (i = 0; i < hubs.Length; i++)
                if (hubs[i] == currentHub)
                    break;

            int adjacentEntityIndex = NegativeMod(i + distance, hubs.Length);
            Entity adjacentHub = hubs[adjacentEntityIndex];
            hubs.Dispose();
            
            return adjacentHub;
        }
        public void OnUpdate(ref SystemState state)
        {
            ActionInput action = SystemAPI.GetSingleton<ActionInput>();
            
            if (action.ShouldBuildHub)
                BuildHub();

            RefRW<FocusedHub> focusedHub = SystemAPI.GetSingletonRW<FocusedHub>();
            Entity selected = focusedHub.ValueRO.Selected;
            Entity hovered = GetHoverHub();

            
            if (action.Next)
                selected = GetAdjacentHub(selected, 1);

            if (action.Previous)
                selected = GetAdjacentHub(selected, -1);
            
            if (action.LeftClickSelectHub)
                // if (hovered != Entity.Null)
                    selected = hovered;

            // Hovering and select effects
            // if (hovered != focusedHub.ValueRO.Hovered)
            {
                if (focusedHub.ValueRO.Hovered != Entity.Null) 
                    SystemAPI.SetComponent(focusedHub.ValueRO.Hovered, StateComponent.Nothing);
                
                if (hovered != Entity.Null)
                    SystemAPI.SetComponent(hovered, StateComponent.Hovered);
                
                focusedHub.ValueRW.Hovered = hovered;
            }
            // if (selected != focusedHub.ValueRO.Selected)
            {
                if (focusedHub.ValueRO.Selected != Entity.Null) 
                    SystemAPI.SetComponent(focusedHub.ValueRO.Selected, StateComponent.Nothing);

                if (selected != Entity.Null)
                   SystemAPI.SetComponent(selected, StateComponent.Selected);

                HUD.Instance.UpdateHubUI(state.EntityManager, selected);
                focusedHub.ValueRW.Selected = selected;

            }

        }

        [BurstCompile]
        void BuildHub()
        {
            SystemAPI.GetSingletonBuffer<HubSpawnConfig>(false).Add(new HubSpawnConfig
            {
                Position = new float3(1,1,1),  
                Big = 0, 
                Medium = 1, 
                Small = 0
            });
        }

        
        [BurstCompile]
        NativeArray<float3> GetHubPositions () => _hubsQuery.ToComponentDataArray<Position>(Temp).Reinterpret<Position, float3>();

        [BurstCompile]
        NativeArray<float3> GetQuadTriangles()
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

                positions[o + 0] = v11;
                positions[o + 1] = v10;
                positions[o + 2] = v00;
                
                positions[o + 3] = v00;
                positions[o + 4] = v11;
                positions[o + 5] = v01;
            }

            return positions;
        }

        [BurstCompile]
        Entity GetHoverHub()
        {
            Raycast.ray ray = SystemAPI.GetSingleton<SharedRays>().CameraMouseRay;
            Raycast.hit hit;
            NativeArray<float3> positions = GetHubPositions();
            SharedSizes sizes = SystemAPI.GetSingleton<SharedSizes>();
            SphereCollection spheres = new SphereCollection
            {
                Positions = positions, 
                Radius = sizes.HubRadius
            };
            
            TriangleCollection triangles = new TriangleCollection { Positions = GetQuadTriangles() };
            DrawTrianglesDebug(triangles);
            
            if (!Raycast.CollisionTriangle(ray, triangles, out hit))
                Debug.Log("no hit");
            else 
                Debug.Log("hit a triangle");
            
            
            if (!Raycast.CollisionSphere(ray, spheres, out hit))
                return Entity.Null;

            return _hubsQuery.ToEntityArray(Temp)[hit.index];
        }

        [BurstDiscard]
        void DrawTrianglesDebug(TriangleCollection triangles)
        {
            int f = 2;
            int n = triangles.Positions.Length / 3;
            for (int i = 0; i < n; i++)
            {
                float3 v1 = triangles.Positions[i * 3 + 0];
                float3 v2 = triangles.Positions[i * 3 + 1];
                float3 v3 = triangles.Positions[i * 3 + 2];
                Debug.DrawLine(v1, v2, Color.green, 1);
                Debug.DrawLine(v2, v3, Color.yellow, 1);
                Debug.DrawLine(v3, v1, Color.red, 1);
            }
        }
    }
}