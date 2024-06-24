using System.IO;

namespace SingularityGroup.HotReload.Editor.ProjectGeneration.Util
{
  internal static class StringUtils
  {
    public static string NormalizePath(this string path)
    {
      return path.Replace(Path.DirectorySeparatorChar == '\\'
        ? '/'
        : '\\', Path.DirectorySeparatorChar);
    }
  }
}