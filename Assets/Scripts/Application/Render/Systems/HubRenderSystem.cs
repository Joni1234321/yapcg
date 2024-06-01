using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Content;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Application.UserInterface;
using YAPCG.Engine.Components;
using YAPCG.Engine.Render;
using YAPCG.Engine.SystemGroups;

namespace YAPCG.Application.Render.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    [BurstCompile]
    internal partial class HubRenderSystem : SystemBase
    {
        private EntityQuery _query;
        private static RenderUtils.ShaderHelper<Position> _positions = new(Shader.PropertyToID("_Positions"), sizeof(float) * 3);
        private static RenderUtils.ShaderHelper<AnimationComponent> _animations = new(Shader.PropertyToID("_AnimationsStartTime"), sizeof(float));
        private static RenderUtils.ShaderHelper<AnimationStateComponent> _states = new(Shader.PropertyToID("_State"), sizeof(float));
        private RenderParams _rp;


        [BurstCompile]
        protected override void OnCreate()
        {
            _query = SystemAPI.QueryBuilder().WithAll<AnimationComponent, AnimationStateComponent, Position, Domain.NUTS.Hub.HubTag>().Build();

            RequireForUpdate<MeshesSingleton>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            // OPTIMIZATION: SET COMPONENT FLAG THAT WHENEVER ANOTHER OBJECT SPAWNS, THEN CHANGE COMMAND, OTHERWISE DONT
            RenderHubs();
        }

        [BurstCompile]
        protected override void OnDestroy()
        {
            _positions.Dispose();
            _animations.Dispose();
            _states.Dispose();
        }

        [BurstDiscard]
        private void RenderHubs()
        {
            var meshes = SystemAPI.GetSingleton<MeshesSingleton>();
    
            if (!meshes.Deposit.LoadStarted)
            {
                meshes.Deposit.LoadAsync();
                SystemAPI.SetSingleton(meshes);
                return;
            }

            if (!meshes.Deposit.Loaded())
                return;

            Render(meshes.Deposit.Mesh.Result, meshes.Deposit.Material.Result);
        }


        private void Render(Mesh mesh, Material material) 
        {
            _rp = new RenderParams(material) { matProps = new MaterialPropertyBlock(), worldBounds = new Bounds(float3.zero, new float3(1000))};

            int n = _query.CalculateEntityCount();
            if (n == 0) 
                return;
            
            _positions.UpdateBuffer(_query, _rp.matProps, WorldUpdateAllocator);
            _animations.UpdateBuffer(_query, _rp.matProps, WorldUpdateAllocator);
            _states.UpdateBuffer(_query, _rp.matProps, WorldUpdateAllocator);
                
            Graphics.RenderMeshPrimitives(_rp, mesh, 0, n);
        }
        
        private Mesh GetMeshTemp ()
        { 
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh mesh = temp.GetComponent<MeshFilter>().mesh;
            UnityEngine.Object.Destroy(temp);
            return mesh;
        }
    }


}