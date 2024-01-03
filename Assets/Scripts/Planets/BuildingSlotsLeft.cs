using Unity.Entities;

namespace YAPCG.Planets
{
    public struct BuildingSlotsLeft : IComponentData
    {
        public int Large;
        public int Medium;
        public int Small;
    }
}