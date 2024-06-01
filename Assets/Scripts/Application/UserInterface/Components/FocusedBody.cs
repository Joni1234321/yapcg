using Unity.Entities;

namespace YAPCG.UI.Components
{
    public struct FocusedBody : IComponentData
    {
        public Entity Selected, Hovered;
    }
}