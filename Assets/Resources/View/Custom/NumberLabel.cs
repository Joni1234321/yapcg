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
                UpdateProgressBarTitle(value, _max);
            }
        }

      
        public ProgressBarControl() : base("View/Custom/numberlabel")
        {
            _changeLabel = Q<Label>("change");
            _titleLabel = Q<Label>("title");
            _progressBar = Q<ProgressBar>();

            Title = "Progress";
            Max = 100;
            Value = 20;
            Change = 4;
        }
    }
}
