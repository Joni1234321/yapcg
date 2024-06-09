using System;

namespace YAPCG.Resources.View.Custom
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
    }
}