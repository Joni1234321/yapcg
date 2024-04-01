using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Engine.Input;
using YAPCG.Engine.Physics;
using YAPCG.Engine.Physics.Ray;
using YAPCG.Engine.Render.Systems;
using YAPCG.Hub.Systems;
using YAPCG.Planets.Components;
using YAPCG.UI.Components;
using static Unity.Collections.Allocator;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Content;

namespace YAPCG.UI.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    public partial struct ActionUISystem : ISystem
    {
        private EntityQuery _hubsQuery;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SharedRays>();
            _hubsQuery = SystemAPI.QueryBuilder().WithAll<HubTag, Position>().Build();
            
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
                if (hovered != Entity.Null)
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

                focusedHub.ValueRW.Selected = selected;
                
                if (selected != focusedHub.ValueRO.Selected)
                    HUD.Instance.UpdateHubUI(state.EntityManager, selected);
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
        Entity GetHoverHub()
        {
            Ray ray = SystemAPI.GetSingleton<SharedRays>().CameraMouseRay;
            SphereCollection spheres = new SphereCollection
            {
                Positions = _hubsQuery.ToComponentDataArray<Position>(Temp).Reinterpret<Position, float3>(), 
                Radius = 2
            };
            Raycast.Hit hit = Raycast.CollisionSphere(ray.origin, ray.direction, spheres);
            
            if (hit.hit == -1) 
                return Entity.Null;

            return _hubsQuery.ToEntityArray(Temp)[hit.hit];
        }
    }
}