using Unity.Entities;

namespace YAPCG.Engine.Time.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(GameTimeSystemGroup))]
    public partial class TickDailyGroup : ComponentSystemGroup { }
}