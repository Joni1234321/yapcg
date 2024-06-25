using Unity.Entities;
using YAPCG.Engine.Common;
using YAPCG.Engine.Input;

namespace YAPCG.Engine.SystemGroups
{
        [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class RenderSystemGroup : ComponentSystemGroup
    {
    }
}