using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Engine.SystemGroups;
using YAPCG.Planets.Components;
using YAPCG.UI;

namespace YAPCG.Engine.Render.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    [BurstCompile]
    internal partial class CustomRenderSystem : SystemBase
    {
        private EntityQuery _query;
        private GraphicsBuffer _buffer;
        private static readonly int POSITION = Shader.PropertyToID("_Positions");
        private RenderParams _rp;

        private Material _material;
        private Mesh _mesh;
        
        [BurstCompile]
        protected override void OnCreate()
        {
            _material = Meshes.Instance.Deposit.RenderMeshArray.Materials[0];
            _material = UnityEngine.Resources.Load<Material>($"Graphics/Deposit/Hub.mat");
            _mesh = UnityEngine.Resources.Load<Mesh>($"Graphics/Deposit/Hub.mesh");

            _query = new EntityQueryBuilder(WorldUpdateAllocator).WithAll<Position, HubTag>().Build(this);

        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            // OPTIMIZATION: SET COMPONENT FLAG THAT WHENEVER ANOTHER OBJECT SPAWNS, THEN CHANGE COMMAND, OTHERWISE DONT
            RenderHubs();
        }

        [BurstDiscard]
        void RenderHubs()
        {
            const int POSITION_SIZE = sizeof(float) * 3;
            
            Mesh mesh = Meshes.Instance.Deposit.RenderMeshArray.Meshes[0];
            Material material = Meshes.Instance.Deposit.RenderMeshArray.Materials[0];

            _rp = new RenderParams(material) { matProps = new MaterialPropertyBlock(), worldBounds = new Bounds(float3.zero, new float3(1) * 1000)};

            NativeArray<Position> positions = _query.ToComponentDataArray<Position>(WorldUpdateAllocator);
            int n = positions.Length;

            if (n != 0)
            {
                //if (_buffer.count != n)
                {
                    _buffer?.Release();
                    _buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, n, POSITION_SIZE);
                    _buffer.SetData(positions);
                } 
                _rp.matProps.SetBuffer(POSITION, _buffer);
                Graphics.RenderMeshPrimitives(_rp, mesh, 0, n);
            }
        }
    }
    
}