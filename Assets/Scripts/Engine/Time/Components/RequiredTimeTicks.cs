using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using YAPCG.Engine.Common;

namespace YAPCG.Engine.Time.Components
{
    public class RequiredTimeTicks : MonoBehaviour
    {
        public float TicksF = 0;
        public int SpeedLevel = 1;
        public float[] SpeedUpLevels = { 0, 1, 7, 30, 120, 360, 1000};
        
        private class Baker : Baker<RequiredTimeTicks>
        {
            public override void Bake(RequiredTimeTicks authoring)
            {
                CLogger.LogLoaded(authoring, "Ticks");
                Entity e = GetEntity(TransformUsageFlags.None);
                
                AddComponent(e, new Tick { TicksF = authoring.TicksF});
                AddComponent(e, new TickSpeed { SpeedUp = authoring.SpeedUpLevels[authoring.SpeedLevel] });
                AddComponent(e, new TickSpeedLevel { Level =  authoring.SpeedLevel });

                DynamicBuffer<TickSpeedLevels> buffer = AddBuffer<TickSpeedLevels>(e);
                for (int i = 0; i < authoring.SpeedUpLevels.Length; i++)
                    buffer.Add(new TickSpeedLevels { Speed = authoring.SpeedUpLevels[i] } );
            }
        }
    }

    public struct TickSpeedLevel : IComponentData
    {
        public bool Paused;
        public int Level;
    }

    [InternalBufferCapacity(16)]
    public struct TickSpeedLevels : IBufferElementData
    {
        public float Speed;
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