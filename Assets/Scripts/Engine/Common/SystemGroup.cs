using Unity.Entities;

namespace YAPCG.Engine.Common
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)]
    public partial class SystemGroup : ComponentSystemGroup
    {
        
    }
}