using Unity.Entities;

namespace YAPCG.Domain.Hub.Components
{
    public struct Labor : IComponentData
    {
        public float Population;
        public float Ceiling;
    }
}