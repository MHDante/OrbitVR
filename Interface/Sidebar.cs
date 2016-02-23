using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace OrbItProcs {
  public partial class Sidebar {
    private UserLevel _userLevel = UserLevel.Debug;
    public string ActiveGroupName;
    public bool CreatingGroup = false;
    public OrbIt game;
    EventHandler NotImplemented;

    public UserInterface ui;
    //public InspectorInfo ActiveInspectorParent;

    public Sidebar(UserInterface ui) {
      this.game = ui.game;
      //this.room = ui.game.room;
      this.ui = ui;
      NotImplemented = delegate {
                         Console.WriteLine("Not Implemented. Take a hike.");
                         //throw new NotImplementedException();
                       };
    }

    public Room room {
      get { return game.Room; }
    }

    public UserLevel userLevel {
      get { return _userLevel; }
      set {
        if (value == _userLevel) return;
        _userLevel = value;
      }
    }

    public Node ActiveDefaultNode {
      get {
        Group g = GetActiveGroup();
        if (g != null && g.defaultNode != null)
          return g.defaultNode;
        return null;
      }
    }

    public Group GetActiveGroup() {
      if (string.IsNullOrEmpty(ActiveGroupName)) return room.masterGroup;
      return room.masterGroup.FindGroup(ActiveGroupName);
    }

    public void Initialize() {}

    void btnDeleteGroup_Click(object sender, EventArgs e) {
      Group g = GetActiveGroup();
      if (g == null) return;
      if (g.Name.Equals("[G0]")) return;

      if (g.fullSet.Contains(room.targetNode)) room.targetNode = null;
      g.DeleteGroup();
    }


    public void BuildItemsPath(InspectorInfo item, List<InspectorInfo> itemspath) {
      InspectorInfo temp = item;
      itemspath.Insert(0, temp);
      while (temp.parentItem != null) {
        temp = temp.parentItem;
        itemspath.Insert(0, temp);
      }
    }

    public void ProcessConsoleCommand(String text) {
      text = text.Trim();

      if (text.Equals("")) {
        Console.WriteLine("No Command Provided");
        //consoletextbox.Text = "";
        return;
      }
      object currentObj = game.Room;


      List<String> args = text.Split(' ').ToList();
      String methodname;
      if (args.Count > 0) {
        methodname = args.ElementAt(0);
        args.RemoveAt(0);
      }
      else {
        Console.WriteLine("No Command Provided");
        return;
      }

      MethodInfo methinfo = currentObj.GetType().GetMethod(methodname);

      if (methinfo == null || methinfo.IsPrivate) {
        Console.WriteLine("Invalid method specification.");
        return;
      }

      ParameterInfo[] paraminfos = methinfo.GetParameters();

      int paramNum = paraminfos.Length;
      object[] finalargs = new object[paramNum];

      for (int i = 0; i < paramNum; i++) {
        Type ptype = paraminfos[i].ParameterType;
        if (i >= args.Count) {
          if (paraminfos[i].IsOptional) {
            finalargs[i] = Type.Missing;
            continue;
          }
          Console.WriteLine("Parameter Inconsistenc[ies].");
          return;
        }
        try {
          finalargs[i] = TypeDescriptor.GetConverter(ptype).ConvertFromInvariantString(args[i]);
        }
        catch (Exception e) {
          Console.WriteLine("Casting exception: " + e.Message);
          throw e;
        }
      }
      if (methinfo.IsStatic) currentObj = null;
      try {
        methinfo.Invoke(currentObj, finalargs);
      }
      catch (Exception e) {
        Console.WriteLine("Invoking exception: " + e.Message);
        throw e;
      }
    }

    public void Update() {}
  }
}