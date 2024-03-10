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

    public struct Animations : IComponentData
    {
        public float Time;
    }
}