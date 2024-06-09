using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace YAPCG.Resources.View.Custom
{
    [UxmlElement]
    public partial class SlotControl : CustomUI
    {
        
        private string _title, _value;
        private Color _hue;
        
        private readonly Label _titleLabel, _valueLabel;
        private readonly List<VisualElement> _splits; 
        
        [UxmlAttribute]
        public string Title
        {
            get => _title;
            set
            {
                if (_title == value)
                    return;
                _title = value;
                _titleLabel.text = value;
            }
        }
        
        [UxmlAttribute]
        public string Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                _value = value;
                _valueLabel.text = value;
            }
        }

        [UxmlAttribute]
        public Color Hue
        {
            get => _hue;
            set
            {
                if (_hue == value)
                    return;

                _hue = value;
                
                Color.RGBToHSV(value, out float hue, out float sat, out float val);
                const float LOW_SV = 0.34f, HIGH_SV = 0.7f;
                for (int i = 0; i < _splits.Count; i++)
                {
                    float t = (float)i / _splits.Count;
                    float sv = math.lerp(LOW_SV, HIGH_SV, t);
                    _splits[i].style.backgroundColor = Color.HSVToRGB(hue, sv, sv - .1f);
                }
            }
        }

        public SlotControl() : base("View/Custom/Slots")
        {
            _titleLabel = Q<Label>("title");
            _valueLabel = Q<Label>("value");
            _splits = Query<VisualElement>("split").ToList();
            
            
            Title = "S";
            Value = 12.ToString();
            Hue = Color.HSVToRGB(0, 1, 1);
        }
    }
}
