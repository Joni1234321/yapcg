using Unity.Mathematics;
using static YAPCG.Engine.Physics.Collisions.Raycast;

namespace YAPCG.Engine.Physics.Collisions
{
    public struct RayTriangle
    {
        public static bool CheckCollision(ray ray, TriangleCollection triangles, out hit hit)
        {
            float closest = float.MaxValue;
            hit = new hit { index = -1, point = ray.origin };
            
            int n = triangles.Positions.Length / 3;
            for (int i = 0; i < n; i++)
            {
                float3 v0 = triangles.Positions[i * 3];
                float3 v1 = triangles.Positions[i * 3 + 1];
                float3 v2 = triangles.Positions[i * 3 + 2];
                
                if (!MollerTromboneIntersection(ray, v0, v1, v2, out float t))
                    continue;
                if (t > closest)
                    continue;
                
                closest = t;
                hit.index = i;
            }
            
            hit.point = ray.origin + ray.direction * closest;
            return hit.index != -1;
        }

        
        // https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
        static bool MollerTromboneIntersection(Raycast.ray ray, float3 v0, float3 v1, float3 v2, out float t)
        {
            t = 0;
            
            // three equations
            // u, v, t for the boyocentric coordinates

            // this uses the carmers rule to solve the linear algebra
            // |Ai| / |A| where Ai is the ith row replaced by the constant of the equations
            float3 e1 = v1 - v0;
            float3 e2 = v2 - v0;
            float3 o  = ray.origin;
            float3 d  = ray.direction;

            // determinant can also be found with
            // |A B C| = -(A x C) . B = -(C x B).A
            // with this reuse (p and s) saves time
            float3 p = math.cross(d, e2);
            float det = math.dot(p, e1);
            
            // test if parallel
            if (det is > -float.Epsilon and < float.Epsilon)
                return false;

            float invDet = 1.0f / det;
            float3 s = o - v0;
            
            // calculate u and test bounds
            // u = P . T / det = p . s / det
            float u = invDet * math.dot(s, p);
            if (u is < 0 or > 1)
                return false;
            
            // calculate v and test bounds
            // v = q . d / det
            float3 q = math.cross(s, e1);
            float v = invDet * math.dot(d, q);
            if (v < 0 || u + v > 1)
                return false;
            
            // calculate intersection
            // t = q . e2 / det
            t = invDet * math.dot(q, e2);
            return t > float.Epsilon;
        }

    }
}