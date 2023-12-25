using Unity.Entities;

namespace YAPCG.Engine.Time.Systems
{
    [UpdateInGroup(typeof(GameTimeSystemGroup))]
    public partial class TickDailyGroup : ComponentSystemGroup { }
}