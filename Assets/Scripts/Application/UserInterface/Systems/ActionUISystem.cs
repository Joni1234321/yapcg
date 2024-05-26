using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;
using YAPCG.Engine.DebugDrawer;
using YAPCG.Engine.Input;
using YAPCG.Engine.Physics;
using YAPCG.Engine.Physics.Collisions;
using YAPCG.Engine.SystemGroups;
using YAPCG.UI.Components;
using static Unity.Collections.Allocator;

namespace YAPCG.Application.UserInterface.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    public partial struct ActionUISystem : ISystem
    {
        private EntityQuery _hubsQuery, _levelQuery;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SharedSizes>();
            state.RequireForUpdate<SharedRays>();
            _hubsQuery = SystemAPI.QueryBuilder().WithAll<Domain.NUTS.Hub.HubTag, Position>().Build();
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

            Raycast.ray ray = SystemAPI.GetSingleton<SharedRays>().CameraMouseRay;
            
            if (action.ShouldBuildHub)
                BuildHubOnMouse(ray);

            RefRW<FocusedHub> focusedHub = SystemAPI.GetSingletonRW<FocusedHub>();
            Entity selected = focusedHub.ValueRO.Selected;
            Entity hovered = GetHoverHub(ray);

            
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
                    SystemAPI.SetComponent(focusedHub.ValueRO.Hovered, AnimationStateComponent.Nothing);
                
                if (hovered != Entity.Null)
                    SystemAPI.SetComponent(hovered, AnimationStateComponent.Hovered);
                
                focusedHub.ValueRW.Hovered = hovered;
            }
            // if (selected != focusedHub.ValueRO.Selected)
            {
                if (focusedHub.ValueRO.Selected != Entity.Null) 
                    SystemAPI.SetComponent(focusedHub.ValueRO.Selected, AnimationStateComponent.Nothing);

                if (selected != Entity.Null)
                   SystemAPI.SetComponent(selected, AnimationStateComponent.Selected);

                HUD.Instance.UpdateHubUI(state.EntityManager, selected);
                focusedHub.ValueRW.Selected = selected;

            }

        }

        
        [BurstCompile]
        Entity GetHoverHub(Raycast.ray ray)
        {
            NativeArray<float3> positions = GetHubPositions();
            SharedSizes sizes = SystemAPI.GetSingleton<SharedSizes>();
            SphereCollection spheres = new SphereCollection
            {
                Positions = positions, 
                Radius = sizes.HubRadius
            };
            
            if (!RaySphere.CheckCollision(ray, spheres, out Raycast.hit hit))
                return Entity.Null;

            return _hubsQuery.ToEntityArray(Temp)[hit.index];
        }

        
        [BurstCompile]
        bool BuildHubOnMouse(Raycast.ray ray)
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
            
            //CLogger.LogInfo("Building hub");
            return true;
        }
    
        
        [BurstCompile]
        NativeArray<float3> GetHubPositions () => _hubsQuery.ToComponentDataArray<Position>(Temp).Reinterpret<Position, float3>();

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