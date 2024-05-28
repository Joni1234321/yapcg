using Unity.Entities;

namespace YAPCG.Domain.NUTS
{
    public struct Body
    {
        public struct BodyTag : IComponentData { }
        public struct PlanetTag : IComponentData { }

        public struct Orbiting : IComponentData
        {
            public Entity Parent;
        }

        public struct OrbitingDistance : IComponentData
        {
            public float Distance;
        }

        public struct BodySize : IComponentData
        {
            public float Size;
        }
    }
}