using Unity.Entities;

namespace YAPCG.Engine.SystemGroups
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial class InputSystemGroup : ComponentSystemGroup
    {
        
    }
}