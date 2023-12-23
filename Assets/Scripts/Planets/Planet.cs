using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Planets
{
    public struct Planet : IComponentData
    {
    
    }

    public struct PlanetRenderDetails : IComponentData
    {
        public float Size;
        public float3 Position;
        public float4 Color;
    }

    public struct PlanetName: IComponentData { 
        public FixedString128Bytes Name;
    }
}