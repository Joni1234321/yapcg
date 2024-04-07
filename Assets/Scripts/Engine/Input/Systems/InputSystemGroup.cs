using Unity.Entities;
using YAPCG.Engine.Common;

namespace YAPCG.Engine.Input.Systems
{
    [UpdateInGroup(typeof(SystemGroup), OrderFirst = true)]
    public partial class InputSystemGroup : ComponentSystemGroup
    {
        
    }
}