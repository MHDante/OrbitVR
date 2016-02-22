using System;
using System.Collections.Generic;
using System.IO;

namespace OrbItProcs {
  public class DiodeData {
    public DiodeData(Diode d) {
      this.start = d.start.toFloatArray();
      this.end = d.end.toFloatArray();
      this.orientation = d.parent.body.orient;
      this.tickets = d.maxTickets;
      this.isSemaphore = d.semaphore;
    }

    public DiodeData() {}
    public float[] start { get; set; }
    public float[] end { get; set; }
    public float orientation { get; set; }
    public int tickets { get; set; }
    public bool isSemaphore { get; set; }
  }

  public class LevelSave {
    public LevelSave() {}

    public LevelSave(Group group, int levelWidth, int levelHeight, string name) {
      Diodes = new List<DiodeData>();
      foreach (var n in group.room.masterGroup.fullSet) {
        if (n.HasComp<Diode>()) {
          Diodes.Add(new DiodeData(n.Comp<Diode>()));
        }
      }
      this.levelHeight = levelHeight;
      this.levelWidth = levelWidth;
      this.name = name;
      polygonVertices = new List<float[]>();
      polygonPositions = new List<float[]>();
      foreach (Node n in group.fullSet) {
        if (n.body.shape is Polygon) {
          Polygon p = (Polygon) n.body.shape;
          float[] verts = new float[p.vertexCount*2];
          for (int i = 0; i < p.vertexCount; i++) {
            verts[i*2] = p.vertices[i].X;
            verts[(i*2) + 1] = p.vertices[i].Y;
          }
          polygonVertices.Add(verts);
          polygonPositions.Add(new float[] {n.body.pos.X, n.body.pos.Y});
          //cover angle?
        }
      }
    }

    public int levelHeight { get; set; }
    public int levelWidth { get; set; }
    public List<float[]> polygonVertices { get; set; }
    public List<float[]> polygonPositions { get; set; }
    public string name { get; set; }
    public List<DiodeData> Diodes { get; set; }

    public override string ToString() {
      return name;
    }

    public static void SaveLevel(Group group, int levelWidth, int levelHeight, string name) {
      if (name.Equals("")) return;
      name = name.Trim();
      //string filename = "Presets//Nodes//" + name + ".xml";
      string filename = Assets.levelsFilepath + "/" + name + ".xml";
      Action completeSave = delegate {
                              LevelSave levelSave = new LevelSave(group, levelWidth, levelHeight, name);
                              OrbIt.game.serializer = new Polenter.Serialization.SharpSerializer();
                              OrbIt.game.serializer.Serialize(levelSave, filename);
                              //Assets.NodePresets.Add(serializenode);
                            };

      if (File.Exists(filename)) {
        //we must be overwriting, therefore don't update the live presetList
        //PopUp.Prompt("OverWrite?", "O/W?", delegate(bool c, object a) { if (c) { completeSave(); PopUp.Toast("Level '" + name + "' was overwritten."); } return true; });
      }
      else {
        //PopUp.Toast("Level Saved as " + name); completeSave();
      }
    }
  }
}