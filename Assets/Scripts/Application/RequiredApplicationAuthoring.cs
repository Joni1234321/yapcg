using Unity.Entities;
using UnityEngine;

namespace YAPCG.Application
{
    public class RequiredApplicationAuthoring : MonoBehaviour
    {
        public ApplicationSettings ApplicationSettings;
        private class Baker : Baker<RequiredApplicationAuthoring>
        {
            public override void Bake(RequiredApplicationAuthoring authoring)
            {
                Entity e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, authoring.ApplicationSettings);
            }
        }
    }
    
    [System.Serializable]
    public struct ApplicationSettings : IComponentData
    {
        public bool DisableUIRender, DisableBodyRender;
    }
}