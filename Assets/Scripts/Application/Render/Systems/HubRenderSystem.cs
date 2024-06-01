using Unity.Burst;
using Unity.Entities;
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
        private static RenderUtils.ShaderHelper<FadeStartTimeComponent> _fadeStartTimes = new(Shader.PropertyToID("_FadeStartTimes"), sizeof(float));
        private static RenderUtils.ShaderHelper<AlternativeColorRatio> _alternativeColorRatios = new(Shader.PropertyToID("_AlternativeColorRatios"), sizeof(float));

        [BurstCompile]
        protected override void OnCreate()
        {
            _query = SystemAPI.QueryBuilder().WithAll<FadeStartTimeComponent, AlternativeColorRatio, Position, Domain.NUTS.Hub.HubTag>().Build();

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
            _fadeStartTimes.Dispose();
            _alternativeColorRatios.Dispose();
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
            RenderParams renderParams = new RenderParams(material) { matProps = new MaterialPropertyBlock(), worldBounds = new Bounds(float3.zero, new float3(1000))};

            int n = _query.CalculateEntityCount();
            if (n == 0) 
                return;
            
            _positions.UpdateBuffer(_query, renderParams.matProps, WorldUpdateAllocator);
            _fadeStartTimes.UpdateBuffer(_query, renderParams.matProps, WorldUpdateAllocator);
            _alternativeColorRatios.UpdateBuffer(_query, renderParams.matProps, WorldUpdateAllocator);
                
            Graphics.RenderMeshPrimitives(renderParams, mesh, 0, n);
        }
        
    }


}