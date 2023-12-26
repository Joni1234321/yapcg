using Unity.Burst;
using Unity.Entities;
using YAPCG.Engine.Input;
using YAPCG.Engine.SystemGroups;

namespace YAPCG.UI
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    public partial struct ActionUISystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ActionInput action = SystemAPI.GetSingleton<ActionInput>();


            if (action.ShouldBuildHub)
            {
                // Update UI
                
            }

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}