using System;
using System.ComponentModel;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Content;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Engine.SystemGroups;
using YAPCG.Planets.Components;
using YAPCG.UI;
using Object = UnityEngine.Object;

namespace YAPCG.Engine.Render.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    [BurstCompile]
    internal partial class CustomRenderSystem : SystemBase
    {
        private EntityQuery _query, _meshQuery;
        private GraphicsBuffer _positionBuffer, _animationBuffer;
        private static readonly int POSITION = Shader.PropertyToID("_Positions");
        private static readonly int ANIMATION = Shader.PropertyToID("_Animation");
        private RenderParams _rp;

        private Material _material;
        private Mesh _mesh;
        
        [BurstCompile]
        protected override void OnCreate()
        {
            _query = SystemAPI.QueryBuilder().WithAll<Anim, Position, HubTag>().Build();

            RequireForUpdate<MeshesReference>();
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


        void Render(Mesh mesh, Material material) 
        {
            const int POSITION_SIZE = sizeof(float) * 3;
            const int ANIMATION_SIZE = sizeof(uint);

            _rp = new RenderParams(material) { matProps = new MaterialPropertyBlock(), worldBounds = new Bounds(float3.zero, new float3(1) * 1000)};

            NativeArray<Position> positions = _query.ToComponentDataArray<Position>(WorldUpdateAllocator);
            NativeArray<Anim> animations = _query.ToComponentDataArray<Anim>(WorldUpdateAllocator);

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
                    _animationBuffer?.Release();
                    _animationBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, n, ANIMATION_SIZE);
                    _animationBuffer.SetData(animations);
                }
                _rp.matProps.SetBuffer(POSITION, _positionBuffer);
                _rp.matProps.SetBuffer(ANIMATION, _animationBuffer);
                Graphics.RenderMeshPrimitives(_rp, mesh, 0, n);
            }

        }
        Mesh GetMeshTemp ()
        { 
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh mesh = temp.GetComponent<MeshFilter>().mesh;
            Object.Destroy(temp);
            return mesh;
        }
    }
    
}