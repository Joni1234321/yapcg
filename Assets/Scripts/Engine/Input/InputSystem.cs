using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using YAPCG.Engine.SystemGroups;
using YAPCG.Planets.Systems;

namespace YAPCG.Engine.Input
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial struct InputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.CreateSingleton<MouseInput>();
            state.EntityManager.CreateSingleton<KeyInput>();
            state.EntityManager.CreateSingleton<ActionInput>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SystemAPI.SetSingleton(new MouseInput
            {
                Left = UnityEngine.Input.GetMouseButton(0)
            });
            
            SystemAPI.SetSingleton(new KeyInput
            {
                A = UnityEngine.Input.GetKeyDown(KeyCode.A)
            });
            
            SystemAPI.SetSingleton(new ActionInput
                {
                    ShouldBuildHub = UnityEngine.Input.GetKeyDown(KeyCode.Z)
                }

            );
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }


    public struct MouseInput : IComponentData
    {
        public bool Left;
    }

    public struct ActionInput : IComponentData
    {
        public bool ShouldBuildHub;
    }
    public struct KeyInput : IComponentData
    {
        public bool A, B, C, D, E, F;
    }
}