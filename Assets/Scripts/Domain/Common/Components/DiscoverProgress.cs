using Unity.Entities;

namespace YAPCG.Domain.Common.Components
{
    public struct DiscoverProgress : IComponentData
    {
        public float Progress;
        public float Value;
        public float MaxValue;
    }

    public struct PlanetDiscovery : IComponentData
    {
        public bool Telescope; // is telescoped some intel
        public byte Flyby;     // flyby by satelite or solar system
        public const byte FLYBY_MAX = 10;
    }

    public struct LandDiscovery : IComponentData
    {
        public uint Orbit, Probes, People; 
        public uint OrbitThroughput, ProbesThroughput, PeopleThroughput; 
    }
}