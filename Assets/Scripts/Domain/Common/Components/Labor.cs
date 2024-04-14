using System.Collections.Generic;
using Unity.Entities;
using YAPCG.Domain.NUTS;

namespace YAPCG.Domain.Common.Components
{
    public struct Labor : IComponentData
    {
        public long Population;
        public long Ceiling;
    }

    public struct LaborExtras : IComponentData
    {
        public float HousingModifier;
        public long LaborDifference;
        public int NeedsMissing;
    }

    public struct LaborMigration : IComponentData
    {
        public int In;
        public int Out;
        public long EmigratingPopulation;
        public Entity Target;
        
        public struct InSorter : IComparer<LaborMigration>
        { 
            public int Compare(LaborMigration x, LaborMigration y) => x.In.CompareTo(y.In);
        }
    }

    [InternalBufferCapacity(8)]
    public struct LaborNeed : IBufferElementData
    {
        public Deposit.RGO RGO;
        public ushort Need;
        public ushort Received;
    }
    
}