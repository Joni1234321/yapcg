using Unity.Burst;
using Unity.Entities;
using YAPCG.Engine.Render.Systems;

namespace YAPCG.Application.UserInterface.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    public partial struct UserInterfaceSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}