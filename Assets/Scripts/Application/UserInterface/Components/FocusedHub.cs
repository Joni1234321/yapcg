using Unity.Entities;

namespace YAPCG.UI.Components
{
    public struct FocusedHub : IComponentData
    {
        public Entity Selected, Hovered;
    }
}