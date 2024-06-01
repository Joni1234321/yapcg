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

    public struct FadeStartTimeComponent : IComponentData
    {
        public float FadeStartTime;
    }

    public struct AlternativeColorRatio : IComponentData
    {
        public float AlternativeRatio;
        public static AlternativeColorRatio Nothing => new() { AlternativeRatio = 0 };
        public static AlternativeColorRatio Hovered => new() { AlternativeRatio = 0.15f };
        public static AlternativeColorRatio Selected => new() { AlternativeRatio = 0.4f };
    }
}