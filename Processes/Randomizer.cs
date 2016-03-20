using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OrbitVR.Components.Essential;
using OrbitVR.Components.Meta;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Processes {
  public class Randomizer : Process {
    int queuePos = 0;
    public Queue<Group> savedGroups = new Queue<Group>();
    //public Queue<Dictionary<dynamic, dynamic>> savedDicts = new Queue<Dictionary<dynamic, dynamic>>();
    public Queue<Node> savedNodes = new Queue<Node>();

    public Randomizer()
      : base() {
      addProcessKeyAction("spawnsemrandom", KeyCodes.LeftClick, OnPress: SpawnSemiRandom);
      addProcessKeyAction("spawnfullyrandom", KeyCodes.MiddleClick, OnPress: SpawnFullyRandom);
      addProcessKeyAction("plus", KeyCodes.OemPlus, OnPress: Plus);
      addProcessKeyAction("minus", KeyCodes.OemMinus, OnPress: Minus);
      addProcessKeyAction("spawnfromqueue", KeyCodes.RightClick, OnPress: SpawnFromQueue);
    }

    public void SpawnFromQueue() {
      if (queuePos >= savedNodes.Count) return;
      Node saved = savedNodes.ElementAt(savedNodes.Count - queuePos - 1);
      Group g = savedGroups.ElementAt(savedNodes.Count - queuePos - 1);
      Node n = room.SpawnNode(saved.CreateClone(saved.room), lifetime: 5000, g: g);
      n.body.pos = UserInterface.WorldMousePos;
    }

    public void Plus() {
      queuePos = Math.Min(savedNodes.Count - 1, queuePos + 1);
    }

    public void Minus() {
      queuePos = Math.Max(0, queuePos - 1);
    }

    public void SpawnSemiRandom() {
      CreateNode();
    }

    public void SpawnFullyRandom() {
      Node n = CreateNode();
      if (n != null) {
        foreach (var c in n.comps.Values.ToList()) {
          if (c.GetType() == typeof (Delegator)) continue;
          if (c.GetType() == typeof (Modifier)) continue;
          RandomizeObject(c);
        }
      }
      RandomizeObject(n.body);
      n.body.pos = UserInterface.WorldMousePos;
    }


    public Node CreateNode() {
      Vector2R pos = UserInterface.WorldMousePos;

      //new node(s)
      Dictionary<dynamic, dynamic> userP = new Dictionary<dynamic, dynamic>() {
        {nodeE.position, pos},
      };
      HashSet<Type> comps = new HashSet<Type>() {typeof (BasicDraw), typeof (Movement), typeof (Lifetime)};
      List<Type> allComps = new List<Type>();
      foreach (Type c in Component.compTypes) {
        allComps.Add(c);
      }

      int total = allComps.Count - comps.Count;


      Random random = Utils.random;
      int compsToAdd = random.Next(total);

      int a = 21*21;
      int randLog = random.Next(a);
      int root = (int) Math.Ceiling(Math.Sqrt(randLog));
      root = 21 - root;
      compsToAdd = root;

      //System.Console.WriteLine(compsToAdd + " " + root);

      int counter = 0;
      while (compsToAdd > 0) {
        if (counter++ > allComps.Count) break;
        int compNum = random.Next(allComps.Count - 1);
        Type newcomp = allComps[compNum];

        //if (newcomp == typeof(Tree)) continue; //test

        if (comps.Contains(newcomp)) {
          continue;
        }
        comps.Add(newcomp);
        compsToAdd--;
      }

      foreach (Type c in comps) {
        userP.Add(c, true);
      }


      Node n = room.SpawnNode(userP, blank: true, lifetime: 5000);
      if (n != null) {
        foreach (Type t in n.comps.Keys.ToList()) {
          if ((n.comps[t].compType & mtypes.item) == mtypes.item) {
            n.RemoveComponent(t);
          }
        }

        //savedDicts.Enqueue(userP);
        savedNodes.Enqueue(n);
        Group p = room.MasterGroup.childGroups["Link Groups"];
        var g = new Group(room, n, n.name);
        p.AddGroup(g);
        //p.AddGroup(g.Name, g);
        //OrbIt.ui.sidebar.UpdateGroupComboBoxes();
        savedGroups.Enqueue(g);
      }
      return n;
      //ListBox lst = Game1.ui.sidebar.lstMain;
      //Node newNode = (Node)lst.Items.ElementAt(lst.ItemIndex + 1);
      //System.Console.WriteLine(newNode.name);
    }

    public static void RandomizeObject(object o) {
      Type type = o.GetType();
      //properties
      List<PropertyInfo> propertyInfos;
      propertyInfos =
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();
      foreach (var pinfo in propertyInfos) {
        FPInfo fpinfo = new FPInfo(pinfo);
        ApplyFPInfo(o, fpinfo);
      }
      //fields
      List<FieldInfo> fieldInfos;
      fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();
      foreach (var finfo in fieldInfos) {
        FPInfo fpinfo = new FPInfo(finfo);
        ApplyFPInfo(o, fpinfo);
      }
    }

    private static void ApplyFPInfo(object o, FPInfo info) {
      Type ftype = info.FPType;
      Random rand = Utils.random;

      if (ftype == typeof (int)) {
        int a = rand.Next(500);
        info.SetValue(a, o);
      }
      else if (ftype == typeof (float)) {
        if (info.Name.ToLower().Contains("scale")) return;
        float a = (float) rand.NextDouble()*500f;
        info.SetValue(a, o);
      }
      else if (ftype == typeof (bool)) {
        int a = rand.Next(100);
        bool b = a%2 == 0 ? true : false;
        info.SetValue(b, o);
      }
      else if (ftype.IsEnum) {
        if (ftype == typeof (mtypes)) return;
        ArrayList list = new ArrayList(Enum.GetValues(ftype));
        int a = rand.Next(list.Count);
        info.SetValue(list[a], o);
      }
    }
  }
}