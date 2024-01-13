using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using YAPCG.Engine.Components;
using YAPCG.Hub.Components;
using YAPCG.Planets.Components;

namespace YAPCG.Hub.Factories
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
            
            return e;
        }

        public static Entity CreateBigDeposit(EntityCommandBuffer _, float3 position, FixedString64Bytes name = default)
        {
            Entity e = CreateDeposits(_, position, name);
            
            var buffer = _.AddBuffer<Deposit.Sizes>(e);
            buffer.Add(new Deposit.Sizes { Size = 20 });
            buffer.Add(new Deposit.Sizes { Size = 10 });
            buffer.Add(new Deposit.Sizes { Size =  4 });
            return e;
        }
        
        public static Entity CreateMediumDeposit(EntityCommandBuffer _, float3 position, FixedString64Bytes name = default)
        {
            Entity e = CreateDeposits(_, position, name);
            
            var buffer = _.AddBuffer<Deposit.Sizes>(e);
            buffer.Add(new Deposit.Sizes { Size = 40 });
            buffer.Add(new Deposit.Sizes { Size = 20 });
            return e;
        }
        
        public static Entity CreateSmallDeposit(EntityCommandBuffer _, float3 position, FixedString64Bytes name = default)
        {
            Entity e = CreateDeposits(_, position, name);
            
            var buffer = _.AddBuffer<Deposit.Sizes>(e);
            buffer.Add(new Deposit.Sizes { Size = 100 });
            return e;
        }
        
    }
}