using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Planets.Factories;
using Random = Unity.Mathematics.Random;
using TickWeeklyGroup = YAPCG.Engine.Time.Systems.TickWeeklyGroup;

namespace YAPCG.Planets.Systems
{
    [UpdateInGroup(typeof(TickWeeklyGroup))]
    public partial struct HubSpawner : ISystem  
    {
        float3 GetPositionInCircle(ref Random random, float radius)
        {
            return random.NextFloat3(new float3(-radius, -radius, 0), new float3(radius, radius, 0));
        }

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SharedRandom>();

            Entity e = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(e, new HubSpawnConfig
                {
                    Position = float3.zero,  
                    Big = 1, 
                    Medium = 2, 
                    Small = 5
                });
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Random random = SystemAPI.GetSingleton<SharedRandom>().Random;

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (config, entity) in SystemAPI.Query<HubSpawnConfig>().WithEntityAccess())
            {
                for (int i = 0; i < config.Small; i++)
                {
                    Entity e = HubFactory.CreateSmallHub(ecb, config.Position + GetPositionInCircle(ref random, 5));
                    ecb.AddComponent(e, new Name { Value = HubNamingGenerator.Get(ref random)});
                }

                for (int i = 0; i < config.Medium; i++)
                {
                    float3 position = config.Position + GetPositionInCircle(ref random, 2);
                    Entity e = HubFactory.CreateNormalHub(ecb, position);
                    ecb.SetComponent(e, new Name { Value = HubNamingGenerator.Get(ref random)});
                }

                for (int i = 0; i < config.Big; i++)
                {
                    Entity e = HubFactory.CreateBigHub(ecb, config.Position + float3.zero);
                    ecb.SetComponent(e, new Name { Value = HubNamingGenerator.Get(ref random)});
                }
                ecb.DestroyEntity(entity);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            SystemAPI.SetSingleton(new SharedRandom { Random = random });
        }
    }

    public struct HubSpawnConfig : IComponentData
    {
        public float3 Position;
        public int Big, Medium, Small;
    }
    
}