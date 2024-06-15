﻿using Unity.Entities;
using YAPCG.Engine.Time.Components;

namespace YAPCG.Engine.Time.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(Common.SystemGroup))]
    // Should not run directly in this group instead if you want every tick, then do TickDailyGroup
    public partial class GameTimeSystemGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            RequireForUpdate<Tick>();
            RequireForUpdate<TickSpeed>();
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