using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Engine.Components
{
    public struct Position : IComponentData
    {
        public float3 Value;
    }

    public struct ScaleComponent : IComponentData
    {
        public float Value;
    }

    public struct FadeComponent : IComponentData
    {
        public float FadeStartTime;
    }

    public struct StateColorScaleComponent : IComponentData
    {
        public float StateColorScale;
        public static StateColorScaleComponent Nothing => new() { StateColorScale = 0 };
        public static StateColorScaleComponent Hovered => new() { StateColorScale = 0.3f };
        public static StateColorScaleComponent Selected => new() { StateColorScale = 0.6f };
    }
}