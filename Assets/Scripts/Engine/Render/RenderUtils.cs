using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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