using UnityEngine;
using UnityEngine.UIElements;

namespace YAPCG.Resources.View.Custom
{
    [UxmlElement]
    public partial class WorldPlanetNameControl : CustomUI
    {

        private bool _detailed;
        private string _title;
        private StyleClasses.BorderColor _borderColor;

        private readonly VisualElement _detailsElement;
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
        public StyleClasses.BorderColor BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor == value)
                    return;
                
                _titleLabel.RemoveFromClassList(_borderColor.ToClass());
                _titleLabel.AddToClassList(value.ToClass());
                _borderColor = value;
            }
        }
        
        
        [UxmlAttribute]
        public bool Detailed
        {
            get => _detailed;
            set
            {
                if (_detailed == value)
                    return;
                _detailsElement.visible = value;
            }
        }

        public WorldPlanetNameControl() : base("View/Custom/worldplanet")
        {
            _titleLabel = Q<Label>("title");
            _detailsElement = Q<VisualElement>("details");
            Title = "SUN";
            _titleLabel.AddToClassList(((StyleClasses.BorderColor)0).ToClass());
        }
    }
}
