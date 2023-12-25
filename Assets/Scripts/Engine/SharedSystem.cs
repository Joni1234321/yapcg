using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using YAPCG.Engine.Components;
using Random = Unity.Mathematics.Random;
using TickWeeklyGroup = YAPCG.Engine.Time.Systems.TickWeeklyGroup;

namespace YAPCG.Engine
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SharedSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.CreateSingleton(new SharedRandom { Random = Random.CreateFromIndex(29) });

            Debug.Log("Shared");

            state.Enabled = false;
        }
    }
}