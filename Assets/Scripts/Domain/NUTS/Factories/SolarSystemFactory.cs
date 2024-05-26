using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.Common.Components;
using YAPCG.Engine.Common;
using YAPCG.Engine.Common.DOTS.Factory;
using YAPCG.Engine.Components;

namespace YAPCG.Domain.NUTS.Factories
{
    [InternalBufferCapacity(0)]
    public struct SolarySystemFactoryParams : IBufferElementData, IFactoryParams
    {
        public int Planets;
    }
    
    public struct SolarSystemFactory : IFactory<SolarySystemFactoryParams>
    {
        public void Spawn(EntityCommandBuffer ecb, SolarySystemFactoryParams config, ref Random random,
            ref NativeList<Entity> spawned)
        {
            // create planet
            spawned.Add(CreateSun(ecb, ref random));
            
            for (int i = 0; i < config.Planets; i++)
                spawned.Add(CreatePlanet(ecb, spawned[0], ref random));
        }           
        
        private Entity CreateSun(EntityCommandBuffer _, ref Random random)
        {
            Entity e = _.CreateEntity();
            
            // Name
            FixedString64Bytes name = NamingGenerator.Get(ref random);
            _.AddComponent(e, new Name { Value = name });
            _.SetName(e, $"SUN: {name}");
            
            // Give it a size            
            _.AddComponent(e, new SolarSystem.CelestialSize() { Size = random.NextGauss(100f, 30f, 50f, 300f)});

            return e;
        }
        
        private Entity CreatePlanet(EntityCommandBuffer _, Entity parent, ref Random random)
        {
            Entity e = _.CreateEntity();
            _.AddComponent<Deposit.DepositTag>(e);
            
            // Name
            FixedString64Bytes name = NamingGenerator.Get(ref random);
            _.AddComponent(e, new Name { Value = name });
            _.SetName(e, $"PLANET: {name}");
            
            // Orbit
            _.AddComponent(e, new SolarSystem.Orbiting() { Parent = parent} );
            _.AddComponent(e, new SolarSystem.OrbitingDistance() { Distance = random.NextFloat(1f, 10f)} );
            _.AddComponent(e, new SolarSystem.CelestialSize() { Size = random.NextGauss(10f, 3f, 1f, 100f)});
            
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

    }
}