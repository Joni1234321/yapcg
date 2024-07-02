using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Domain.Common.Components;
using YAPCG.Engine.Common;
using YAPCG.Engine.Common.DOTS.Factory;
using YAPCG.Engine.Components;
using YAPCG.Simulation.OrbitalMechanics;
using YAPCG.Simulation.Units;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Domain.NUTS.Factories
{
    [System.Serializable]
    [InternalBufferCapacity(0)]
    public struct SolarSystemFactoryParams : IBufferElementData, IFactoryParams
    {
        public int Planets;
    }
    
    public struct SolarSystemFactory : IFactory<SolarSystemFactoryParams>
    {
        public void Spawn(ref Random random,
            ref EntityCommandBuffer ecb,
            ref NativeList<Entity> spawned, in SolarSystemFactoryParams config)
        {
            // create planet
            float earthMass = 330_000;
            CreateSun(ref random, ref ecb, ref spawned, earthMass);
            StandardGravitationalParameter mu = new StandardGravitationalParameter(new MassConverter(earthMass, MassConverter.UnitType.EarthMass).To(MassConverter.UnitType.KiloGrams));
            for (int i = 0; i < config.Planets; i++)
                spawned.Add(CreatePlanet(ref ecb, ref random, spawned[0], mu));

            const float ASTEROID_AU = 7f;
            const int ASTEROID_COUNT = 100;
            CreateAsteroidBelt(ref random, ref ecb, ref spawned, spawned[0], mu, ASTEROID_AU, ASTEROID_COUNT);
        }
        
        private void CreateSun(ref Random random, ref EntityCommandBuffer _, ref NativeList<Entity> spawned,
            float earthMass)
        {
            Entity e = _.CreateEntity();
            _.AddComponent<Body.BodyTag>(e);
            _.AddComponent<Body.SunTag>(e);

            // Name
            FixedString64Bytes name = "Sol " + random.NextInt(1, 50).ToRoman();
            _.AddComponent(e, new Name { Value = name });
            _.SetName(e, $"SUN: {name}");
            
            // Planet            
            float earthRadius = random.NextGauss(100f, 30f, 50f, 300f);
            _.AddComponent(e, new Body.BodyInfo { EarthRadius = earthRadius, EarthMass = earthMass, });
            _.AddComponent(e, new DiscoverProgress { MaxValue = 100 });
            _.AddComponent(e, new Body.Owner { ID = Body.Owner.NO_OWNER_ID });
            
            // Render
            _.AddComponent(e, new Position { Value = new float3(0) });
            _.AddComponent(e, new Body.TrueAnomaly { Value = 0 });
            _.AddComponent(e, new ScaleComponent { Value = earthRadius / 20f });
            _.AddComponent(e, new FadeStartTimeComponent { FadeStartTime = float.MinValue } );
            _.AddComponent(e, new AlternativeColorRatio { AlternativeRatio = 0 } );

            spawned.Add(e);
        }
        
        private Entity CreatePlanet(ref EntityCommandBuffer _, ref Random random, in Entity parent, in StandardGravitationalParameter mu)
        {
            // HINT: burstable code
            // no mananged array by using stackalloc
            // no typeof
            // faster if you set thea rchetype once 
            // _.create
            /* HOTPATH EP 2
            Entity e2 = _.CreateEntity(stackalloc ComponentType[] { ComponentType.ReadOnly<Body.BodySize>() });*/
            Entity e = _.CreateEntity();
            _.AddComponent<Body.BodyTag>(e);
            _.AddComponent<Body.PlanetTag>(e);
            
            // Name
            FixedString64Bytes name = NamingGenerator.GetPlanetName(ref random);
            _.AddComponent(e, new Name { Value = name });
            _.SetName(e, $"PLANET: {name}");
                
            // Orbit
            // could have different generators depending on gas or giants
            float earthRadius = random.NextGauss(2f, 3f, 0.5f, 100f);
            float au = random.NextFloat(1f, 5f);
            float earthMass = random.NextGauss(10f, 3f, 1f, 100f);
            float offset = random.NextFloat(1);

            // Orbit calculation
            float distance = new Length(au, Length.UnitType.AstronomicalUnits).To(Length.UnitType.Meters);
            float radius = new Length(earthRadius, Length.UnitType.EarthRadius).To(Length.UnitType.Meters);
            float mass = new MassConverter(earthMass, MassConverter.UnitType.EarthMass).To(MassConverter.UnitType.KiloGrams);
            SiTime period = OrbitalMechanics.GetOrbitalPeriod(mu, distance);
            float offsetTicksF = offset * period.Days;
            
            float linearEccentricity = EllipseMechanics.GetLinearEccentricity(distance, distance); // this is the case since its a circular orbit
            float eccentricity = distance > 0 ? linearEccentricity / distance : 0;
            
            // gravity
            float ownMu = new StandardGravitationalParameter(mass).Value;
            float gravity = ownMu / math.pow(radius, 2);
            float ecapeVelocity = math.sqrt(2 * ownMu / radius);
            
            _.AddComponent(e, new Body.Orbit { Parent = parent, Period = period, AU = au, Eccentricity = eccentricity, PeriodOffsetTicksF = offsetTicksF } );
            _.AddComponent(e, new Body.BodyInfo { EarthRadius = earthRadius, EarthMass = earthMass, EarthGravity = gravity, EscapeVelocity = ecapeVelocity, Mu = ownMu});
            _.AddComponent(e, new DiscoverProgress { MaxValue = 30, Progress = 0 });
            _.AddComponent(e, new Body.Owner { ID = Body.Owner.NO_OWNER_ID });
            
            // Render
            _.AddComponent(e, new Position { Value = new float3(au, 0, 0) });
            _.AddComponent(e, new Body.TrueAnomaly { Value = 0 });
            _.AddComponent(e, new ScaleComponent { Value = earthRadius / 3f });
            _.AddComponent(e, new FadeStartTimeComponent { FadeStartTime = float.MinValue } );
            _.AddComponent(e, new AlternativeColorRatio { AlternativeRatio = 0 } );

            return e;
        }


        private void CreateAsteroidBelt(ref Random random, ref EntityCommandBuffer _, ref NativeList<Entity> spawned, in Entity parent, in StandardGravitationalParameter mu, float au, int count)
        {
            for (int i = 0; i < count; i++)
                CreateAsteroid(ref _, parent, mu, au, ref random, ref spawned);
        }
        
        private void CreateAsteroid(ref EntityCommandBuffer _, in Entity parent, in StandardGravitationalParameter mu, float au, ref Random random, ref NativeList<Entity> spawned)
        {
            Entity e = _.CreateEntity();
            _.AddComponent<Body.BodyTag>(e);
            _.AddComponent<Body.AsteroidTag>(e);
            
            // Name
            FixedString64Bytes name = NamingGenerator.GetPlanetName(ref random);
            _.AddComponent(e, new Name { Value = name });
            _.SetName(e, $"ASTEROID: {name}");
                
            // Orbit
            // https://en.wikipedia.org/wiki/Asteroid_belt#/media/File:Main_belt_asteroid_size_distribution.svg, linear distribution
            float earthRadius = random.NextGauss(1, 1, 0.1f, 8);
            float earthMass = random.NextGauss(1, 1, 0.1f, 8);
            float offset = random.NextFloat(1);

            // Orbit calculation
            float distance = new Length(au, Length.UnitType.AstronomicalUnits).To(Length.UnitType.Meters);
            float radius = new Length(earthRadius, Length.UnitType.EarthRadius).To(Length.UnitType.Meters);
            float mass = new MassConverter(earthMass, MassConverter.UnitType.EarthMass).To(MassConverter.UnitType.KiloGrams);
            SiTime period = OrbitalMechanics.GetOrbitalPeriod(mu, distance);
            float offsetTicksF = offset * period.Days;
            
            float linearEccentricity = EllipseMechanics.GetLinearEccentricity(distance, distance); // this is the case since its a circular orbit
            float eccentricity = distance > 0 ? linearEccentricity / distance : 0;
            
            // gravity
            float ownMu = new StandardGravitationalParameter(mass).Value;
            float gravity = ownMu / math.pow(radius, 2);
            float ecapeVelocity = math.sqrt(2 * ownMu / radius);
            
            _.AddComponent(e, new Body.Orbit { Parent = parent, Period = period, AU = au, Eccentricity = eccentricity, PeriodOffsetTicksF = offsetTicksF } );
            _.AddComponent(e, new Body.BodyInfo { EarthRadius = earthRadius, EarthMass = earthMass, EarthGravity = gravity, EscapeVelocity = ecapeVelocity, Mu = ownMu});
            _.AddComponent(e, new DiscoverProgress { MaxValue = 30, Progress = 0 });
            _.AddComponent(e, new Body.Owner { ID = Body.Owner.NO_OWNER_ID });
            
            // Render
            _.AddComponent(e, new Position { Value = new float3(au, 0, 0) });
            _.AddComponent(e, new Body.TrueAnomaly { Value = 0 });
            _.AddComponent(e, new ScaleComponent { Value = earthRadius / 2f });
            _.AddComponent(e, new FadeStartTimeComponent { FadeStartTime = float.MinValue } );
            _.AddComponent(e, new AlternativeColorRatio { AlternativeRatio = 0 } );

            spawned.Add(e);
        }

    }
}