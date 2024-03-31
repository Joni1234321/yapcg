using Unity.Entities;
using Unity.Entities.Content;
using UnityEngine;

namespace YAPCG.Engine.Components
{
    public class SharedCameraManaged : IComponentData
    {
        public Camera MainCamera;
    }
    public struct SharedRays : IComponentData
    {
        public Ray CameraMouseRay;
    }
    
    public struct SharedRandom : IComponentData
    {
        public Unity.Mathematics.Random Random;
    }
}