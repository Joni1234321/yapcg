using UnityEngine;
using UnityEngine.UIElements;

namespace YAPCG.Resources.View.Custom
{
    [UxmlElement]
    public partial class WorldPlanetNameControl : CustomUI
    {
        
        private string _title;
        private short _hue = -1;

        private readonly VisualElement _root;
        private readonly Label _titleLabel;
        
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
        public short Hue
        {
            get => _hue;
            set
            {
                if (_hue == value)
                    return;
                _hue = value;
                var borderColor = new StyleColor(Color.HSVToRGB(value / 360f, 0.93f, 0.83f));
                _root.style.borderBottomColor = borderColor;
                _root.style.borderLeftColor = borderColor;
                _root.style.borderRightColor = borderColor;
                _root.style.borderTopColor = borderColor;
            }
        }

        public WorldPlanetNameControl() : base("View/Custom/worldplanet")
        {
            _titleLabel = Q<Label>("title");
            _root = Q<VisualElement>("world-planet");
            Title = "SUN";
            Hue = 120;
        }
    }
}
