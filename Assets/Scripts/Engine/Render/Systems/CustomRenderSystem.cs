﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Content;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Planets.Components;
using YAPCG.UI;
using Object = UnityEngine.Object;

namespace YAPCG.Engine.Render.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    [BurstCompile]
    internal partial class CustomRenderSystem : SystemBase
    {
        private EntityQuery _query;
        private GraphicsBuffer _positionBuffer, _animationBuffer, _animationsBuffer;
        private static readonly int SHADER_POSITIONS = Shader.PropertyToID("_Positions");
        private static readonly int SHADER_ANIMATIONS = Shader.PropertyToID("_Animations");
        private RenderParams _rp;

        [BurstCompile]
        protected override void OnCreate()
        {
            _query = SystemAPI.QueryBuilder().WithAll<AnimationComponent, Position, HubTag>().Build();

            RequireForUpdate<MeshesReference>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            // OPTIMIZATION: SET COMPONENT FLAG THAT WHENEVER ANOTHER OBJECT SPAWNS, THEN CHANGE COMMAND, OTHERWISE DONT
            RenderHubs();
        }

        [BurstDiscard]
        private void RenderHubs()
        {
            var meshes = SystemAPI.GetSingleton<MeshesReference>();
    
            if (!meshes.LoadStarted)
            {
                var meshesRW = SystemAPI.GetSingletonRW<MeshesReference>();
                meshesRW.ValueRW.DepositMaterial.LoadAsync();
                meshesRW.ValueRW.DepositMesh.LoadAsync();
                meshesRW.ValueRW.LoadStarted = true;
                return;
            }

            if (meshes.DepositMaterial.LoadingStatus != ObjectLoadingStatus.Completed ||
                meshes.DepositMesh.LoadingStatus != ObjectLoadingStatus.Completed)
                return;

            Render(meshes.DepositMesh.Result, meshes.DepositMaterial.Result);
        }


        private void Render(Mesh mesh, Material material) 
        {
            const int POSITION_SIZE = sizeof(float) * 3;
            const int ANIMATION_SIZE = sizeof(float);

            _rp = new RenderParams(material) { matProps = new MaterialPropertyBlock(), worldBounds = new Bounds(float3.zero, new float3(1) * 1000)};

            NativeArray<Position> positions = _query.ToComponentDataArray<Position>(WorldUpdateAllocator);

            int n = positions.Length;

            if (n != 0)
            {
                //if (_buffer.count != n)
                {
                    _positionBuffer?.Release();
                    _positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, n, POSITION_SIZE);
                    _positionBuffer.SetData(positions);
                }
                {
                    _animationsBuffer?.Release();
                    _animationsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, n, ANIMATION_SIZE);
                    _animationsBuffer.SetData(_query.ToComponentDataArray<AnimationComponent>(WorldUpdateAllocator));
                }
                
                _rp.matProps.SetBuffer(SHADER_POSITIONS, _positionBuffer);
                _rp.matProps.SetBuffer(SHADER_ANIMATIONS, _animationsBuffer);
                
                Graphics.RenderMeshPrimitives(_rp, mesh, 0, n);
            }

        }

        private Mesh GetMeshTemp ()
        { 
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh mesh = temp.GetComponent<MeshFilter>().mesh;
            Object.Destroy(temp);
            return mesh;
        }
    }
    
}