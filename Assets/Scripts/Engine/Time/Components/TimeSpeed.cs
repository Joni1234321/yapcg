using Unity.Entities;

namespace YAPCG.Engine.Time.Components
{
    public struct TimeSpeed : IComponentData
    {
        public float SpeedUp;
    }
        
    public struct Ticks : IComponentData
    {
        public int Value;
    }

    public struct DeltaTick : IComponentData
    {
        public float Value;
    }
}