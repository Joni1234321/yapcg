using System;
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
    }
}