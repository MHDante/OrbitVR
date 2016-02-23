using System.Diagnostics.CodeAnalysis;

namespace OrbitVR {
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
  [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
  internal static class DebugFlags {
    //Dont update nodes outside grid
    public static bool skipOutsideGrid = false;
    //3D shape of the room.
    public static RenderShape renderShape = RenderShape.Cylinder;
    //Add polygon Colliders to room walls
    public static bool addRoomWalls = false;
    public static bool drawRoomBorder = true;
    public static bool drawCollisionGrid = false;
    public static bool drawAffectGrid = false;
  }
}