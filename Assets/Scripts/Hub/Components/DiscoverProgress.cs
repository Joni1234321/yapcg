using Unity.Entities;

namespace YAPCG.Planets.Components
{
    public struct DiscoverProgress : IComponentData
    {
        public float Progress;
        public float Value;
        public float MaxValue;
    }
}