using Unity.Entities;

namespace YAPCG.Engine.Time.Components
{
    public struct TimeSpeed : IComponentData
    {
        public float SpeedUp;
    }
        
    internal struct Ticks : IComponentData
    {
        public int Value;
    }

    internal struct DeltaTick : IComponentData
    {
        public float Value;
    }
}