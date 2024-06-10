using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using YAPCG.Resources.View.Custom;

namespace YAPCG.Application.UserInterface
{
    public class WorldHUD
    {
        public Camera Camera;
        public VisualElement Root;

        public List<WorldPlanetNameControl> WorldPlanetNameControls;
        public int Visible = 0;
        
        public WorldHUD(VisualElement root)
        {
            Camera = Camera.main;
            Root = root;
            const int STARTING_COUNT = 8;
            WorldPlanetNameControls = new List<WorldPlanetNameControl>(STARTING_COUNT);
            for (int i = 0; i < STARTING_COUNT; i++)
                Add();
        }

        
        public void DrawPlanetNames(NativeArray<FixedString64Bytes> names, NativeArray<float3> positions, NativeArray<StyleClasses.BorderColor> borderColors)
        {
            int n = names.Length;
            while (WorldPlanetNameControls.Count < n)
                DoubleSize();
            
            if (Visible < n)
                for (int i = Visible; i < n; i++)
                    WorldPlanetNameControls[i].visible = true;
            else if (Visible > n)
                for (int i = Visible; i < WorldPlanetNameControls.Count; i++)
                    WorldPlanetNameControls[i].visible = false;
            
            for (int i = 0; i < n; i++)
            {
                WorldPlanetNameControls[i].Title = names[i].ToString();
                WorldPlanetNameControls[i].BorderColor = borderColors[i];
                float3 screen = Camera.WorldToScreenPoint(positions[i]);
                float w = WorldPlanetNameControls[i].resolvedStyle.width;
                float h = WorldPlanetNameControls[i].resolvedStyle.height;
                WorldPlanetNameControls[i].style.left = screen.x - w * 0.5f;
                WorldPlanetNameControls[i].style.bottom = screen.y - h * 0.5f - 50;
            }
        }

        void DoubleSize()
        {
            int n = WorldPlanetNameControls.Count;
            for (int i = 0; i < n; i++)
                Add();
        }

        void Add()
        {
            WorldPlanetNameControl control = new WorldPlanetNameControl { visible = true, style = { position = new StyleEnum<Position>(Position.Absolute) }  };
            Root.Add(control);
            WorldPlanetNameControls.Add(control);
        }
    }
    
}