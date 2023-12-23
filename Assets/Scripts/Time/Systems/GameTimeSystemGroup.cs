// System that covers ticks
using Unity.Entities;
using YAPCG.Time.Components;

namespace YAPCG.Time.Systems
{
    // Should not run directly in this group instead if you want every tick, then do TickDailyGroup
    internal partial class GameTimeSystemGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            World.EntityManager.CreateSingleton(new Ticks());
            World.EntityManager.CreateSingleton(new DeltaTick());
            World.EntityManager.CreateSingleton(new TimeSpeed { SpeedUp = 4 });
            
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            float dt = World.Time.DeltaTime;
            float speedup = SystemAPI.GetSingleton<TimeSpeed>().SpeedUp;
            
            RefRW<DeltaTick> deltaTick = SystemAPI.GetSingletonRW<DeltaTick>();
            deltaTick.ValueRW.Value += dt * speedup;

            if (deltaTick.ValueRO.Value >= 1)
            {
                RefRW<Ticks> ticks = SystemAPI.GetSingletonRW<Ticks>();
                ticks.ValueRW.Value = ticks.ValueRO.Value + 1;
                deltaTick.ValueRW.Value = 0;
                
                base.OnUpdate();    
            }
        }
    }

    
    
}