using System.Collections.Generic;
using System.Linq;
using OrbitVR.Components.Meta;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Linkers {
  /// <summary>
  /// Use logic (circuit) gates to affect the flow of component affection control
  /// </summary>
  [Info(UserLevel.Advanced, "Use logic (circuit) gates to affect the flow of component affection control", CompType)]
  public class Flow : Component, ILinkable {
    public enum gate {
      None,
      AND,
      OR,
      NOT,
    }

    public const mtypes CompType = mtypes.draw; // | mtypes.exclusiveLinker;

    private bool _activated = false;

    /// <summary>
    /// This setting determines the output signal depending on the input signals of this node.
    /// </summary>
    [Info(UserLevel.Advanced, "This setting determines the output signal depending on the input signals of this node.")] private gate _gatetype = gate.AND;

    private HashSet<Node> _incoming = new HashSet<Node>();

    private HashSet<Node> _outgoing = new HashSet<Node>();

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    public HashSet<Node> outgoing {
      get { return _outgoing; }
      set { _outgoing = value; }
    }

    public HashSet<Node> incoming {
      get { return _incoming; }
      set { _incoming = value; }
    }

    /// <summary>
    /// Determines the state of the node. this is also the state that it will broadcast to other nodes.
    /// </summary>
    [Info(UserLevel.Advanced,
      "Determines the state of the node. this is also the state that it will broadcast to other nodes.")]
    public bool activated {
      get { return _activated; }
      set { _activated = value; }
    }

    public gate gatetype {
      get { return _gatetype; }
      set { _gatetype = value; }
    }

    public Flow() : this(null) {}

    public Flow(Node parent = null) {
      if (parent != null) {
        this.parent = parent;
      }

      //InitializeLists();
    }

    public Link link { get; set; }

    [Info(UserLevel.Developer)]
    public override bool active {
      get { return _active; }
      set {
        _active = value;
        if (!_active) {
          foreach (Node n in outgoing.ToList()) {
            outgoing.Remove(n);
            n.Comp<Flow>().incoming.Remove(parent);
          }
          foreach (Node n in incoming.ToList()) {
            incoming.Remove(n);
            n.Comp<Flow>().outgoing.Remove(parent);
          }
        }
      }
    }

    public override void AffectOther(Node other) {
      if (!active) {
        return;
      }
    }

    public override void Draw() {
      Color col;
      if (activated)
        col = Color.Green;
      else
        col = Color.Red;


      //spritebatch.Draw(parent.getTexture(), parent.body.pos * mapzoom, null, col, 0, parent.TextureCenter(), (parent.body.scale * mapzoom) * 1.2f, SpriteEffects.None, 0);
      room.Camera.Draw(parent.texture, parent.body.pos, parent.body.color, parent.body.scale*1.2f, Layers.Under2);

      foreach (Node receiver in outgoing) {
        /*
                Color tempcol;
                if (receiver.comps[comp.flow].gatetype == 2 && receiver.comps[comp.flow])
                {
                    //indexof wont work...
                }
                */

        room.Camera.DrawLine(parent.body.pos, receiver.body.pos, 2f, col, Layers.Under3);
        Vector2 center = (receiver.body.pos + parent.body.pos)/2;
        Vector2 perp = new Vector2(center.Y, -center.X);
        VMath.NormalizeSafe(ref perp);
        perp *= 10;
        //center += perp;
        room.Camera.DrawLine(center + perp, receiver.body.pos, 2f, col, Layers.Under3);
        room.Camera.DrawLine(center - perp, receiver.body.pos, 2f, col, Layers.Under3);


        //count++;
      }

      string gatestring = "";
      if ((int) gatetype > 0) gatestring = gatetype.ToString();

      //spriteBatch.Begin();

      //spritebatch.DrawString(room.game.font, gatestring, parent.body.pos * mapzoom, Color.White, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
      room.Camera.DrawStringWorld(gatestring, parent.body.pos, parent.body.color, scale: 0.5f);
    }

    public override void AfterCloning() {
      //if (!parent.comps.ContainsKey(comp.queuer)) parent.addComponent(comp.queuer, true);
      parent.addComponent(typeof (Lifetime), true);
    }

    public override void InitializeLists() {
      outgoing = new HashSet<Node>();
      incoming = new HashSet<Node>();
    }

    public override void AffectSelf() {
      if (gatetype == gate.AND)
        AND_Gate();
      else if (gatetype == gate.OR)
        OR_Gate();
      else if (gatetype == gate.NOT)
        NOT_Gate();
    }

    public void AND_Gate() {
      if (incoming.Count == 0) {
        //activated = false;
        return;
      }
      foreach (Node input in incoming) {
        if (input.HasComp<Flow>()) {
          if (!input.Comp<Flow>().activated) {
            activated = false;
            return;
          }
        }
      }
      activated = true;
    }

    public void OR_Gate() {
      if (incoming.Count == 0) {
        //activated = false;
        return;
      }
      foreach (Node input in incoming) {
        if (input.HasComp<Flow>()) {
          if (input.Comp<Flow>().activated) {
            activated = true;
            return;
          }
        }
      }
      activated = false;
    }

    public void NOT_Gate() {
      if (incoming.Count == 0) {
        activated = false;
        return;
      }
      foreach (Node input in incoming) {
        if (input.HasComp<Flow>()) {
          activated = !input.Comp<Flow>().activated;
          return;
        }
      }
      activated = false;
    }

    public void AddToOutgoing(Node node) {
      if (node != parent && node.HasComp<Flow>()) {
        if (outgoing.Contains(node)) {
          outgoing.Remove(node);
          node.Comp<Flow>().incoming.Remove(parent);
        }
        else {
          outgoing.Add(node);
          node.Comp<Flow>().incoming.Add(parent);
        }
      }
    }
  }
}