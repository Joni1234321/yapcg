using Unity.Entities;

namespace YAPCG.Engine.SystemGroups
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(InputSystemGroup))]
    public partial class RenderSystemGroup : ComponentSystemGroup
    {
        
    }
}