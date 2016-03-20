using System.Diagnostics.CodeAnalysis;
using OrbitVR.Components.AffectOthers;
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
    private Vector2R spawnPos;
    private double testTimer;

    protected override void Initialize() {
      base.Initialize();
      randomizer = Game.ProcessManager.GetProcess<Randomizer>();
    }

    
    protected override void UpdateAsync() {
      base.UpdateAsync();

      testTimer += Time.ElapsedGameTime.TotalMilliseconds;
      if (testTimer > 5000) {
        spawnPos += Vector2R.One*10;
        UserInterface.WorldMousePos = spawnPos;
        randomizer.SpawnSemiRandom();
        //randomizer.SpawnFullyRandom();
        testTimer = 0;
      }
      //if (testTimer > 5000) {
      //  
      //  Node n = Room.SpawnNode(20, 20);
      //  n.addComponent<Gravity>();
      //  //n.addComponent<Transfer>();
      //  n.body.velocity = new Vector2(1, 1) * 0.001f;
      //}

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
        for (int i = 0; i < 150; i++) {
          Node n = Room.SpawnNode(i, i);
          //n.addComponent<Gravity>();
        n.body.velocity  = new Vector2R(1,1)*0.001f;
        }

      }
    }
  }
}