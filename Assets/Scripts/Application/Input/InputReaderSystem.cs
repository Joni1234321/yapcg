﻿using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using YAPCG.Application.UserInterface;
using YAPCG.Engine.Components;
using YAPCG.Engine.Input;
using YAPCG.Engine.Physics.Collisions;

namespace YAPCG.Application.Input
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(InputSystemGroup), OrderFirst = true)]
    internal partial struct InputReaderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.CreateSingleton<ActionInput>();
            state.EntityManager.CreateSingleton<SharedRays>();
            state.RequireForUpdate<HUD.HUDReady>();
        }

        public void OnUpdate(ref SystemState state)
        {
            bool mouseOverUI = HUD.Instance.IsOverUserInterface(Mouse.current.position.value);
            
            SystemAPI.SetSingleton(new ActionInput
                {
                    ShouldBuildHub = Keyboard.current[Key.A].wasPressedThisFrame,
                    Next = Keyboard.current[Key.Z].wasPressedThisFrame,
                    Previous = Keyboard.current[Key.X].wasPressedThisFrame,
                    LeftClickSelectBody = Mouse.current.leftButton.wasPressedThisFrame && !mouseOverUI, // for now should change to detect if ui
                    DeselectBody = Mouse.current.rightButton.wasPressedThisFrame && !mouseOverUI,
                    SpeedPause = Keyboard.current[Key.P].wasPressedThisFrame,
                    SpeedIncrease = Keyboard.current[Key.NumpadPlus].wasPressedThisFrame,
                    SpeedDecrease = Keyboard.current[Key.NumpadMinus].wasPressedThisFrame,
                    Zoom = Mouse.current.scroll.ReadValue().y,
                    ResetView = Keyboard.current[Key.F].wasPressedThisFrame,
                }
            );

            SetCameraRay();
        }

        void SetCameraRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);
            SystemAPI.SetSingleton(new SharedRays
            {
                CameraMouseRay = new Raycast.ray {origin = ray.origin, direction = ray.direction }
            });
        }
    }
}