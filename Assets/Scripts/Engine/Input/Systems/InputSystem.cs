using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using YAPCG.Engine.SystemGroups;
using static UnityEngine.Input;

namespace YAPCG.Engine.Input
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    internal partial struct InputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.CreateSingleton<MouseInput>();
            state.EntityManager.CreateSingleton<ActionInput>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SystemAPI.SetSingleton(new MouseInput
            {
                Left = GetMouseButton(0)
            });
            
            SystemAPI.SetSingleton(new ActionInput
                {
                    ShouldBuildHub = GetKeyDown(KeyCode.A),
                    Next = GetKeyDown(KeyCode.Z),
                    Previous = GetKeyDown(KeyCode.X),
                }
            );
         }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}