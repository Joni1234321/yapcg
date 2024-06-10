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