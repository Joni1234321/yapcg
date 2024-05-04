using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.Common.Components;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Domain.NUTS.Factories
{
    public struct HubFactory : IFactory<Hub.HubSpawnConfig>
    {
        private static float3 GetPositionOnSquare(ref Random random, float radius) => random.NextFloat3(new float3(-radius, 0, -radius), new float3(radius, 0, radius));

        public void Spawn (EntityCommandBuffer ecb, DynamicBuffer<Hub.HubSpawnConfig> configs, ref Random random, ref NativeList<Entity> spawned)
        {
            foreach (Hub.HubSpawnConfig config in configs)
            {
                Deposit.RGO rgo = (Deposit.RGO)random.NextInt((int)Deposit.RGO.COUNT);
                FixedString64Bytes name = NamingGenerator.Get(ref random);
                
                Entity e = CreateHub(ecb, rgo, config.Position, name);
                
                switch (config.Size)
                {
                    case Hub.Size.Small:
                        ToSmall(ecb, e);
                        break;
                    case Hub.Size.Medium:
                        ToMedium(ecb, e);
                        break;
                    case Hub.Size.Big:
                        ToBig(ecb, e);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                spawned.Add(e);
            }
        }        
        
        private Entity CreateHub(EntityCommandBuffer _, Deposit.RGO rgo, float3 position, FixedString64Bytes name = default)
        {
            Entity e = _.CreateEntity();
            _.AddComponent<Hub.HubTag>(e);

            // Name
            _.AddComponent(e, new Name { Value = name});
            _.SetName(e, $"HUB: {name}");
            
            // Components
            _.AddComponent(e, new Position { Value = position });
            _.AddComponent(e, new DiscoverProgress { Progress = 1, Value = 0, MaxValue = 20 });
            _.AddComponent<BuildingSlotsLeft>(e);

            // Anim
            _.AddComponent(e, new AnimationComponent { AnimationStart = float.MinValue } );
            _.AddComponent(e, new AnimationStateComponent { AnimationState = 0 } );
            

            return e;
        }

        private void Assemble(EntityCommandBuffer _, Entity e, BuildingSlotsLeft buildingSlotsLeft)
        {
            _.SetComponent(e, buildingSlotsLeft);
        }

        private void ToSmall(EntityCommandBuffer _, Entity e) =>
            Assemble(_, e,
                new BuildingSlotsLeft { Large = 2, Medium = 5, Small = 25 }
            );
        
        private void ToMedium(EntityCommandBuffer _, Entity e) =>
            Assemble(_, e,
                new BuildingSlotsLeft { Large = 5, Medium = 10, Small = 10 }
            );
        
        private void ToBig(EntityCommandBuffer _, Entity e) =>
            Assemble(_, e,
                new BuildingSlotsLeft { Large = 10, Medium = 5, Small = 5 }
            );
    }
}