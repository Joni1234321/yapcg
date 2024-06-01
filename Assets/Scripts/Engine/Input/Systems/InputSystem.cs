using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Engine.Physics.Collisions;
using YAPCG.Engine.SystemGroups;
using static UnityEngine.Input;

namespace YAPCG.Engine.Input.Systems
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    internal partial struct InputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.CreateSingleton<MouseInput>();
            state.EntityManager.CreateSingleton<ActionInput>();
            state.EntityManager.CreateSingleton<SharedRays>();
        }

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
                    LeftClickSelectBody = GetMouseButtonDown(0) // for now should change to detect if ui
                }
            );

            SetCameraRay();
        }

        [BurstDiscard]
        void SetCameraRay()
        {
            Ray ray = SystemAPI.ManagedAPI.GetSingleton<SharedCameraManaged>().MainCamera.ScreenPointToRay(mousePosition);
            SystemAPI.SetSingleton(new SharedRays
            {
                CameraMouseRay = new Raycast.ray {origin = ray.origin, direction = ray.direction }
            });
        }
    }
}