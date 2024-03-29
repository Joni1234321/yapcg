﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Engine.Components;
using YAPCG.Engine.Time.Systems;
using YAPCG.Hub.Factories;
using YAPCG.Planets.Components;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Hub.Systems
{
    [UpdateInGroup(typeof(TickDailyGroup))]
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
            state.EntityManager.CreateSingletonBuffer<HubSpawnConfig>();

            SystemAPI.GetSingletonBuffer<HubSpawnConfig>(false).Add(new HubSpawnConfig
            {
                Position = float3.zero,
                Big = 1,
                Medium = 10,
                Small = 100000
            });
            
        }
        

        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Random random = SystemAPI.GetSingleton<SharedRandom>().Random;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var spawnConfigs = SystemAPI.GetSingletonBuffer<HubSpawnConfig>(false);
            foreach (HubSpawnConfig config in spawnConfigs)
            {
                NativeList<Entity> spawnedEntities = new NativeList<Entity>(config.Total, Allocator.Temp);

                for (int i = 0; i < config.Small; i++)
                    spawnedEntities.Add(
                        HubFactory.CreateSmallHub(
                            ecb, 
                            config.Position + GetPositionInCircle(ref random, 200), 
                            HubNamingGenerator.Get(ref random)
                            )
                        );

                for (int i = 0; i < config.Medium; i++)
                    spawnedEntities.Add(
                        HubFactory.CreateNormalHub(
                            ecb, 
                            config.Position + GetPositionInCircle(ref random, 50), 
                            HubNamingGenerator.Get(ref random)
                        )
                    );
                
                for (int i = 0; i < config.Big; i++)
                    spawnedEntities.Add(
                        HubFactory.CreateBigHub(
                            ecb, 
                            config.Position, 
                            HubNamingGenerator.Get(ref random)
                        )
                    );

                foreach (Entity e in spawnedEntities)
                {
                    ecb.SetComponent<DiscoverProgress>(e, new DiscoverProgress { Value = random.NextInt(0, 40), Progress = 1, MaxValue = 40} );
                }
            }
            
            
            
            spawnConfigs.Clear();
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            SystemAPI.SetSingleton(new SharedRandom { Random = random });
        }
    }

    [InternalBufferCapacity(0)]
    public struct HubSpawnConfig : IBufferElementData
    {
        public float3 Position;
        public int Big, Medium, Small;

        public int Total => Big + Medium + Small;
    }
    
}