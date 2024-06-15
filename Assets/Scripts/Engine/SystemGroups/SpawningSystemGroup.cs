// System that covers ticks

using Unity.Entities;
using YAPCG.Engine.Time.Systems;

namespace YAPCG.Engine.SystemGroups
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(GameTimeSystemGroup), OrderFirst = true)]
    public partial class SpawningSystemGroup : ComponentSystemGroup
    {
        
    }
}