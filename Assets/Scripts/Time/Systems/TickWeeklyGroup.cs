using Unity.Entities;
using YAPCG.Time.Components;

namespace YAPCG.Time.Systems
{
    [UpdateInGroup(typeof(GameTimeSystemGroup))]
    public partial class TickWeeklyGroup : ComponentSystemGroup
    {
        protected override void OnUpdate()
        {
            float ticks = SystemAPI.GetSingleton<Ticks>().Value;
            if (ticks % 7 == 0)
                base.OnUpdate();
        }
    }
}