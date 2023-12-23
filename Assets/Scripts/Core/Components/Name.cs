using Unity.Collections;
using Unity.Entities;

namespace YAPCG.Core.Components
{
    public struct Name : IComponentData
    {
        public FixedString128Bytes Value;
    }
}