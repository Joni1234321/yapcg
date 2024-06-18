using Unity.Entities;

namespace YAPCG.Application.UserInterface.Components
{
    public struct FocusedBody : IComponentData
    {
        public Entity Selected, Hovered;
    }
}