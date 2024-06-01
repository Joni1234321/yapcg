using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace YAPCG.Resources.View.Custom
{
    [UxmlElement]
    public partial class SlotControl : CustomUI
    {
        
        private string _title;
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

        public SlotControl() : base("View/Custom/WorldPlanet")
        {
            _titleLabel = Q<Label>("title");
            Label = "SUN";
        }
    }
}
