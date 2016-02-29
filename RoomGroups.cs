using OrbitVR.Framework;

namespace OrbitVR {
  public class RoomGroups
  {
    private readonly Room _room;

    public Group General => _room.MasterGroup?.childGroups["General Groups"];

    public Group Preset => _room.MasterGroup?.childGroups["Preset Groups"];

    public Group Player => _room.MasterGroup?.childGroups["Player Group"];

    public Group Items => _room.MasterGroup?.childGroups["Item Group"];

    public Group Bullets => _room.MasterGroup?.childGroups["Bullet Group"];

    public Group Walls => _room.MasterGroup?.childGroups["Wall Group"];

    public RoomGroups(Room room)
    {
      _room = room;
    }
  }
}