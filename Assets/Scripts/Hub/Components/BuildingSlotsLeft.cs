using Unity.Entities;

namespace YAPCG.Hub.Components
{
    public struct BuildingSlotsLeft : IComponentData
    {
        public int Large;
        public int Medium;
        public int Small;
    }
}