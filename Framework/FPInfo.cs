using System;
using System.Reflection;

namespace OrbitVR.Framework {
  public class FPInfo {
    private FieldInfo _fieldInfo;

    private PropertyInfo _propertyInfo;
    public object ob;

    public FieldInfo fieldInfo {
      get { return _fieldInfo; }
      set { _fieldInfo = value; }
    }

    public PropertyInfo propertyInfo {
      get { return _propertyInfo; }
      set { _propertyInfo = value; }
    }

    public string DeclaringTypeName { get; set; }

    public string Name { get; set; }

    public Type FPType {
      get {
        if (propertyInfo != null) {
          return propertyInfo.PropertyType;
        }
        else if (fieldInfo != null) {
          return fieldInfo.FieldType;
        }
        return null;
      }
      set { }
    }

    public FPInfo(FieldInfo fieldInfo) {
      this.fieldInfo = fieldInfo;
      this.DeclaringTypeName = this.fieldInfo.DeclaringType.ToString();
      Name = fieldInfo.Name;
    }

    public FPInfo(PropertyInfo propertyInfo) {
      this.propertyInfo = propertyInfo;
      this.DeclaringTypeName = this.propertyInfo.DeclaringType.ToString();
      Name = propertyInfo.Name;
    }

    public FPInfo(FieldInfo fieldInfo, PropertyInfo propertyInfo) //for copying component use
    {
      this.propertyInfo = propertyInfo;
      this.fieldInfo = fieldInfo;
      if (propertyInfo != null) {
        this.DeclaringTypeName = this.propertyInfo.DeclaringType.ToString();
        Name = propertyInfo.Name;
      }
      else if (fieldInfo != null) {
        this.DeclaringTypeName = this.fieldInfo.DeclaringType.ToString();
        Name = fieldInfo.Name;
      }
      else Name = "error_Name_1";
      //ob = null;
    }

    public FPInfo(FPInfo old) //for copying component use
    {
      this.propertyInfo = old.propertyInfo;
      this.fieldInfo = old.fieldInfo;

      if (propertyInfo != null) {
        Name = propertyInfo.Name;
        DeclaringTypeName = propertyInfo.DeclaringType.ToString();
      }
      else if (fieldInfo != null) {
        Name = fieldInfo.Name;
        DeclaringTypeName = fieldInfo.DeclaringType.ToString();
      }
      else if (old.DeclaringTypeName != null) {
        //PropertyInfo pi = old.DeclaringTypeName.GetProperty(old.Name);
        PropertyInfo pi = Type.GetType(old.DeclaringTypeName).GetProperty(old.Name);
        if (pi != null) {
          this.propertyInfo = pi;
          Name = old.Name;
          return;
        }
        //FieldInfo fi = old.DeclaringTypeName.GetField(old.Name);
        FieldInfo fi = Type.GetType(old.DeclaringTypeName).GetField(old.Name);
        if (fi != null) {
          this.fieldInfo = fi;
          Name = old.Name;
          return;
        }
      }
      else Name = "error_Name_2";

      //ob = null;
    }

    public FPInfo(string name, object obj) {
      ob = obj;
      propertyInfo = obj.GetType().GetProperty(name);
      DeclaringTypeName = obj.GetType().ToString();
      Name = name;
      if (propertyInfo == null) {
        fieldInfo = obj.GetType().GetField(name);
        if (fieldInfo == null) {
          Console.WriteLine("member was not found.");
          name = "error_Name_3";
          DeclaringTypeName = null;
        }
      }
    }

    public static FPInfo GetNew(string name, object obj) {
      return new FPInfo(name, obj);
    }

    public object GetValue() {
      if (propertyInfo != null) {
        return propertyInfo.GetValue(ob, null);
      }
      else if (fieldInfo != null) {
        return fieldInfo.GetValue(ob);
      }
      return null;
    }

    public object GetValue(object obj) {
      if (propertyInfo != null) {
        return propertyInfo.GetValue(obj, null);
      }
      else if (fieldInfo != null) {
        return fieldInfo.GetValue(obj);
      }
      return null;
    }


    public void SetValue(object value) {
      if (propertyInfo != null) {
        propertyInfo.SetValue(ob, value, null);
      }
      else if (fieldInfo != null) {
        if (fieldInfo.IsLiteral) return;
        fieldInfo.SetValue(ob, value);
      }
    }

    public void SetValue(object value, object obj) {
      if (propertyInfo != null) {
        propertyInfo.SetValue(obj, value, null);
      }
      else if (fieldInfo != null) {
        if (fieldInfo.IsLiteral) return;
        fieldInfo.SetValue(obj, value);
      }
    }

    public static object GetValue(string name, object obj) {
      PropertyInfo propertyInfo = obj.GetType().GetProperty(name);
      if (propertyInfo != null) {
        return propertyInfo.GetValue(obj, null);
      }
      else {
        FieldInfo fieldInfo = obj.GetType().GetField(name);
        if (fieldInfo != null) {
          return fieldInfo.GetValue(obj);
        }
      }
      return null;
    }

    //calls a method with no parameters on either fieldinfo or propertyinfo object
    public object CallMethod(string methodname) {
      //if (ob == null) return;
      try {
        if (propertyInfo != null) {
          return propertyInfo.GetType().GetMethod(methodname).Invoke(propertyInfo, null);
        }
        else if (fieldInfo != null) {
          return fieldInfo.GetType().GetMethod(methodname).Invoke(fieldInfo, null);
        }
        return null;
      }
      catch (Exception e) {
        Console.WriteLine("FPInfo exception: {0}", e.Message);
        return null;
      }
    }

    /*
        public string Name()
        {
            if (propertyInfo != null)
            {

                return propertyInfo.Name;
            }
            else if (fieldInfo != null)
            {
                return fieldInfo.Name;
            }
            return "nameless";
        }
        */

    public static void SetValue(string name, object obj, object value) {
      PropertyInfo propertyInfo = obj.GetType().GetProperty(name);
      if (propertyInfo != null) {
        propertyInfo.SetValue(obj, value, null);
      }
      else {
        FieldInfo fieldInfo = obj.GetType().GetField(name);
        if (fieldInfo != null) {
          fieldInfo.SetValue(obj, value);
        }
      }
    }
  }
}