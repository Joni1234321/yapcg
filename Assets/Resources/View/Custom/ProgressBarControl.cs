using System.Globalization;
using UnityEngine.UIElements;

namespace YAPCG.Resources.View.Custom
{
    [UxmlElement]
    public partial class ProgressBarControl : CustomUI
    {

        private readonly Label _changeLabel, _titleLabel;
        private readonly ProgressBar _progressBar;

        private string _title;
        private float _max, _value, _change;

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
        public float Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;
                _value = value;
                _progressBar.value = value;
                
                UpdateProgressBarTitle(value, _max);
            }
        }

        [UxmlAttribute]
        public float Max
        {
            get => _max;
            set
            {
                if (_max == value)
                    return;
                _max = value;
                _progressBar.highValue = _max;
                UpdateProgressBarTitle(_value, value);
            }
        }
        
        [UxmlAttribute]
        public float Change
        {
            get => _change;
            set
            {
                if (_change == value)
                    return;
                _change = value;
                _changeLabel.text = value.ToString(CultureInfo.InvariantCulture);
            }   
        }

        void UpdateProgressBarTitle(float value, float max) => _progressBar.title = $"{value} / {max}";
        
        public ProgressBarControl() : base("View/Custom/progressbar")
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
