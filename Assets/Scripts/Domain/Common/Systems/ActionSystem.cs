using Unity.Burst;
using Unity.Entities;
using YAPCG.Domain.Common.Components;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Time.Systems;

namespace YAPCG.Domain.Common.Systems
{
    [UpdateInGroup(typeof(TickDailyGroup))]
    public partial struct ActionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Body.ActionClaim>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            RefRW<Body.ActionClaim> actionClaim = SystemAPI.GetSingletonRW<Body.ActionClaim>();

            Entity e = actionClaim.ValueRW.Body;
            if (e != Entity.Null)
            {
                SystemAPI.GetComponentRW<Body.Owner>(e).ValueRW.ID = actionClaim.ValueRW.OwnerID;
                SystemAPI.GetComponentRW<DiscoverProgress>(e).ValueRW.Progress++;
                RefRW<LandDiscovery> landDiscovery = SystemAPI.GetComponentRW<LandDiscovery>(e);
                landDiscovery.ValueRW.OrbitThroughput  += 1;
                landDiscovery.ValueRW.ProbesThroughput += 2;
                landDiscovery.ValueRW.PeopleThroughput += 4;

                actionClaim.ValueRW.Body = Entity.Null;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}