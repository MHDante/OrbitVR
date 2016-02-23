using OrbitVR.Interface;
using OrbitVR.Processes;
using SharpDX;
using SharpDX.Toolkit;

namespace OrbitVR {
  internal class Tester : OrbIt {
    private Randomizer randomizer;
    private int testTimer;
    private bool spawned;
    private Vector2 spawnPos;

    protected override void Initialize() {
      base.Initialize();
      randomizer = Game.ProcessManager.GetProcess<Randomizer>();
    }
    
    protected override void Update(GameTime gt) {
      base.Update(gt);

      testTimer += gt.ElapsedGameTime.Milliseconds;
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
        for (int i = 0; i < 500; i++)
        {
          Node n = Room.SpawnNode(0, 0);
        }
      }
    }
  }
}