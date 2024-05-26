using Unity.Entities;
using YAPCG.Engine.Common;
using YAPCG.Engine.Input.Systems;

namespace YAPCG.Engine.SystemGroups
{
    [UpdateInGroup(typeof(SystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(InputSystemGroup))]
    public partial class RenderSystemGroup : ComponentSystemGroup
    {
    }
}