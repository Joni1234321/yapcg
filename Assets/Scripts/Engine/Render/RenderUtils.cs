using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Engine.Components;

namespace YAPCG.Engine.Render
{
    public static class RenderUtils
    {
        public static Mesh GetMeshTemp ()
        { 
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh mesh = temp.GetComponent<MeshFilter>().mesh;
            UnityEngine.Object.Destroy(temp);
            return mesh;
        }


        public class PositionScaleAlternativeMaterialRender : IDisposable
        {
            private ShaderHelper<Position> _positions = new(Shader.PropertyToID("_Positions"), sizeof(float) * 3);
            private ShaderHelper<ScaleComponent> _scales = new(Shader.PropertyToID("_Scales"), sizeof(float));
            private ShaderHelper<FadeStartTimeComponent> _fadeStartTimes = new(Shader.PropertyToID("_FadeStartTimes"), sizeof(float));
            private ShaderHelper<AlternativeColorRatio> _alternativeColorRatios = new(Shader.PropertyToID("_AlternativeColorRatios"), sizeof(float));

            public void Render(EntityQuery query, Mesh mesh, Material material, Allocator allocator)
            {
                MaterialPropertyBlock matProps = new MaterialPropertyBlock();
                Bounds bounds = new Bounds(float3.zero, new float3(1000));
                int n = query.CalculateEntityCount();
                if (n == 0) 
                    return;
            
                _positions.UpdateBuffer(query, matProps, allocator);
                _scales.UpdateBuffer(query, matProps, allocator);
                _fadeStartTimes.UpdateBuffer(query, matProps, allocator);
                _alternativeColorRatios.UpdateBuffer(query, matProps, allocator);

                RenderParams renderParams = new RenderParams(material){ matProps = matProps, worldBounds = bounds };
                Graphics.RenderMeshPrimitives(renderParams, mesh, 0, n);
            }

            public void Dispose()
            {
                _positions.Dispose();
                _scales.Dispose();
                _alternativeColorRatios.Dispose();
                _fadeStartTimes.Dispose();
            }
        }
        
        public struct ShaderHelper<T> : IDisposable
            where T : unmanaged, IComponentData
        {
            public GraphicsBuffer Buffer;
            public readonly int ShaderProperty;
            public readonly int Size;

            public ShaderHelper(int shaderProperty, int size)
            {
                Size = size;
                ShaderProperty = shaderProperty;
                Buffer = null;
            }
            
            public void UpdateBuffer(EntityQuery query, MaterialPropertyBlock propertyBlock, Allocator worldUpdateAllocator)
            {
                NativeArray<T> data = query.ToComponentDataArray<T>(worldUpdateAllocator);
                Buffer?.Release();
                Buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, data.Length, Size);
                Buffer.SetData(data);
                propertyBlock.SetBuffer(ShaderProperty, Buffer);
            }

            public void Dispose()
            {
                Buffer?.Dispose();
            }
        }
    }
}