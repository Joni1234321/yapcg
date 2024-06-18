using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using YAPCG.Application.Input;
using YAPCG.Application.UserInterface.Components;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Components;
using YAPCG.Engine.DebugDrawer;
using YAPCG.Engine.Physics;
using YAPCG.Engine.Physics.Collisions;
using YAPCG.Engine.SystemGroups;
using YAPCG.Engine.Time.Components;

namespace YAPCG.Application.UserInterface.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.Default)] 
    [UpdateInGroup(typeof(RenderSystemGroup), OrderFirst = true)]
    public partial struct ActionSystem : ISystem
    {
        private EntityQuery _levelQuery;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TickSpeedLevel>();
            state.RequireForUpdate<TickSpeed>();
            state.RequireForUpdate<SharedSizes>();
            state.RequireForUpdate<SharedRays>();
            _levelQuery = SystemAPI.QueryBuilder().WithAll<LevelQuad>().Build();
            
            state.EntityManager.CreateSingleton(new FocusedBody { Selected = Entity.Null });

            state.RequireForUpdate<ActionInput>(); 
            state.RequireForUpdate<FocusedBody>();
        }

        private int NegativeMod(int x, int m) => (x % m + m) % m;

        private int BodyEntityToIndex(Entity body, NativeArray<Entity> bodies)
        {
            for (int i = 0; i < bodies.Length; i++)
                if (bodies[i] == body)
                    return i;
            return -1;
        }
        
        public void OnUpdate(ref SystemState state)
        {
            ActionInput actionInput = SystemAPI.GetSingleton<ActionInput>();

            Raycast.ray ray = SystemAPI.GetSingleton<SharedRays>().CameraMouseRay;
            
            if (actionInput.ShouldBuildHub)
                BuildBodyOnMouse(ray, ref state);

            EntityQuery query = SystemAPI.QueryBuilder().WithAll<Body.BodyTag, Position, ScaleComponent>().Build();
            NativeArray<Entity> bodies = query.ToEntityArray(state.WorldUpdateAllocator);
            
            NativeArray<float3> positions = query.ToComponentDataArray<Position>(state.WorldUpdateAllocator).Reinterpret<Position, float3>();
            NativeArray<float> sizes = query.ToComponentDataArray<ScaleComponent>(state.WorldUpdateAllocator).Reinterpret<ScaleComponent, float>();
            int hoveredIndex = GetHoverBodyIndex(ray, positions, sizes);
            
            Entity hovered = hoveredIndex != -1 ? bodies[hoveredIndex] : Entity.Null;

            RefRW<FocusedBody> focusedBody = SystemAPI.GetSingletonRW<FocusedBody>();
            Entity selected = focusedBody.ValueRO.Selected;
            int selectedIndex = BodyEntityToIndex(selected, bodies);

            if (actionInput.Next)
                selectedIndex = NegativeMod(selectedIndex + 1, bodies.Length);

            if (actionInput.Previous)
                selectedIndex = NegativeMod(selectedIndex - 1, bodies.Length);

            if (bodies.Length != 0 && selectedIndex > 0 && selectedIndex < bodies.Length)
                selected = bodies[selectedIndex];
            
            
            if (actionInput.LeftClickSelectBody)
                selected = hovered;
            
            if (actionInput.DeselectBody)
                selected = Entity.Null;
            
            // Hovered
            if (focusedBody.ValueRO.Hovered != Entity.Null) 
                SystemAPI.SetComponent(focusedBody.ValueRO.Hovered, AlternativeColorRatio.Nothing);

            if (hovered != Entity.Null)
                SystemAPI.SetComponent(hovered, AlternativeColorRatio.Hovered);
            
            focusedBody.ValueRW.Hovered = hovered;

            // Selected
            if (focusedBody.ValueRO.Selected != Entity.Null) 
                SystemAPI.SetComponent(focusedBody.ValueRO.Selected, AlternativeColorRatio.Nothing);

            if (selected != Entity.Null)
               SystemAPI.SetComponent(selected, AlternativeColorRatio.Selected);

            focusedBody.ValueRW.Selected = selected;

            TimeControls(actionInput, ref state);
        }

        void TimeControls(in ActionInput actionInput, ref SystemState state)
        {
            TickSpeedLevel speedLevel = SystemAPI.GetSingleton<TickSpeedLevel>();
            
            if (actionInput.SpeedIncrease)
                speedLevel.Level++;
            if (actionInput.SpeedDecrease)
                speedLevel.Level--;
            if (actionInput.SpeedPause)
                speedLevel.Paused = !speedLevel.Paused;

            if (actionInput is { SpeedPause: false, SpeedIncrease: false, SpeedDecrease: false }) 
                return;
            
            var buffer = SystemAPI.GetSingletonBuffer<TickSpeedLevels>(true);
            speedLevel.Level = math.clamp(speedLevel.Level, 0, buffer.Length - 1);
            SystemAPI.SetSingleton(speedLevel);
            float speed = speedLevel.Paused ? 0 : buffer[speedLevel.Level].Speed;
            SystemAPI.SetSingleton(new TickSpeed { SpeedUp = speed });

        }
        
        [BurstCompile]
        int GetHoverBodyIndex(Raycast.ray ray, NativeArray<float3> positions, NativeArray<float> sizes)
        {
            SphereCollection spheres = new SphereCollection
            {
                Positions = positions, 
                Radius = sizes
            };
            
            if (!RaySphere.CheckCollision(ray, spheres, out Raycast.hit hit))
                return -1;

            return hit.index;
        }

        
        [BurstCompile]
        bool BuildBodyOnMouse(Raycast.ray ray, ref SystemState state)
        {
            TriangleCollection triangles = new TriangleCollection { Positions = GetLevelTriangles(ref state) };

            if (!RayTriangle.CheckCollision(ray, triangles, out Raycast.hit hit))
                return false;
            
            DebugDrawer.DrawTriangles(triangles);
            DebugDrawer.DrawRaycastHit(ray, hit);
 
            SystemAPI.GetSingletonBuffer<Hub.HubFactoryParams>(false).Add(new Hub.HubFactoryParams
            {
                Position = hit.point,  
                Size = Hub.Size.Medium,
            });
            
            return true;
        }
        
        [BurstCompile]
        NativeArray<float3> GetLevelTriangles(ref SystemState state)
        {
            NativeArray<LevelQuad> quads = _levelQuery.ToComponentDataArray<LevelQuad>(state.WorldUpdateAllocator);
            NativeArray<float3> positions = new NativeArray<float3>(6, state.WorldUpdateAllocator);
            for(int i = 0; i < quads.Length; i++)
            {
                LevelQuad quad = quads[i];
                int o = i * 6;
                
                float x0 = quad.Position.x - quad.Size.x;
                float x1 = quad.Position.x + quad.Size.x;
                float y0 = quad.Position.y - quad.Size.y;
                float y1 = quad.Position.y + quad.Size.y;
                
                float3 v00 = new float3(x0, 0, y0);
                float3 v01 = new float3(x0, 0, y1);
                float3 v11 = new float3(x1, 0, y1);
                float3 v10 = new float3(x1, 0, y0);

                positions[o + 0] = v00;
                positions[o + 1] = v11;
                positions[o + 2] = v10;
                
                positions[o + 3] = v00;
                positions[o + 4] = v01;
                positions[o + 5] = v11;
            }

            return positions;
        }

        


    }
}