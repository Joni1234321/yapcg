using Unity.Entities;

namespace YAPCG.Domain.NUTS
{
    public struct SolarSystem
    {
        public struct PlanetTag : IComponentData { }

        public struct Orbiting : IComponentData
        {
            public Entity Parent;
        }

        public struct OrbitingDistance : IComponentData
        {
            public float Distance;
        }

        public struct CelestialSize : IComponentData
        {
            public float Size;
        }
    }
}