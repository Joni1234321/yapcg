using Unity.Collections;
using Unity.Entities;

namespace YAPCG.Engine.Components
{
    public struct Name : IComponentData
    {
        public FixedString64Bytes Value;
    }
}