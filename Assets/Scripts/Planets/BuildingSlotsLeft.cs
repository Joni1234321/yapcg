using Unity.Entities;

namespace YAPCG.Planets
{
    public struct BuildingSlotsLeft : IComponentData
    {
        public int Big;
        public int Medium;
        public int Small;
    }
}