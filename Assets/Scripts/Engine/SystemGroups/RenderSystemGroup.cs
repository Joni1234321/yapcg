using Unity.Entities;
using YAPCG.Engine.Common;
using YAPCG.Engine.Input.Systems;
using YAPCG.Engine.SystemGroups;

namespace YAPCG.Engine.Render.Systems
{
    [UpdateInGroup(typeof(SystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(InputSystemGroup))]
    public partial class RenderSystemGroup : ComponentSystemGroup
    {
    }
}