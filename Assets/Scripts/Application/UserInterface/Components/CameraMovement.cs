using Unity.Entities;
using Unity.Mathematics;

namespace YAPCG.Application.UserInterface.Components
{
    public struct CameraMovement : IComponentData
    {
        public bool DefaultView;
        public float3 DefaultCameraPosition;
        public quaternion DefaultCameraRotation;

        public float3 LookAtPosition;
    }
}