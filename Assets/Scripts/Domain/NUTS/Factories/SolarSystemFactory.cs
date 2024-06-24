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
        public void Spawn(EntityCommandBuffer ecb, SolarSystemFactoryParams config, ref Random random,
            ref NativeList<Entity> spawned)
        {
            // create planet
            float earthMass = 330_000;
            spawned.Add(CreateSun(ecb, earthMass, ref random));
            StandardGravitationalParameter mu = new StandardGravitationalParameter(new MassConverter(earthMass, MassConverter.UnitType.EarthMass).To(MassConverter.UnitType.KiloGrams));
            for (int i = 0; i < config.Planets; i++)
                spawned.Add(CreatePlanet(ecb, spawned[0], mu, ref random));
        }
        
        private Entity CreateSun(EntityCommandBuffer _, float earthMass, ref Random random)
        {
            Entity e = _.CreateEntity();
            _.AddComponent<Body.BodyTag>(e);
            _.AddComponent<Body.SunTag>(e);

            // Name
            FixedString64Bytes name = "Sol " + random.NextInt(50).ToRoman();
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

            return e;
        }
        
        private Entity CreatePlanet(EntityCommandBuffer _, Entity parent, StandardGravitationalParameter mu, ref Random random)
        {
            // HINT: burstable code
            // no mananged array by using stackalloc
            // no typeof
            // faster if you set thea rchetype once 
            // _.create
            /* HOTPATH EP 2
            Entity e2 = _.CreateEntity(stackalloc ComponentType[] { ComponentType.ReadOnly<Body.BodySize>() });
            Entity w1 = _.CreateEntity(stackalloc ComponentType[]
                {
                    ComponentType.ReadOnly<Body.BodySize>(),
                    ComponentType.ReadOnly<Body.PlanetTag>()
                });*/
            Entity e = _.CreateEntity();
            _.AddComponent<Body.BodyTag>(e);
            _.AddComponent<Body.PlanetTag>(e);
            
            // Name
            FixedString64Bytes name = NamingGenerator.GetPlanetName(ref random);
            _.AddComponent(e, new Name { Value = name });
            _.SetName(e, $"PLANET: {name}");
                
            // Orbit
            // could have different generators depending on gas or giants
            float earthRadius = random.NextGauss(10f, 3f, 1f, 100f);
            float au = random.NextFloat(1f, 5f);
            float earthMass = random.NextGauss(10f, 3f, 1f, 100f);
            float offset = random.NextFloat(1);

            // float earthRadius = 1;
            // float au = 1;
            // float earthMass = 1;
            // float offset = random.NextFloat(1);
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
            _.AddComponent(e, new DiscoverProgress { MaxValue = 30, Progress = 1});
            _.AddComponent(e, new Body.Owner { ID = Body.Owner.NO_OWNER_ID });
            
            // Render
            _.AddComponent(e, new Position { Value = new float3(au, 0, 0) });
            _.AddComponent(e, new Body.TrueAnomaly { Value = 0 });
            _.AddComponent(e, new ScaleComponent { Value = earthRadius / 5f });
            _.AddComponent(e, new FadeStartTimeComponent { FadeStartTime = float.MinValue } );
            _.AddComponent(e, new AlternativeColorRatio { AlternativeRatio = 0 } );

            return e;
        }
    }
}