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
        public void Spawn(EntityCommandBuffer ecb, Deposit.DepositSpawnConfig config, ref Random random,
            ref NativeList<Entity> spawned)
        {
            int j = spawned.Length;

            for (int i = 0; i < config.Total; i++)
            {
                Deposit.RGO rgo = (Deposit.RGO)random.NextInt((int)Deposit.RGO.COUNT);
                FixedString64Bytes name = NamingGenerator.Get(ref random);
                spawned.Add(CreateDeposit(ecb, rgo, ref random, name));
            }

            for (int i = 0; i < config.Small; i++, j++)
                ToSmall(ecb, spawned[j]);

            for (int i = 0; i < config.Medium; i++, j++)
                ToMedium(ecb, spawned[j]);

            for (int i = 0; i < config.Big; i++, j++)
                ToBig(ecb, spawned[j]);
        }        
        
        private Entity CreateDeposit(EntityCommandBuffer _, Deposit.RGO rgo, ref Random random, FixedString64Bytes name = default)
        {
            Entity e = _.CreateEntity();
            _.AddComponent<Deposit.DepositTag>(e);
            
            // Name
            _.AddComponent(e, new Name { Value = name });
            _.SetName(e, $"DEPOSIT: {name}");
    
            // Components
            _.AddComponent(e, new DiscoverProgress { Progress = 1, Value = 0, MaxValue = 20});            
            _.AddComponent(e, new Deposit.Reserves { Value = 0});
            _.AddComponent(e, new Deposit.RGOType { RGO = rgo });

            // Size dependent
            _.AddComponent<Deposit.Sizes>(e);
            _.AddComponent<Labor>(e);
            
            // Temps 
            _.AddComponent<LaborExtras>(e);
            _.AddComponent<LaborMigration>(e);
            _.AddBuffer<LaborNeed>(e); 
            _.AppendToBuffer(e, new LaborNeed { RGO = Deposit.RGO.Coal, Need = 10 });
            _.AppendToBuffer(e, new LaborNeed { RGO = Deposit.RGO.Iron, Need = 5 });
            _.AppendToBuffer(e, new LaborNeed { RGO = Deposit.RGO.Aluminum, Need = 3 });
            _.AppendToBuffer(e, new LaborNeed { RGO = Deposit.RGO.Gas, Need = 7 });
            _.AppendToBuffer(e, new LaborNeed { RGO = Deposit.RGO.Rare, Need = (ushort)random.NextInt(1, 30) });
            

            
            return e;
        }

        private void Assemble(EntityCommandBuffer _, Entity e, Deposit.Sizes sizes, Labor labor)  
        {
            _.SetComponent(e, sizes);
            _.SetComponent(e, labor);
        }

        private void ToBig(EntityCommandBuffer _, Entity e) =>
            Assemble(_, e, 
                new Deposit.Sizes { S = 20, M = 10, L = 4 },
                new Labor { Population = 12_500, Ceiling = 100_000 }
            );
        
        private void ToMedium(EntityCommandBuffer _, Entity e) =>
            Assemble(_, e, 
                new Deposit.Sizes { S = 40, M = 20, L = 0 },
                new Labor { Population = 3_000, Ceiling = 10_000 }
            );

        private void ToSmall(EntityCommandBuffer _, Entity e) =>
            Assemble(_, e, 
                new Deposit.Sizes { S = 100, M = 0, L = 0 },
                new Labor { Population = 250, Ceiling = 5_500 }
            );

        public Entity CreateBigDeposit(EntityCommandBuffer _, Deposit.RGO rgo, FixedString64Bytes name = default)
        {
//            Entity e = CreateDeposit(_, rgo, ref ,name);
            

            
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
            
            return Entity.Null;
        }
        
   
    }
}