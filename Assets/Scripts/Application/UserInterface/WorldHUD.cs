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

        public List<WorldPlanetNameControl> Controls;
        public int Visible = 0;
        
        public WorldHUD(VisualElement root)
        {
            Camera = Camera.main;
            Root = root;
            const int STARTING_COUNT = 8;
            Controls = new List<WorldPlanetNameControl>(STARTING_COUNT);
            for (int i = 0; i < STARTING_COUNT; i++)
                Add();
        }

        
        public void SetNames(NativeArray<FixedString64Bytes> names, NativeArray<float3> positions)
        {
            int n = names.Length;
            while (Controls.Count < n)
                DoubleSize();
            
            if (Visible < n)
                for (int i = Visible; i < n; i++)
                    Controls[i].visible = true;
            else if (Visible > n)
                for (int i = Visible; i < Controls.Count; i++)
                    Controls[i].visible = false;
            
            for (int i = 0; i < n; i++)
            {
                Controls[i].Title = names[i].ToString();
                float3 screen = Camera.WorldToScreenPoint(positions[i]);
                float w = Controls[i].resolvedStyle.width;
                float h = Controls[i].resolvedStyle.height;
                Controls[i].style.left = screen.x - w * 0.5f;
                Controls[i].style.bottom = screen.y - h * 0.5f - 50;
            }
        }

        void DoubleSize()
        {
            int n = Controls.Count;
            for (int i = 0; i < n; i++)
                Add();
        }

        void Add()
        {
            WorldPlanetNameControl control = new WorldPlanetNameControl { visible = true };
            control.style.position = new StyleEnum<Position>(Position.Absolute);
            Root.Add(control);
            Controls.Add(control);
        }
    }
    
}