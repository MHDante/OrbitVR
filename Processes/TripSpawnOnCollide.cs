using System;
using OrbitVR.Framework;

namespace OrbitVR.Processes {
  public class TripSpawnOnCollide : Process {
    public int colCount = 0;

    public Node triggerNode { get; set; }

    public TripSpawnOnCollide(Node node) : base() {
      this.triggerNode = node;

      OnCollision += CollisionEvent;
      triggerNode.body.OnCollisionStay += InvokeOnCollision;
    }


    public void CollisionEvent(Node me, Node it) {
      if (me == null) return;
      Console.WriteLine("event1");
      colCount++;
      if (colCount > 10) {
        //Collision -= CollisionEvent;
        me.body.OnCollisionStay -= InvokeOnCollision;
        Console.WriteLine("yes");
      }
      Node n1 = me.CreateClone(), n2 = me.CreateClone(), n3 = me.CreateClone();
      //Node.cloneNode(me, n1); // take params (...)
      //Node.cloneNode(me, n2);
      //Node.cloneNode(me, n3);
      //CollisionArgs["trigger"].Collided -= Collision;
      n1.body.OnCollisionStay -= InvokeOnCollision;
      n2.body.OnCollisionStay -= InvokeOnCollision;
      n3.body.OnCollisionStay -= InvokeOnCollision;
      n1.body.pos.X -= 150;
      n2.body.pos.X += 150;
      n3.body.pos.Y -= 150;


      Group g = room.MasterGroup.FindGroup("[G0]");
      g.IncludeEntity(n1);
      g.IncludeEntity(n2);
      g.IncludeEntity(n3);

      //n1.room.nodesToAdd.Enqueue(n1);
      //n1.room.nodesToAdd.Enqueue(n2);
      //n1.room.nodesToAdd.Enqueue(n3);

      //System.Console.WriteLine("Heyo");
    }
  }
}