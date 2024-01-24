using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Engine.Time.Systems;
using YAPCG.UI;

namespace YAPCG.Hub.Systems
{
    public partial struct RenderSystem : ISystem
    {
        const int w = 500, h = 500;
        private NativeArray<Matrix4x4> positions;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            positions = GeneratePositions(w, h);
        }

        public void OnUpdate(ref SystemState state)
        {
            RenderParams renderParams = new RenderParams(Meshes.Instance.Hub.Material);
            Mesh mesh = Meshes.Instance.Hub.Mesh;
            
            const int commands = 2;
            GraphicsBuffer buffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, commands, GraphicsBuffer.IndirectDrawArgs.size);
            GraphicsBuffer.IndirectDrawArgs[] commandData = new GraphicsBuffer.IndirectDrawArgs[commands];

            commandData[0].instanceCount = 10;
            commandData[0].vertexCountPerInstance = 10;
            GraphicsBuffer.IndirectDrawArgs a = new GraphicsBuffer.IndirectDrawArgs
            {
                vertexCountPerInstance = 10,
                instanceCount = 100,
                startVertex = 0,
                startInstance = 0
            };
            
            commandData[1] = a;
            buffer.SetData(commandData);
            
            
            for (int i = 0; i < positions.Length; i++)
            {
                //Graphics.RenderMesh(renderParams, mesh, 0, positions[i]); // CPU 20 GPU 10, T 15.4M V 10.3M
            }
            // Not good at frustrum culling
            // Graphics.RenderMeshInstanced(renderParams, mesh, 0, positions) ; // CPU 7 GPU 3, T 15.4M V 10.3M
            
            
            Graphics.RenderMeshIndirect(renderParams, mesh, buffer, commands);
        }


        [BurstCompile]
        NativeArray<Matrix4x4> GeneratePositions (int w, int h)
        {
            NativeArray<Matrix4x4> re = new NativeArray<Matrix4x4>(w * h, Allocator.Persistent);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    re [y * w + x] = Matrix4x4.Translate(new float3(x, y, 0));
            return re;
        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}