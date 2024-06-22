using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using YAPCG.Engine.Physics;
using YAPCG.Engine.Physics.Collisions;

namespace YAPCG.Engine.DebugDrawer
{
    public static class DebugDrawer
    {
        public static void DrawTriangles(TriangleCollection triangles)
        {
            int n = triangles.Positions.Length / 3;
            for (int i = 0; i < n; i++)
            {
                float3 v1 = triangles.Positions[i * 3 + 0];
                float3 v2 = triangles.Positions[i * 3 + 1];
                float3 v3 = triangles.Positions[i * 3 + 2];
                Debug.DrawLine(v1, v2, Color.green, 1);
                Debug.DrawLine(v2, v3, Color.yellow, 1);
                Debug.DrawLine(v3, v1, Color.red, 1);
            }
        }

        public static void DrawRaycastHit(Raycast.ray ray, Raycast.hit hit, Color color = default)
        {
            if (color == default)
                color = Color.black;
            Debug.DrawLine(ray.origin, hit.point, color);
        } 
    }
}