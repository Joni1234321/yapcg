using Unity.Entities;
using YAPCG.Engine.Common;

namespace YAPCG.Engine.Input
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InputSystemGroup : ComponentSystemGroup
    {
        
    }
}