using Unity.Entities;

namespace YAPCG.Domain.Common.Components
{
    public struct Labor : IComponentData
    {
        public float Population;
        public float Ceiling;
    }
}