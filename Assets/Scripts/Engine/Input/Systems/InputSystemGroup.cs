using Unity.Entities;
using YAPCG.Engine.Common;

namespace YAPCG.Engine.SystemGroups
{
    [UpdateInGroup(typeof(SystemGroup), OrderFirst = true)]
    public partial class InputSystemGroup : ComponentSystemGroup
    {
        
    }
}