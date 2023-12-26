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

        
    }
}
