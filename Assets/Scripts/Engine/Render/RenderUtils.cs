using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace YAPCG.Engine.Render
{
    public static class RenderUtils
    {
        public static GraphicsBuffer SetBufferData<T>(GraphicsBuffer buffer, NativeArray<T> data, int structSize)
            where T : unmanaged, IComponentData
        {
            buffer?.Release();
            buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, data.Length, structSize);
            buffer.SetData(data);
            return buffer;
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