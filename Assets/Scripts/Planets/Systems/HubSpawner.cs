using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Planets.Components;
using YAPCG.Planets.Factories;
using YAPCG.Time.Systems;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Planets.Systems
{
    [UpdateInGroup(typeof(TickWeeklyGroup))]
    public partial struct HubSpawner : ISystem  
    {
        float3 GetPositionInCircle(Random random, float radius)
        {
            return random.NextFloat3(new float3(-radius, -radius, 0), new float3(radius, radius, 0));
        }
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SharedRandom>();
            Debug.Log("Hub");
           /* Random random = SystemAPI.GetSingleton<SharedRandom>().Random;
            
            for (int i = 0; i < 5; i++)
                HubFactory.CreateSmallHub(state.EntityManager, GetPositionInCircle(random, 1));
            for (int i = 0; i < 2; i++)
                HubFactory.CreateNormalHub(state.EntityManager, GetPositionInCircle(random, 1));
            for (int i = 0; i < 1; i++)
                HubFactory.CreateBigHub(state.EntityManager, float3.zero);
*/
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}