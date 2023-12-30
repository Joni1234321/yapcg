using UnityEngine.UIElements;

namespace YAPCG.Resources.View.Custom
{
    [UxmlElement]
    public partial class SlotControl : VisualElement
    {
        
        private string _label, _value;
        private readonly Label _nameLabel, _valueLabel;
        
        [UxmlAttribute]
        public string Label
        {
            get => _label;
            private set
            {
                if (_label == value)
                    return;
                _label = value;
                _nameLabel.name = value;
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


        public SlotControl()
        {
            VisualElement body = UnityEngine.Resources.Load<VisualTreeAsset>($"View/Custom/Slots").CloneTree();
            Add(body);
            _nameLabel = body.Q<Label>("label");
            _valueLabel = body.Q<Label>("value");
            Label = "S";
            Value = 12.ToString();
        }
    }
}
