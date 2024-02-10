using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Engine.SystemGroups;

namespace YAPCG.Engine.Render.Systems
{
    [UpdateInGroup(typeof(RenderSystemGroup))]
    internal partial struct CustomRenderSystem : ISystem
    {
        private EntityQuery query;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            query = new EntityQueryBuilder(state.WorldUpdateAllocator).WithAll<Position, RenderPositionOnlyTag>().Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            NativeArray<Position> data = query.ToComponentDataArray<Position>(Allocator.Temp);
            //Debug.Log(data.Length);
            data.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
    
    [BurstCompile]
    internal struct RenderPositionOnlyChunkJob : IJobChunk
    {
        [ReadOnly] public ComponentTypeHandle<Position> PositionTypeHandle;
        [ReadOnly] public ComponentTypeHandle<RenderMeshArray> RenderMeshArrayHandle;
        
        [BurstCompile]
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            NativeArray<Position> positions = chunk.GetNativeArray(ref PositionTypeHandle);
            
            
        }
    }
     
}