using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.Common.Components;
using YAPCG.Engine.Components;

namespace YAPCG.Domain.NUTS.Factories
{
    public struct DepositFactory
    {
        private static Entity CreateDeposits(EntityCommandBuffer _, float3 position, FixedString64Bytes name = default)
        {
            Entity e = _.CreateEntity();
            _.AddComponent<Deposit.Tag>(e);
            
            _.AddComponent(e, new Name { Value = name });
            _.SetName(e, $"DEPOSIT: {name}");
    
            _.AddComponent(e, new DiscoverProgress { Progress = 1, Value = 0, MaxValue = 20});            
            
            _.AddComponent(e, new Deposit.Reserves { Value = 0});
            _.AddComponent<Deposit.Sizes>(e);
            return e;
        }

        public static Entity CreateBigDeposit(EntityCommandBuffer _, float3 position, FixedString64Bytes name = default)
        {
            Entity e = CreateDeposits(_, position, name);
            
            _.SetComponent(e, new Deposit.Sizes { S = 20, M = 10, L = 4 });

            
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
        
        public static Entity CreateMediumDeposit(EntityCommandBuffer _, float3 position, FixedString64Bytes name = default)
        {
            Entity e = CreateDeposits(_, position, name);
            _.SetComponent(e, new Deposit.Sizes { S = 40, M = 20 });
            return e;
        }
        
        public static Entity CreateSmallDeposit(EntityCommandBuffer _, float3 position, FixedString64Bytes name = default)
        {
            Entity e = CreateDeposits(_, position, name);
            _.SetComponent(e, new Deposit.Sizes { S = 100 });
            return e;
        }
        
    }
}