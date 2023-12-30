using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace YAPCG.Resources.View.Custom
{
    [UxmlElement]
    public partial class SlotControl : VisualElement
    {
        
        private string _label, _value;
        private Color _hue;
        
        private readonly Label _labelLabel, _valueLabel;
        private readonly List<VisualElement> _splits; 
        
        [UxmlAttribute]
        public string Label
        {
            get => _label;
            set
            {
                if (_label == value)
                    return;
                _label = value;
                _labelLabel.text = value;
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

        public SlotControl()
        {
            VisualElement body = UnityEngine.Resources.Load<VisualTreeAsset>($"View/Custom/Slots").CloneTree();
            Add(body);
            _labelLabel = body.Q<Label>("label");
            _valueLabel = body.Q<Label>("value");
            _splits = body.Query<VisualElement>("split").ToList();
            
            
            Label = "S";
            Value = 12.ToString();
            Hue = Color.HSVToRGB(0, 1, 1);
        }
    }
}
