using Unity.Entities;

namespace YAPCG.Domain.Common.Components
{
    public struct DiscoverProgress : IComponentData
    {
        public float Progress;
        public float Value;
        public float MaxValue;
    }
}