using System.Drawing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace YAPCG.UI
{
    public partial struct UIRenderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
               
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }
    }

    [InternalBufferCapacity(0)]
    public struct ListOfText : IComponentData
    {
        public FixedString128Bytes Text;
        public float EndOfLife;
    }
}