using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using YAPCG.Engine.Input;
using YAPCG.Engine.SystemGroups;
using YAPCG.Planets.Components;
using YAPCG.UI.Components;
using static Unity.Collections.Allocator;

namespace YAPCG.UI
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    public partial struct ActionUISystem : ISystem
    {
        private EntityQuery _hubsQuery;
        public void OnCreate(ref SystemState state)
        {
            _hubsQuery = SystemAPI.QueryBuilder().WithAll<HubTag>().Build();

            state.EntityManager.CreateSingleton(new FocusedHub { Entity = Entity.Null });

            state.RequireForUpdate<ActionInput>(); 
            state.RequireForUpdate<FocusedHub>();
        }

        private int NegativeMod(int x, int m) => (x % m + m) % m;
        
        public void OnUpdate(ref SystemState state)
        {
            NativeArray<Entity> hubs = _hubsQuery.ToEntityArray(Temp);
            int focusedIndex = SystemAPI.GetSingleton<FocusedHub>().Entity.Index;
            
            ActionInput action = SystemAPI.GetSingleton<ActionInput>();

            if (action.ShouldBuildHub)
            {
                // Update UI
                
            }

            if (action.Next)
                focusedIndex++;
            if (action.Previous)
                focusedIndex--;

            if (hubs.Length != 0)
            {
                Entity focused = hubs[NegativeMod(focusedIndex, hubs.Length)];

                HUD.Instance.UpdateHubUI(state.EntityManager, focused);

                SystemAPI.SetSingleton(new FocusedHub() {Entity = focused} );
            }
        }
    }
}