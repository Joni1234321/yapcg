using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine.UIElements;
using YAPCG.Domain.NUTS;
using YAPCG.Resources.View.Custom;
using YAPCG.Resources.View.Custom.Util;

namespace YAPCG.Application.ControlRenderer
{
    public class WorldPlanetControlRenderer : ControlRenderer<WorldPlanetControl>
    {
        private EntityQuery _claimSingletonQuery;

        public WorldPlanetControlRenderer(VisualElement root) : base(root)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                return;
#endif
            _claimSingletonQuery = new EntityQueryBuilder(Allocator.Temp).WithAllRW<Body.ActionClaim>().Build(World.DefaultGameObjectInjectionWorld.EntityManager);
        }

        private NativeArray<Entity> _bodies;
        public void Draw(NativeArray<Entity> entities, NativeArray<FixedString64Bytes> names, NativeArray<StyleClasses.BorderColor> borderColors, NativeArray<float3> positions, int detailed = -1)
        {
            _bodies = entities;
            int n = names.Length;
            Draw(n);
            
            for (int i = 0; i < n; i++)
            {
                Controls[i].Title = names[i].ToString();
                Controls[i].BorderColor = borderColors[i];
                Controls[i].Detailed = i == detailed ? StyleClasses.Detailed.Detailed : StyleClasses.Detailed.NotDetailed;
                float w = Controls[i].resolvedStyle.width;
                float h = Controls[i].resolvedStyle.height;
                Controls[i].style.left = positions[i].x - w * 0.5f;
                Controls[i].style.bottom = positions[i].y - h * 0.5f - 50; // TODO: Calculate the visible
            }
        }

        protected override WorldPlanetControl NewControl(int id)
        {
            
            WorldPlanetControl control =  base.NewControl(id);
            control.ClaimButton.clicked += () =>
            {
                Entity body = _bodies[id];
                Body.ActionClaim actionClaim = new Body.ActionClaim { Body = body, OwnerID = Body.Owner.YOU_OWNER_ID };
                _claimSingletonQuery.GetSingletonRW<Body.ActionClaim>().ValueRW = actionClaim;
            };
            return control;
        }
    }
}