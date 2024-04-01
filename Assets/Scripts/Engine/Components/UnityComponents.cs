using Unity.Entities;
using UnityEngine;
using YAPCG.Engine.Physics.Collisions;

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
        public Unity.Mathematics.Random Random;
    }
}