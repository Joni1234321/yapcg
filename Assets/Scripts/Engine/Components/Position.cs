using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Engine.Components
{
    public struct Position : IComponentData
    {
        public float3 Value;
    }

    public struct AnimationComponent : IComponentData
    {
        public float Time;
    }

    public struct StateComponent : IComponentData
    {
        public float State;
        public static StateComponent Nothing => new() { State = 0 };
        public static StateComponent Hovered => new() { State = 0.3f };
        public static StateComponent Selected => new() { State = 0.6f };
    }
}