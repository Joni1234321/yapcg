using Unity.Burst;
using Unity.Entities;
using Unity.Entities.Content;
using UnityEngine;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Engine
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SharedInitSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.CreateSingleton(new SharedRandom { Random = Random.CreateFromIndex(29) });
            state.EntityManager.CreateSingleton(new SharedCameraManaged { MainCamera = Camera.main });
            CLogger.LogLoaded(null, "SharedSystemInit");
            state.Enabled = false;
        }
    }
}