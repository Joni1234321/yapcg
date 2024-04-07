using Unity.Entities;

namespace YAPCG.Domain.Common.Components
{
    public struct BuildingSlotsLeft : IComponentData
    {
        public int Large;
        public int Medium;
        public int Small;
    }
}