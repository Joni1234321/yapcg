using Unity.Entities;

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
            public float Distance;
            public float Eccentricity;
            public Simulation.Units.SiTime Period;
            public float PeriodOffsetTicksF;
        }

        public struct BodySize : IComponentData
        {
            public float Size;
        }

        public struct Owner : IComponentData
        {
            public const byte NO_OWNER = 0;
            public const byte YOU_OWNER = 1;
            public byte ID;
        }

        
        [InternalBufferCapacity(16)]
        public struct Claim : IBufferElementData
        {
            public byte Owner;
            public Entity Body;
        }
    }
}