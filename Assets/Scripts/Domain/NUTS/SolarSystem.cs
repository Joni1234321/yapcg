using Unity.Entities;
using YAPCG.Domain.NUTS.SpawnConfigs;

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
        
        
        

    }
}