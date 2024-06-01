using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Domain.Common.Components;
using YAPCG.Engine.Common;
using YAPCG.Engine.Common.DOTS.Factory;
using YAPCG.Engine.Components;
using YAPCG.Simulation.OrbitalMechanics;
using YAPCG.Simulation.Units;

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
            var mu = new StandardGravitationalParameter(new MassConverter(330_000, MassConverter.UnitType.EarthMass).To(MassConverter.UnitType.KiloGrams));
            for (int i = 0; i < config.Planets; i++)
                spawned.Add(CreatePlanet(ecb, spawned[0], mu, ref random));
        }
        
        private Entity CreateSun(EntityCommandBuffer _, ref Random random)
        {
            Entity e = _.CreateEntity();
            _.AddComponent<Body.BodyTag>(e);
            
            // Name
            FixedString64Bytes name = NamingGenerator.Get(ref random);
            _.AddComponent(e, new Name { Value = name });
            _.SetName(e, $"SUN: {name}");
            
            // Give it a size            
            float size = random.NextGauss(100f, 30f, 50f, 300f);
            _.AddComponent(e, new Body.BodySize { Size = size });

            // Render
            _.AddComponent(e, new Position { Value = new float3(0) });
            _.AddComponent(e, new ScaleComponent { Value = size / 20f });
            
            return e;
        }
        
        private Entity CreatePlanet(EntityCommandBuffer _, Entity parent, StandardGravitationalParameter mu, ref Random random)
        {
            Entity e = _.CreateEntity();
            _.AddComponent<Body.BodyTag>(e);
            _.AddComponent<Body.PlanetTag>(e);
            
            // Name
            FixedString64Bytes name = NamingGenerator.Get(ref random);
            _.AddComponent(e, new Name { Value = name });
            _.SetName(e, $"PLANET: {name}");
            
            // Orbit
            float size = random.NextGauss(10f, 3f, 1f, 100f);
            float orbitDistance = random.NextFloat(1f, 5f);
            float distance = new Length(orbitDistance, Length.UnitType.AstronomicalUnits).To(Length.UnitType.Meters);
            
            SiTime period = OrbitalMechanics.GetOrbitalPeriod(mu, distance);
            
            float linearEccentricity = EllipseMechanics.GetLinearEccentricity(distance, distance); // this is the case since its a circular orbit
            float eccentricity = distance == 0 ? 0 : linearEccentricity / distance;
            
            _.AddComponent(e, new Body.Orbit { Parent = parent, Period = period, Distance = orbitDistance, Eccentricity = eccentricity } );
            _.AddComponent(e, new Body.BodySize { Size = size });
            
            // Render
            _.AddComponent(e, new Position { Value = new float3(orbitDistance, 0, 0) });
            _.AddComponent(e, new ScaleComponent { Value = size / 5f});

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