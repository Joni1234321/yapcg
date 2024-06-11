using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;
using YAPCG.Application.UserInterface;
using YAPCG.Resources.View.Custom;
using YAPCG.Resources.View.Custom.Util;

namespace YAPCG.Application.ControlRenderer
{
    public class WorldPlanetControlRenderer : ControlRenderer<WorldPlanetNameControl>
    {
        public WorldPlanetControlRenderer(VisualElement root) : base(root)
        {
        }

        private NativeArray<Entity> _entities;
        public void Draw(NativeArray<Entity> entities, NativeArray<FixedString64Bytes> names, NativeArray<float3> positions, NativeArray<StyleClasses.BorderColor> borderColors, int detailed = -1)
        {
            _entities = entities;
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
            var control =  base.NewControl(id);
            return control;
        }
    }
}