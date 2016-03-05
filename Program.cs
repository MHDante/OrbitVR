using System.Diagnostics.CodeAnalysis;
using OrbitVR.Processes;
using OrbitVR.UI;
using SharpDX;
using SharpDX.Toolkit;

namespace OrbitVR {
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
  [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
  internal static class DebugFlags
  {
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

  public class Program
    {
    public static void Main(){
      using (Tester orbit = new Tester())
        orbit.Run();
    }
  }


  internal class Tester : OrbIt {
    private Randomizer randomizer;
    private bool spawned;
    private Vector2 spawnPos;
    private int testTimer;

    protected override void Initialize() {
      base.Initialize();
      randomizer = Game.ProcessManager.GetProcess<Randomizer>();
    }

    protected override void Update(GameTime gameTime) {
      base.Update(gameTime);

      testTimer += Time.ElapsedGameTime.Milliseconds;
      if (testTimer > 1000) {
        spawnPos += Vector2.One*10;
        UserInterface.WorldMousePos = spawnPos;
        randomizer.SpawnSemiRandom();
        //randomizer.SpawnFullyRandom();
        testTimer = 0;
      }

      //if (testTimer < 1000) {
      //  testTimer = 1000;
      //  Node n = spawnNode(0, 0);
      //  n.addComponent<Lifetime>(true);
      //  n.Comp<Lifetime>().timeUntilDeath.value = 2000;
      //  n.Comp<Lifetime>().timeUntilDeath.enabled = true;
      //  n.addComponent<Tree>(true);
      //  //n.Comp<Tree>().branchStages = 1;
      //  n.addComponent<Shovel>(true);
      //}

      if (!spawned) {
        spawned = true;
        for (int i = 0; i < 500; i++) {
          Node n = Room.SpawnNode(0, 0);
        }
      }
    }
  }
}