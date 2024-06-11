using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;
using YAPCG.Application.UserInterface;
using YAPCG.Domain.NUTS;
using YAPCG.Engine.Common.DOTS;
using YAPCG.Resources.View.Custom;
using YAPCG.Resources.View.Custom.Util;

namespace YAPCG.Application.ControlRenderer
{
    public class WorldPlanetControlRenderer : ControlRenderer<WorldPlanetNameControl>
    {
        private EntityQuery claimSingletonQuery;

        public WorldPlanetControlRenderer(VisualElement root) : base(root)
        {
            claimSingletonQuery = new EntityQueryBuilder(Allocator.Temp).WithAllRW<Body.ActionClaim>().Build(World.DefaultGameObjectInjectionWorld.EntityManager);
        }

        private NativeArray<Entity> _bodies;
        public void Draw(NativeArray<Entity> entities, NativeArray<FixedString64Bytes> names, NativeArray<float3> positions, NativeArray<StyleClasses.BorderColor> borderColors, int detailed = -1)
        {
            _bodies = entities;
            int n = names.Length;
            Draw(n);
            
            for (int i = 0; i < n; i++)
            {
                Controls[i].Title = names[i].ToString();
                Controls[i].BorderColor = borderColors[i];
                Controls[i].Detailed = i == detailed ? StyleClasses.Detailed.Detailed : StyleClasses.Detailed.NotDetailed;
                float3 screen = HUD.Instance.Camera.WorldToScreenPoint(positions[i]);
                float w = Controls[i].resolvedStyle.width;
                float h = Controls[i].resolvedStyle.height;
                Controls[i].style.left = screen.x - w * 0.5f;
                Controls[i].style.bottom = screen.y - h * 0.5f - 50;
            }
        }

        protected override WorldPlanetNameControl NewControl(int id)
        {
            WorldPlanetNameControl control =  base.NewControl(id);
            control.ClaimButton.clicked += () =>
            {
                Entity body = _bodies[id];
                Body.ActionClaim actionClaim = new Body.ActionClaim { Body = body, OwnerID = Body.Owner.YOU_OWNER_ID };
                claimSingletonQuery.GetSingletonRW<Body.ActionClaim>().ValueRW = actionClaim;
            };
            return control;
        }
    }
}