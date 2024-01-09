using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Engine.Components;
using YAPCG.Engine.Input;
using YAPCG.Engine.SystemGroups;
using YAPCG.Hub.Systems;
using YAPCG.Planets.Components;
using YAPCG.UI.Components;
using static Unity.Collections.Allocator;

namespace YAPCG.UI.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    public partial struct ActionUISystem : ISystem
    {
        private EntityQuery _hubsQuery;
        private ComponentLookup<HubTag> _hubLookup;
        public void OnCreate(ref SystemState state)
        {
            _hubsQuery = SystemAPI.QueryBuilder().WithAll<HubTag>().Build();
            
            state.EntityManager.CreateSingleton(new FocusedHub { Entity = Entity.Null });

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
            _hubLookup = SystemAPI.GetComponentLookup<HubTag>(true);
            ActionInput action = SystemAPI.GetSingleton<ActionInput>();
            if (action.ShouldBuildHub)
            {
                SystemAPI.GetSingletonBuffer<HubSpawnConfig>(false).Add(new HubSpawnConfig
                {
                    Position = new float3(1,1,1),  
                    Big = 0, 
                    Medium = 1, 
                    Small = 0
                });
            }

            Entity focusedEntity = SystemAPI.GetSingleton<FocusedHub>().Entity;

            if (action.Next)
                focusedEntity = GetAdjacentHub(focusedEntity, 1);

            if (action.Previous)
                focusedEntity = GetAdjacentHub(focusedEntity, -1);
            
            if (focusedEntity != Entity.Null)
            {
                HUD.Instance.UpdateHubUI(state.EntityManager, focusedEntity);
                SystemAPI.SetSingleton(new FocusedHub {Entity = focusedEntity});
            }

        }
    }
}