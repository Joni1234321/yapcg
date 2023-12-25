// System that covers ticks

using Unity.Entities;
using YAPCG.Engine.Time.Components;
using YAPCG.Engine.Time.Systems;

namespace YAPCG.Engine.SystemGroups
{
    [UpdateInGroup(typeof(GameTimeSystemGroup), OrderFirst = true)]
    public partial class SpawningSystemGroup : ComponentSystemGroup
    {
        
    }
}