using Unity.Entities;
using YAPCG.Engine.Common;
using YAPCG.Engine.Input;

namespace YAPCG.Engine.SystemGroups
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(SystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(InputSystemGroup))]
    public partial class RenderSystemGroup : ComponentSystemGroup
    {
    }
}