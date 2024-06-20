using System.Collections.Generic;
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
            if (_visible == n)
                return;

            while (Controls.Count < n)
                Grow();
            
            while (_visible < n)
                Controls[_visible++].visible = true;

            while (_visible > n)
                Controls[--_visible].visible = false;
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