// System that covers ticks

using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Engine.Time.Components;

namespace YAPCG.Engine.Time.Systems
{
    
    [UpdateInGroup(typeof(Common.SystemGroup))]
    // Should not run directly in this group instead if you want every tick, then do TickDailyGroup
    public partial class GameTimeSystemGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            World.EntityManager.CreateSingleton(new Tick());
            World.EntityManager.CreateSingleton(new TickSpeed { SpeedUp = 10 });
            
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            float dt = World.Time.DeltaTime;
            float speedup = SystemAPI.GetSingleton<TickSpeed>().SpeedUp;
            
            RefRW<Tick> tick = SystemAPI.GetSingletonRW<Tick>();
            int ticks = (int)tick.ValueRO.TicksF;
            float deltaTicks = tick.ValueRO.TicksF - ticks + dt * speedup;
            
            if (deltaTicks >= 1)
            {
                tick.ValueRW.TicksF = ticks + 1;
                base.OnUpdate();
            }
            else
            {
                tick.ValueRW.TicksF = ticks + deltaTicks;
            }
        }
    }

    
    
}