using Unity.Entities;
using YAPCG.Simulation.Units;

namespace YAPCG.Domain.NUTS
{
    public struct Body
    {
        public struct BodyTag : IComponentData { }
        public struct PlanetTag : IComponentData { }
        public struct SunTag : IComponentData { }

        public struct Orbit : IComponentData
        {
            public Entity Parent;
            public float AU;
            public float Eccentricity;
            public SiTime Period;
            public float PeriodOffsetTicksF;
        }

        public struct TrueAnomaly : IComponentData
        {
            public float Value;
        }

        public struct BodyInfo : IComponentData
        {
            public float Mu;
            public float EarthRadius;
            public float EarthMass;
            public float EarthGravity;
            public float EscapeVelocity;
        }

        public struct Owner : IComponentData
        {
            public const byte NO_OWNER_ID = 0;
            public const byte YOU_OWNER_ID = 1;
            public byte ID;
        }

        
        public struct ActionClaim : IComponentData
        {
            public Entity Body;
            public byte OwnerID;
        }
    }
}