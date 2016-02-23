using OrbitVR.Processes;
using SharpDX.Toolkit;

namespace OrbitVR {
  internal class Tester : OrbIt {
    private Randomizer randomizer;
    private int testTimer;

    protected override void Initialize() {
      base.Initialize();
      randomizer = Room.processManager.GetProcess<Randomizer>();
    }

    protected override void Update(GameTime gt) {
      base.Update(gt);

      testTimer += gt.ElapsedGameTime.Milliseconds;
      //if (testTimer > 1000) {
      //  spawnPos += Vector2.One*10;
      //  randomizer.SpawnSemiRandom();
      //  //randomizer.SpawnFullyRandom();
      //  testTimer = 0;
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

      if (testTimer < 1000)
      {
        testTimer = 1000;
        for (int i = 0; i < 500; i++)
        {
          Node n = Room.spawnNode(0, 0);
        }
      }
    }
  }
}