using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Application.UserInterface;
using YAPCG.Engine.Components;
using YAPCG.Engine.Input;
using YAPCG.Engine.Input.Systems;
using YAPCG.Engine.Physics.Collisions;
using static UnityEngine.Input;

namespace YAPCG.Application.Input
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
            
            float2 mousePosition = new float2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y);
            bool mouseOverUI = HUD.Instance.IsOverUI(mousePosition);
            
            SystemAPI.SetSingleton(new ActionInput
                {
                    ShouldBuildHub = GetKeyDown(KeyCode.A),
                    Next = GetKeyDown(KeyCode.Z),
                    Previous = GetKeyDown(KeyCode.X),
                    LeftClickSelectBody = GetMouseButtonDown(0) && !mouseOverUI, // for now should change to detect if ui
                    DeselectBody = GetMouseButtonDown(1) && !mouseOverUI
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