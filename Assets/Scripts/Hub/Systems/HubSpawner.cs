using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Engine.Components;
using YAPCG.Hub.Factories;
using YAPCG.UI;
using Random = Unity.Mathematics.Random;
using TickWeeklyGroup = YAPCG.Engine.Time.Systems.TickWeeklyGroup;

namespace YAPCG.Hub.Systems
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
            state.EntityManager.CreateSingletonBuffer<HubSpawnConfig>();
            
            /*
            SystemAPI.GetSingletonBuffer<HubSpawnConfig>(false).Add(new HubSpawnConfig
                {
                    Position = float3.zero,  
                    Big = 1, 
                    Medium = 2, 
                    Small = 5
                });*/
            SystemAPI.GetSingletonBuffer<HubSpawnConfig>(false).Add(new HubSpawnConfig
            {
                Position = float3.zero,
                Big = 1
            });
        }
        
        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            Random random = SystemAPI.GetSingleton<SharedRandom>().Random;

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var spawnConfigs = SystemAPI.GetSingletonBuffer<HubSpawnConfig>(false);
            
            foreach (var config in spawnConfigs)
            {
                for (int i = 0; i < config.Small; i++)
                    HubFactory.CreateSmallHub(ecb, config.Position + GetPositionInCircle(ref random, 5), HubNamingGenerator.Get(ref random));

                for (int i = 0; i < config.Medium; i++)
                    HubFactory.CreateNormalHub(ecb, config.Position + GetPositionInCircle(ref random, 2), HubNamingGenerator.Get(ref random));

                for (int i = 0; i < config.Big; i++)    
                    HubFactory.CreateBigHub(ecb, config.Position + float3.zero, HubNamingGenerator.Get(ref random));
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
    }
    
}