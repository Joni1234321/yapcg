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
            _.AddComponent<Body.SunTag>(e);

            // Name
            FixedString64Bytes name = "Sol " + random.NextInt(50).ToRoman();
            _.AddComponent(e, new Name { Value = name });
            _.SetName(e, $"SUN: {name}");
            
            // Planet            
            float size = random.NextGauss(100f, 30f, 50f, 300f);
            _.AddComponent(e, new Body.BodySize { Size = size });
            _.AddComponent(e, new DiscoverProgress() { MaxValue = 100 });
            
            
            // Render
            _.AddComponent(e, new Position { Value = new float3(0) });
            _.AddComponent(e, new ScaleComponent { Value = size / 20f });
            _.AddComponent(e, new FadeStartTimeComponent { FadeStartTime = float.MinValue } );
            _.AddComponent(e, new AlternativeColorRatio { AlternativeRatio = 0 } );

            return e;
        }
        
        private Entity CreatePlanet(EntityCommandBuffer _, Entity parent, StandardGravitationalParameter mu, ref Random random)
        {
            Entity e = _.CreateEntity();
            _.AddComponent<Body.BodyTag>(e);
            _.AddComponent<Body.PlanetTag>(e);
            
            // Name
            FixedString64Bytes name = NamingGenerator.GetPlanetName(ref random);
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
            _.AddComponent(e, new DiscoverProgress() { MaxValue = 30, Progress = 1});

            // Render
            _.AddComponent(e, new Position { Value = new float3(orbitDistance, 0, 0) });
            _.AddComponent(e, new ScaleComponent { Value = size / 5f });
            _.AddComponent(e, new FadeStartTimeComponent { FadeStartTime = float.MinValue } );
            _.AddComponent(e, new AlternativeColorRatio { AlternativeRatio = 0 } );

            return e;
        }
    }
}