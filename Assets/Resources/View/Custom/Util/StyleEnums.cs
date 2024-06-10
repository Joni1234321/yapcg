using System;
using UnityEngine.UIElements;

namespace YAPCG.Resources.View.Custom.Util
{
    public static class StyleEnums
    {
        public static readonly StyleEnum<DisplayStyle> DISPLAY_HIDDEN = new() { value = DisplayStyle.None };
        public static readonly StyleEnum<DisplayStyle> DISPLAY_VISIBLE = new() { value = DisplayStyle.Flex };

        public static StyleEnum<DisplayStyle> ToDisplayStyle(this StyleClasses.Detailed detailed) => detailed switch
        {
            StyleClasses.Detailed.NotDetailed => DISPLAY_HIDDEN,
            StyleClasses.Detailed.Detailed => DISPLAY_VISIBLE,
            _ => throw new NotImplementedException()
        };
    }
}