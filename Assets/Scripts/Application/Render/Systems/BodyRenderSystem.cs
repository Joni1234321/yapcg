using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Application.UserInterface;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Components;
using YAPCG.Engine.SystemGroups;

namespace YAPCG.Application.Render.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    [BurstCompile]
    internal partial class BodyRenderSystem : SystemBase
    {
        private EntityQuery _query;
        private GraphicsBuffer _positionsBuffer, _scalesBuffer;
        private static readonly int SHADER_POSITIONS = Shader.PropertyToID("_Positions");
        private static readonly int SHADER_SCALES = Shader.PropertyToID("_Scales");
        private RenderParams _rp;

        [BurstCompile]
        protected override void OnCreate()
        {
            _query = SystemAPI.QueryBuilder().WithAll<Position, ScaleComponent, Body.BodyTag>().Build();

            RequireForUpdate<MeshesSingleton>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            // OPTIMIZATION: SET COMPONENT FLAG THAT WHENEVER ANOTHER OBJECT SPAWNS, THEN CHANGE COMMAND, OTHERWISE DONT
            RenderBodies();
        }

        [BurstCompile]
        protected override void OnDestroy()
        {
            _positionsBuffer?.Dispose();
            _scalesBuffer?.Dispose();
        }

        [BurstDiscard]
        private void RenderBodies()
        {
            var meshes = SystemAPI.GetSingleton<MeshesSingleton>();
    
            if (!meshes.Body.LoadStarted)
            {
                meshes.Body.LoadAsync();
                SystemAPI.SetSingleton(meshes);
                return;
            }

            if (!meshes.Body.Loaded())
                return;

            Render(meshes.Body.Mesh.Result, meshes.Body.Material.Result);
        }


        private void Render(Mesh mesh, Material material) 
        {
            _rp = new RenderParams(material) { matProps = new MaterialPropertyBlock(), worldBounds = new Bounds(float3.zero, new float3(1000))};

            int n = _query.CalculateEntityCount();
            if (n == 0) 
                return;
            
            SetBuffer<Position>(_positionsBuffer, sizeof(float) * 3, SHADER_POSITIONS);
            SetBuffer<ScaleComponent>(_scalesBuffer, sizeof(float), SHADER_SCALES);

            Graphics.RenderMeshPrimitives(_rp, mesh, 0, n);
        }

        void SetBuffer<T>(GraphicsBuffer buffer, int structSize, int shaderProperty) where T : unmanaged, IComponentData
        {
            NativeArray<T> data = _query.ToComponentDataArray<T>(WorldUpdateAllocator);
            SetBuffer(buffer, data, structSize, shaderProperty);
        }
        
        void SetBuffer<T>(GraphicsBuffer buffer, NativeArray<T> data, int structSize, int shaderProperty) where T : unmanaged, IComponentData
        {
            buffer?.Release();
            buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, data.Length, structSize);
            buffer.SetData(data);
            _rp.matProps.SetBuffer(shaderProperty, buffer);
        }
    }
}