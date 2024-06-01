using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Engine.Physics;
using YAPCG.Engine.Physics.Collisions;

namespace YAPCG.Application.UserInterface
{
    public class DebugScript : MonoBehaviour
    {
        
        private EntityManager _;
        private EntityQuery _bodyQuery, _rayQuery;
        public float Radius;

        void Awake()
        {
            _ = World.DefaultGameObjectInjectionWorld.EntityManager;
            _bodyQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Position, ScaleComponent>().Build(_);
            _rayQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<SharedRays>().Build(_); 
        }
        void OnDrawGizmos()
        {
            if (UnityEngine.Application.isPlaying)
                DrawSelectedSphere();
        }

        private void DrawSelectedSphere()
        {
            Raycast.ray ray = _rayQuery.GetSingleton<SharedRays>().CameraMouseRay; 

            //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.white, 0.1f);
            
            var positions = _bodyQuery.ToComponentDataArray<Position>(Allocator.Temp);
            var scales = _bodyQuery.ToComponentDataArray<ScaleComponent>(Allocator.Temp);
            SphereCollection spheres = new SphereCollection { Positions = positions.Reinterpret<Position, float3>(), Radius = scales.Reinterpret<ScaleComponent, float>() };
            
            if (!RaySphere.CheckCollision(ray, spheres, out Raycast.hit hit))
                return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ray.origin, hit.point);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(positions[hit.index].Value, Radius + 1);

        }
        
    }
    
    
}