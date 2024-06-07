using Unity.Entities;

namespace YAPCG.Engine.Time.Components
{
    public struct TickSpeed : IComponentData
    {
        public float SpeedUp;
    }
        
    public struct Tick : IComponentData
    {
        public float TicksF;
    }


}