using Unity.Entities;

namespace YAPCG.Hub.Components
{
    public struct Labor : IComponentData
    {
        public float Population;
        public float Ceiling;
    }
}