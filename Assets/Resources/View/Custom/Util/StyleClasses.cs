using System;

namespace YAPCG.Resources.View.Custom.Util
{
    public static class StyleClasses
    {
        public enum BorderColor
        {
            Valid,
            Invalid,
            Impossible
        }
        
        public static string ToClass(this BorderColor borderColor) => borderColor switch
        {
            BorderColor.Valid => "border-valid",
            BorderColor.Invalid => "border-invalid",
            BorderColor.Impossible => "border-impossible",
            _ => throw new NotImplementedException()
        };

        public enum Opacity
        {
            None,
            Half,
            Full,
        }

        public static string ToClass(this Opacity opacity) => opacity switch
        {
            Opacity.None => "opacity-0",
            Opacity.Half => "opacity-50",
            Opacity.Full => "opacity-100",
            _ => throw new NotImplementedException()
        };


        public enum Transitions
        {
            Border,
            Opacity,
        }
        public static string ToClass(this Transitions transitions) => transitions switch
        {
            Transitions.Border => "transition-border",
            Transitions.Opacity => "transition-opacity",
            _ => throw new NotImplementedException()
        };
        
        public enum Detailed
        {
            NotDetailed,
            Detailed, 
        }

        public static string ToClass(this Detailed detailed) => detailed switch
        {
            Detailed.NotDetailed => "border-hidden",
            Detailed.Detailed => "border-white",
            _ => throw new NotImplementedException()
        };
    }
}