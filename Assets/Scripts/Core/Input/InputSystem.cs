using Unity.Burst;
using Unity.Entities;

namespace YAPCG.Core.Input
{
    public partial struct InputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.CreateSingleton(new MouseInput());
            state.EntityManager.CreateSingleton(new KeyInput());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SystemAPI.SetSingleton(new MouseInput() {});
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }


    public struct MouseInput : IComponentData
    {
        
    }

    public struct KeyInput : IComponentData
    {
        
    }
}