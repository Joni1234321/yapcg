using Unity.Entities;
using YAPCG.Engine.Time.Components;

namespace YAPCG.Engine.Time.Systems
{
    
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(GameTimeSystemGroup))]
    public partial class TickWeeklyGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            RequireForUpdate<Tick>();
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            int ticks = (int)SystemAPI.GetSingleton<Tick>().TicksF;
            if (ticks % 7 == 0)
                base.OnUpdate();
        }
    }
}