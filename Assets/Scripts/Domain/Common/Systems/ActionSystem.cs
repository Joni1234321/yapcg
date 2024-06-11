using Unity.Burst;
using Unity.Entities;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Input.Systems;

namespace YAPCG.Domain.Common.Systems
{
    [UpdateInGroup(typeof(InputSystemGroup))]
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

            if (actionClaim.ValueRW.Body != Entity.Null)
            {
                SystemAPI.GetComponentRW<Body.Owner>(actionClaim.ValueRW.Body).ValueRW.ID = actionClaim.ValueRW.OwnerID;
                actionClaim.ValueRW.Body = Entity.Null;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}