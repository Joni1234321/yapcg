using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

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
