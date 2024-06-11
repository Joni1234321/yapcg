using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine.UIElements;
using YAPCG.Resources.View.Custom;

namespace YAPCG.Application.ControlRenderer
{
    public abstract class ControlRenderer<T> where T : CustomUI, new()
    {
        private const int MIN_N = 8;
        
        public List<T> Controls;

        private int _visible = 0;
        private VisualElement _root;
        public ControlRenderer (VisualElement root)
        {
            Controls = new ();
            _root = root;
            Grow();
        }
        
        public void Draw(int n)
        {
            while (Controls.Count < n)
                Grow();
            
            if (_visible < n)
                for (int i = _visible; i < n; i++)
                    Controls[i].visible = true;
            
            else if (_visible > n)
                for (int i = _visible; i < Controls.Count; i++)
                    Controls[i].visible = false;
        }

        private void Grow()
        {
            int n = Controls.Count;
            int next = math.max(MIN_N, n * 2);
            for (int i = n; i < next; i++)
                NewControl(i);
        }

        protected virtual T NewControl(int id)
        {
            T control = new() { visible = false, style = { position = new StyleEnum<Position>(Position.Absolute) }  };
            _root.Add(control);
            Controls.Add(control);
            return control;
        }

    }
}