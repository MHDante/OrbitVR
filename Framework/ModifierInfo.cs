using System;
using System.Collections.Generic;

namespace OrbitVR.Framework {
  public delegate void ModifierDelegate(Dictionary<string, dynamic> args, ModifierInfo modifierInfo);

  /// <summary>
  /// A data class to drive the Modifier component
  /// </summary>
  public class ModifierInfo {
    private Dictionary<string, dynamic> _args = new Dictionary<string, dynamic>();

    private string _delegateName = "";
    /*
        public Dictionary<string, Tuple<FPInfo, object>> _fpInfos = new Dictionary<string, Tuple<FPInfo, object>>();
        public Dictionary<string, Tuple<FPInfo, object>> fpInfos
        {
            get { return _fpInfos; }
            set { _fpInfos = value; }
        }
        */
    private Dictionary<string, FPInfo> _fpInfos = new Dictionary<string, FPInfo>();
    private Dictionary<string, object> _fpInfosObj = new Dictionary<string, object>();

    public ModifierDelegate modifierDelegate; //will not be serialzied, therefore anon methods lost.
    /*
        public ModifierDelegate modifierDelegate
        {
            get { return _modifierDelegate; }
            set { _modifierDelegate = value; }
        }
        //*/

    public ModifierInfo() {}

    public ModifierInfo(Dictionary<string, FPInfo> fpInfos,
                        Dictionary<string, object> fpInfosObj,
                        Dictionary<string, dynamic> args,
                        ModifierDelegate modifierDelegate) {
      this.fpInfos = fpInfos;
      this.fpInfosObj = fpInfosObj;
      this.args = args;
      this.modifierDelegate = modifierDelegate;
    }

    public ModifierInfo(Dictionary<string, FPInfo> fpInfos,
                        Dictionary<string, object> fpInfosObj,
                        Dictionary<string, dynamic> args,
                        string delegateName) {
      this.fpInfos = fpInfos;
      this.fpInfosObj = fpInfosObj;
      this.args = args;
      this.delegateName = delegateName;
    }

    public Dictionary<string, FPInfo> fpInfos {
      get { return _fpInfos; }
      set { _fpInfos = value; }
    }

    public Dictionary<string, object> fpInfosObj {
      get { return _fpInfosObj; }
      set { _fpInfosObj = value; }
    }

    public Dictionary<string, dynamic> args {
      get { return _args; }
      set { _args = value; }
    }

    public string delegateName {
      get { return _delegateName; }
      set { _delegateName = value; }
    }

    public void RebuildInfo(object obj) {
      if (fpInfos.Count < 1) {
        Console.Write("There were not keys in fpInfos while Rebuilding the Info");
        return;
      }

      fpInfosObj = new Dictionary<string, object>();
      foreach (string key in fpInfos.Keys) {
        fpInfos[key] = new FPInfo(fpInfos[key].Name, obj);
        fpInfosObj.Add(key, obj);
      }
    }

    public void AddFPInfoObject(string id, FPInfo fpInfo, object obj) {
      fpInfos.Add(id, fpInfo);
      fpInfosObj.Add(id, obj);
    }

    public void AddFPInfoFromString(string id, string fieldname, object obj) {
      FPInfo fpInfo = new FPInfo(fieldname, obj);
      if (fpInfos.ContainsKey(id)) {
        Console.WriteLine("[{0}] was already found in the dictionary", id);
        return;
      }
      if (fpInfosObj.ContainsKey(id)) {
        Console.WriteLine("[{0}] was already found in the dictionary", id);
        return;
      }
      fpInfos.Add(id, fpInfo);
      fpInfosObj.Add(id, obj);
    }
  }
}