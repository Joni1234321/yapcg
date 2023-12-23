using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Time.Systems;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Engine
{
    [UpdateInGroup(typeof(TickWeeklyGroup))]
    public partial struct SharedSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.CreateSingleton(new SharedRandom { Random = Random.CreateFromIndex(29) });

            Debug.Log("Shared");

            state.Enabled = false;
        }
    }
}