using Unity.Entities;
using UnityEngine;
using YAPCG.Engine.Physics.Collisions;
using Random = Unity.Mathematics.Random;

namespace YAPCG.Engine.Components
{
    public class SharedCameraManaged : IComponentData
    {
        public Camera MainCamera;
    }
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