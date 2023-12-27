using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace YAPCG.UI
{
    [UxmlElement]
    public partial  class SlotControl : VisualElement
    {
        [UxmlAttribute]
        public string Value { get; private set; }

        [UxmlAttribute]
        public string Label { get; private set; }

        [UxmlAttribute]
        public VisualTreeAsset myTemplate { get; set; }


        
        public SlotControl()
        {
            Value = "42";
            Label = "Fisk";
            if (myTemplate != null)
                Add(myTemplate.Instantiate());
        }
    }
}
