using Unity.Entities;

namespace YAPCG.Engine.Components
{
    public struct SharedRandom : IComponentData
    {
        public Unity.Mathematics.Random Random;
    }
}