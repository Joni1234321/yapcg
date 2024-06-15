using Unity.Entities;
using UnityEngine;
using YAPCG.Engine.Common;

namespace YAPCG.Engine.Time.Components
{
    public class RequiredTimeTicks : MonoBehaviour
    {
        public float TicksF;
        public int SpeedUp;
        
        private class Baker : Baker<RequiredTimeTicks>
        {
            public override void Bake(RequiredTimeTicks authoring)
            {
                CLogger.LogLoaded(authoring, "Ticks");
                Entity e = GetEntity(TransformUsageFlags.None);
                
                AddComponent(e, new Tick { TicksF = authoring.TicksF});
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