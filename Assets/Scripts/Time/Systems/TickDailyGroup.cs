using Unity.Entities;

namespace YAPCG.Time.Systems
{
    [UpdateInGroup(typeof(GameTimeSystemGroup))]
    public partial class TickDailyGroup : ComponentSystemGroup { }
}