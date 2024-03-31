using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Engine.Components;
using YAPCG.Engine.Physics;
using YAPCG.Engine.Physics.Ray;

namespace YAPCG.UI
{
    public class DebugScript : MonoBehaviour
    {
        
        private EntityManager _;
        private EntityQuery _positionsQuery, _rayQuery;
        public float Radius;

        void Awake()
        {
            _ = World.DefaultGameObjectInjectionWorld.EntityManager;
            _positionsQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<Position>().Build(_);
            _rayQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<SharedRays>().Build(_);
        }
        void OnDrawGizmos()
        {
            if (Application.isPlaying)
                DrawSelectedSphere();
        }

        private void DrawSelectedSphere()
        {
            Ray ray = _rayQuery.GetSingleton<SharedRays>().CameraMouseRay; 

            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.white, 0.1f);
            
            var positions = _positionsQuery.ToComponentDataArray<Position>(Allocator.Temp);
            SphereCollection spheres = new SphereCollection { Positions = positions.Reinterpret<Position, float3>(), Radius = Radius };
            Raycast.Hit hit = Raycast.CollisionSphere(ray.origin, ray.direction, spheres);
            
            if (hit.hit == -1) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ray.origin, ray.GetPoint(hit.t));
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(positions[hit.hit].Value, Radius + 1);

        }
    }
}