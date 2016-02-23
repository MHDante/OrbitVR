using System.Diagnostics.CodeAnalysis;

namespace OrbitVR
{
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
  [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
  internal static class DebugFlags {
    public static bool skipOutsideGrid = false;
    public static RenderShape renderShape = RenderShape.Cylinder;
  }
}
