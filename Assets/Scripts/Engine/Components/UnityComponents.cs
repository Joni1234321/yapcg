using Unity.Entities;
using Unity.Entities.Content;
using Unity.Mathematics;
using YAPCG.Engine.Physics.Collisions;

namespace YAPCG.Engine.Components
{
    public struct SharedRays : IComponentData
    {
        public Raycast.ray CameraMouseRay;
    }
    
    public struct SharedRandom : IComponentData
    {
        public Random Random;
    }

    [System.Serializable]
    public struct SharedSizes : IComponentData
    {
        public float HubRadius;
    }
}