using System.Diagnostics;
using System.Globalization;
using UnityEngine.UIElements;

namespace YAPCG.Resources.View.Custom
{
    [UxmlElement]
    public partial class NumberLabel : CustomUI
    {

        private readonly Label _titleLabel, _valueLabel;

        private string _title, _value;

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

      
        public NumberLabel() : base("View/Custom/numberlabel")
        {
            _titleLabel = Q<Label>("title");
            _valueLabel = Q<Label>("value");

            Title = "Pop";
            Value = "200k";
        }
    }
}
