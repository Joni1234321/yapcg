using Unity.Entities;
using UnityEngine;

namespace YAPCG.Engine.Time.Components
{
    public class TimeTicks : MonoBehaviour
    {
        public float TicksF;
        public int SpeedUp;
        
        private class Baker : Baker<TimeTicks>
        {
            public override void Bake(TimeTicks authoring)
            {
                Entity e = GetEntity(TransformUsageFlags.None);
                
                AddComponent(e, new Tick() { TicksF = authoring.TicksF});
                AddComponent(e, new TickSpeed { SpeedUp = authoring.SpeedUp });
            }
        }
    }
    
    public struct TickSpeed : IComponentData
    {
        public float SpeedUp;
    }
        
    public struct Tick : IComponentData
    {
        public float TicksF;
    }
}