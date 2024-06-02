using UnityEngine.UIElements;

namespace YAPCG.Resources.View.Custom
{
    [UxmlElement]
    public partial class WorldPlanetNameControl : CustomUI
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

        public WorldPlanetNameControl() : base("View/Custom/worldplanet")
        {
            _titleLabel = Q<Label>("title");
            Title = "SUN";
        }
    }
}
