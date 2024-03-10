using System;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Engine.Components
{
    public struct Position : IComponentData
    {
        public float3 Value;
    }

    public struct Anim : IComponentData
    {
        public uint Value;

        public const uint MAX_VALUE = 0xFF;
    }

    public struct AnimStart : IComponentData
    {
        public float Time;
    }
}