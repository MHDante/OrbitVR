using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OrbitVR.Components.AffectOthers;
using OrbitVR.Components.Movement;
using OrbitVR.Framework;
using OrbitVR.Processes;
using OrbitVR.PSMove;
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
    private Node selectNode;
    protected override void Initialize() {
      base.Initialize();
      selectNode = Room.SpawnNode(0, 0);
      selectNode.body.radius = 25;
      selectNode.body.velocity = Vector2R.Zero;
      selectNode.addComponent<Gravity>().multiplier = Utils.random.NextFloat(-.01f, .01f);
      selectNode.collision.active = false;
      selectNode.movement.maxVelocity.value = .1f;
      selectNode.movement.maxVelocity.enabled = true;

      randomizer = Game.ProcessManager.GetProcess<Randomizer>();
      
    }


    bool SphereRayCollision(Vector3 point, Vector3 dir, out Vector3 HitPt)
    {
      var center = Vector3.Zero;
      var radius = 100f;
      HitPt = Vector3.Zero;
      dir.Normalize();
      Vector3 m = point - center;
      float b = Vector3.Dot(m, dir);
      float c = Vector3.Dot(m, m) - radius * radius;

      // Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0) 
      if (c > 0.0f && b > 0.0f) return false;
      float discr = b * b - c;

      // A negative discriminant corresponds to ray missing sphere 
      if (discr < 0.0f) return false;

      // Ray now found to intersect sphere, compute smallest t value of intersection
      var t = -b - Math.Sqrt(discr);

      // If t is negative, ray started inside sphere so clamp t to zero 
      //if (t < 0.0f) t = 0.0f;
      HitPt = point + (float)t * dir;
      HitPt.Y *= -1;
      HitPt.Z *= -1;
      return true;
    }
    
    protected override void UpdateAsync() {



      base.UpdateAsync();

      var dir = Vector3.Transform(Vector3.ForwardLH, PsMoveController.transform.rotation);
      var pt = Vector3.Zero;//PsMoveController.transform.position;
      Vector3 HitPt;
      bool hit = SphereRayCollision(pt, dir, out HitPt);
      //Debug.WriteLine("|| " + HitPt);

      if (hit)
      {

        var rho = MathHelper.ToDegrees((float) Math.Atan2(HitPt.X,HitPt.Z));
        var phi = MathHelper.ToDegrees((float)Math.Acos(HitPt.Y/100));


        selectNode.body.pos.X = rho;
        selectNode.body.pos.Y = phi-90;
        Debug.WriteLine(rho + " : " + phi);

      }




     


      //testTimer += Time.ElapsedGameTime.TotalMilliseconds;
      //if (testTimer > 5000) {
      //  spawnPos += Vector2R.One*10;
      //  UserInterface.WorldMousePos = spawnPos;
      //  Node n = randomizer.CreateNode();
      //  n.body.radius = 5;
      //  //randomizer.SpawnFullyRandom();
      //  testTimer = 0;
      //}
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
        for (int i = 0; i < 10; i++) {
          Node n = Room.SpawnNode(Utils.random.Next(180), Utils.random.Next(180));
          n.body.radius = 50;
          
          //n.addComponent<Gravity>();
          n.body.velocity  = Vector2R.Zero;
          //; new Vector2R(Utils.random.NextFloat(1, 360),Utils.random.NextFloat(1, 360));
          //f.force = 0.05f;
          n.addComponent<Gravity>().multiplier = Utils.random.NextFloat(-.01f, .01f);
          n.collision.active = false;
          n.movement.maxVelocity.value = .1f;
          n.movement.maxVelocity.enabled = true;

        }

      }
    }
  }
}