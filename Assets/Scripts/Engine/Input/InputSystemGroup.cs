using Unity.Entities;
using YAPCG.Engine.Common;

namespace YAPCG.Engine.Input
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(SystemGroup), OrderFirst = true)]
    public partial class InputSystemGroup : ComponentSystemGroup
    {
        
    }
}