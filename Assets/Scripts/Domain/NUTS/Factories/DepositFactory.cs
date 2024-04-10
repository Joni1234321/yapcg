using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.Common.Components;
using YAPCG.Engine.Common;
using YAPCG.Engine.Components;

namespace YAPCG.Domain.NUTS.Factories
{
    public struct DepositFactory : IFactory<Deposit.DepositSpawnConfig>
    {
        public void Spawn (EntityCommandBuffer ecb, DynamicBuffer<Deposit.DepositSpawnConfig> configs, ref Random random, ref NativeList<Entity> spawned)
        {
            foreach (var config in configs)
            {
                int j = spawned.Length;

                for (int i = 0; i < config.Total; i++)
                {
                    Deposit.RGO rgo = (Deposit.RGO)random.NextInt((int)Deposit.RGO.COUNT);
                    FixedString64Bytes name = NamingGenerator.Get(ref random);
                    spawned.Add(CreateDeposit(ecb, rgo, name));
                }

                for (int i = 0; i < config.Small; i++, j++)
                    ToSmall(ecb, spawned[j]);

                for (int i = 0; i < config.Medium; i++, j++)
                    ToMedium(ecb, spawned[j]);

                for (int i = 0; i < config.Big; i++, j++)
                    ToBig(ecb, spawned[j]);

            }
        }        
        
        private Entity CreateDeposit(EntityCommandBuffer _, Deposit.RGO rgo, FixedString64Bytes name = default)
        {
            Entity e = _.CreateEntity();
            _.AddComponent<Deposit.Tag>(e);
            
            // Name
            _.AddComponent(e, new Name { Value = name });
            _.SetName(e, $"DEPOSIT: {name}");
    
            // Components
            _.AddComponent(e, new DiscoverProgress { Progress = 1, Value = 0, MaxValue = 20});            
            _.AddComponent(e, new Deposit.Reserves { Value = 0});
            _.AddComponent(e, new Deposit.RGOType { RGO = rgo });
            _.AddComponent<Deposit.Sizes>(e);
            
            return e;
        }

        private void ToBig(EntityCommandBuffer _, Entity e)
        {
            _.SetComponent(e, new Deposit.Sizes { S = 20, M = 10, L = 4 });
        }
        
        private void ToMedium(EntityCommandBuffer _, Entity e)
        {
            _.SetComponent(e, new Deposit.Sizes { S = 40, M = 20 });
        }

        private void ToSmall(EntityCommandBuffer _, Entity e)
        {
            _.SetComponent(e, new Deposit.Sizes { S = 100 });
        }

        public Entity CreateBigDeposit(EntityCommandBuffer _, Deposit.RGO rgo, FixedString64Bytes name = default)
        {
            Entity e = CreateDeposit(_, rgo, name);
            

            
            /*
            var buffer = _.AddBuffer<Deposit.Sizes>(e);
            buffer.Add(new Deposit.Sizes { Size = 20 });
            buffer.Add(new Deposit.Sizes { Size = 10 });
            buffer.Add(new Deposit.Sizes { Size =  4 });

            Deposit.Levels levels = new Deposit.Levels();
            unsafe
            {
                levels.Values[0] = 20;
                levels.Values[1] = 10;
                levels.Values[2] =  4;
            }
            _.AddComponent(e, levels);

            Deposit.L2 l2 = new Deposit.L2
            {
                Values = new NativeArray<byte>(8, Allocator.Persistent)
            };
            l2.Values[0] = 20;
            l2.Values[1] = 10;
            l2.Values[2] =  4;
            _.AddComponent(e, l2);

            Deposit.L3 l3 = new Deposit.L3
            {
                Bytes = new NativeArray<byte>(8, Allocator.Persistent),
                Long = 0x02040710,
            };
            _.AddComponent(e, l3);*/
            
            return e;
        }
        
   
    }
}