using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using OrbitVR.Framework;

namespace OrbitVR.UI {
  public class Command {
    
    public void ProcessConsoleCommand(String text) {
      text = text.Trim();

      if (text.Equals("")) {
        Console.WriteLine("No Command Provided");
        //consoletextbox.Text = "";
        return;
      }
      object currentObj = OrbIt.Game;


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