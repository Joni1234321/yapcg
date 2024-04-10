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
        public float AnimationStart;
    }

    public struct AnimationStateComponent : IComponentData
    {
        public float AnimationState;
        public static AnimationStateComponent Nothing => new() { AnimationState = 0 };
        public static AnimationStateComponent Hovered => new() { AnimationState = 0.3f };
        public static AnimationStateComponent Selected => new() { AnimationState = 0.6f };
    }
}