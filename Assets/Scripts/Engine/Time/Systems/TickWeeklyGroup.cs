using Unity.Entities;
using YAPCG.Engine.Time.Components;

namespace YAPCG.Engine.Time.Systems
{
    [UpdateInGroup(typeof(GameTimeSystemGroup))]
    public partial class TickWeeklyGroup : ComponentSystemGroup
    {
        protected override void OnUpdate()
        {
            int ticks = (int)SystemAPI.GetSingleton<Tick>().TicksF;
            if (ticks % 7 == 0)
                base.OnUpdate();
        }
    }
}