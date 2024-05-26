using Unity.Entities;

namespace YAPCG.Domain.Common.Components
{
    
    public struct Terrain : IComponentData
    {
        public enum TerrainType
        {
            Mountains,
            Hills,
            Valley,
            Flatlands,
            COUNT
        }

        public TerrainType Type;
    }
}