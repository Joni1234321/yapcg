using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using YAPCG.Resources.View.Custom.Util;

namespace YAPCG.Resources.View.Custom
{
    [UxmlElement]
    public partial class WorldPlanetNameControl : CustomUI
    {

        private StyleClasses.Detailed _detailed;
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
        public StyleClasses.Detailed Detailed
        {
            get => _detailed;
            set
            {
                if (_detailed == value)
                    return;
                
                Body.RemoveFromClassList(_detailed.ToClass());
                Body.AddToClassList(value.ToClass());
                _detailsElement.style.display = value.ToDisplayStyle();
                
                _detailed = value;
            }
        }
 
        public WorldPlanetNameControl() : base("View/Custom/worldplanet")
        {
            _titleLabel = Q<Label>("title");
            _detailsElement = Q<VisualElement>("details");
            
            
            Detailed = StyleClasses.Detailed.Detailed; Detailed = StyleClasses.Detailed.NotDetailed;
            Title = ""; Title = "SUN";
            BorderColor = StyleClasses.BorderColor.Impossible; BorderColor = StyleClasses.BorderColor.Valid;
        }
    }
}
