using Unity.Entities;

namespace YAPCG.Domain.Common.Components
{
    public struct Labor : IComponentData
    {
        public long Population;
        public long Ceiling;
    }

    public struct LaborExtras : IComponentData
    {
        public long LaborDifference;
        public int MigrationValue;
        public int EmigrationValue;
        
    }
    
}