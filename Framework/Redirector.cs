using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using OrbitVR.Interface;

namespace OrbitVR.Framework {
  public class Redirector {
    public static Dictionary<Type, Dictionary<string, Func<object, object>>> getters =
      new Dictionary<Type, Dictionary<string, Func<object, object>>>();

    public static Dictionary<Type, Dictionary<string, Action<object, object>>> setters =
      new Dictionary<Type, Dictionary<string, Action<object, object>>>();

    private object _Parent;

    public Dictionary<string, Func<object, object>> getDelegates = new Dictionary<string, Func<object, object>>();

    public Dictionary<string, object> GetPropertyToObject = new Dictionary<string, object>();
    public Dictionary<string, Action<object, object>> setDelegates = new Dictionary<string, Action<object, object>>();
    public Dictionary<string, object> SetPropertyToObject = new Dictionary<string, object>();

    public object Parent {
      get { return _Parent; }
      set {
        _Parent = value;
        Type targetType = _Parent.GetType();
        if (!getters.ContainsKey(targetType) || !setters.ContainsKey(targetType)) {
          PopulateDelegates(targetType);
        }
      }
    }

    public bool active {
      get {
        try {
          //MethodInfo castMethod = this.GetType().GetMethod("Cast").MakeGenericMethod(
          //Type t = _TargetObject.GetType();
          //MethodInfo minfo = typeof(Redirector).GetMethod("Cast");
          //MethodInfo genericmethod = minfo.MakeGenericMethod(new Type[] { t });
          //
          //return getters[_TargetObject.GetType()]["active"](genericmethod.Invoke(null, new object[] { _TargetObject }));
          //return getters[_TargetObject.GetType()]["active"]( _TargetObject );

          //Delegate getter = getters[_TargetObject.GetType()]["active"];
          //return (bool)getter.DynamicInvoke(_TargetObject);

          object obj = GetPropertyToObject["active"];
          //return (bool)getters[obj.GetType()]["active"](obj);
          return (bool) getDelegates["active"](obj);

          //return getters[TargetObject.GetType()]["active"]((Movement)TargetObject);
        }
        catch (Exception e) {
          throw e;
        }
        //return false;
      }
      set {
        //Type t = _TargetObject.GetType();
        //Console.WriteLine(t);
        //MethodInfo minfo = typeof(Redirector).GetMethod("Cast");
        //MethodInfo genericmethod = minfo.MakeGenericMethod(new Type[] { t });
        //
        //setters[_TargetObject.GetType()]["active"](genericmethod.Invoke(null,new object[]{_TargetObject}), value);

        //Delegate setter = setters[_TargetObject.GetType()]["active"];
        //setter.DynamicInvoke(_TargetObject, value);
        //setters[_TargetObject.GetType()]["active"]((Movement)_TargetObject, value);

        try {
          object obj = SetPropertyToObject["active"];
          //setters[obj.GetType()]["active"](obj, value);
          setDelegates["active"](obj, value);

          //setters[TargetObject.GetType()]["active"]((Movement)TargetObject, value);
        }
        catch (Exception e) {
          throw e;
        }
      }
    }

    public Redirector() {
      /*
            List<PropertyInfo> pinfos = typeof(Redirector).GetProperties().ToList();
            foreach (PropertyInfo pinfo in pinfos)
            {
                
            }
            */
    }

    public void AssignObjectToPropertiesAll(object o, bool OnlyUninhabited = false, bool AssignGetters = true,
                                            bool AssignSetters = true) {
      Type targetType = o.GetType();
      if (!getters.ContainsKey(targetType) || !setters.ContainsKey(targetType)) {
        PopulateDelegates(targetType);
      }

      List<PropertyInfo> pinfos = o.GetType().GetProperties().ToList();
      if (AssignGetters) {
        foreach (PropertyInfo pinfo in pinfos) {
          if (OnlyUninhabited && GetPropertyToObject.ContainsKey(pinfo.Name)) continue;
          if (getters[o.GetType()].ContainsKey(pinfo.Name)) {
            GetPropertyToObject[pinfo.Name] = o;
            getDelegates[pinfo.Name] = getters[o.GetType()][pinfo.Name];
          }
        }
      }
      if (AssignSetters) {
        foreach (PropertyInfo pinfo in pinfos) {
          if (OnlyUninhabited && SetPropertyToObject.ContainsKey(pinfo.Name)) continue;
          if (setters[o.GetType()].ContainsKey(pinfo.Name)) {
            SetPropertyToObject[pinfo.Name] = o;
            setDelegates[pinfo.Name] = setters[o.GetType()][pinfo.Name];
          }
        }
      }
    }

    public void AssignObjectToProperty(string property, object o) {
      Type targetType = o.GetType();
      if (!getters.ContainsKey(targetType) || !setters.ContainsKey(targetType)) {
        PopulateDelegates(targetType);
      }
      if (getters[o.GetType()].ContainsKey(property)) {
        GetPropertyToObject[property] = o;
        getDelegates[property] = getters[o.GetType()][property];
      }
      if (setters[o.GetType()].ContainsKey(property)) {
        SetPropertyToObject[property] = o;
        setDelegates[property] = setters[o.GetType()][property];
      }
    }

    public void AssignObjectToPropertyGet(string property, object o) {
      Type targetType = o.GetType();
      if (!getters.ContainsKey(targetType)) {
        PopulateDelegates(targetType);
      }
      if (getters[o.GetType()].ContainsKey(property)) {
        GetPropertyToObject[property] = o;
        getDelegates[property] = getters[o.GetType()][property];
      }
    }

    public void AssignObjectToPropertySet(string property, object o) {
      Type targetType = o.GetType();
      if (!setters.ContainsKey(targetType)) {
        PopulateDelegates(targetType);
      }
      if (setters[o.GetType()].ContainsKey(property)) {
        SetPropertyToObject[property] = o;
        setDelegates[property] = setters[o.GetType()][property];
      }
    }

    //for getters (Func)
    static Func<object, object> MagicFunc(MethodInfo method) {
      // First fetch the generic form
      MethodInfo genericHelper = typeof (Redirector).GetMethod("MagicFuncHelper",
                                                               BindingFlags.Static | BindingFlags.NonPublic);

      // Now supply the type arguments
      //ParameterInfo[] parameters = method.GetParameters();
      MethodInfo constructedHelper = genericHelper.MakeGenericMethod
        (method.ReflectedType, method.ReturnType);

      // Now call it. The null argument is because it's a static method.
      object ret = constructedHelper.Invoke(null, new object[] {method});

      // Cast the result to the right kind of delegate and return it
      return (Func<object, object>) ret;
    }

    static Func<object, object> MagicFuncHelper<TTarget, TReturn>(MethodInfo method) {
      // Convert the slow MethodInfo into a fast, strongly typed, open delegate
      Func<TTarget, TReturn> func = (Func<TTarget, TReturn>) Delegate.CreateDelegate
                                                               (typeof (Func<TTarget, TReturn>), method);
      // Now create a more weakly typed delegate which will call the strongly typed one
      Func<object, object> ret = (object target) => func((TTarget) target);
      return ret;
    }

    //for setters (Action)
    static Action<object, object> MagicAction(MethodInfo method) {
      // First fetch the generic form
      MethodInfo genericHelper = typeof (Redirector).GetMethod("MagicActionHelper",
                                                               BindingFlags.Static | BindingFlags.NonPublic);

      // Now supply the type arguments
      //ParameterInfo[] parameters = method.GetParameters();
      MethodInfo constructedHelper = genericHelper.MakeGenericMethod
        (method.ReflectedType, method.GetParameters()[0].ParameterType);

      // Now call it. The null argument is because it's a static method.
      object ret = constructedHelper.Invoke(null, new object[] {method});

      // Cast the result to the right kind of delegate and return it
      return (Action<object, object>) ret;
    }

    static Action<object, object> MagicActionHelper<TTarget, TParam>(MethodInfo method) {
      // Convert the slow MethodInfo into a fast, strongly typed, open delegate
      Action<TTarget, TParam> action = (Action<TTarget, TParam>) Delegate.CreateDelegate
                                                                   (typeof (Action<TTarget, TParam>), method);

      // Now create a more weakly typed delegate which will call the strongly typed one
      Action<object, object> ret = (object target, object value) => action((TTarget) target, (TParam) value);
      return ret;
    }

    ///////////////
    /*
        john skeet { // new keywords in C# -1.0
            static Func<T, object, object> MagicMethod<T>(MethodInfo method) where T : class
            {
                // First fetch the generic form
                MethodInfo genericHelper = typeof(Redirector).GetMethod("MagicMethodHelper",
                    BindingFlags.Static | BindingFlags.NonPublic);

                // Now supply the type arguments
                MethodInfo constructedHelper = genericHelper.MakeGenericMethod
                    (typeof(T), method.GetParameters()[0].ParameterType, method.ReturnType);

                // Now call it. The null argument is because it's a static method.
                object ret = constructedHelper.Invoke(null, new object[] { method });

                // Cast the result to the right kind of delegate and return it
                return (Func<T, object, object>)ret;
            }

            static Func<TTarget, object, object> MagicMethodHelper<TTarget, TParam, TReturn>(MethodInfo method)
                where TTarget : class
            {
                // Convert the slow MethodInfo into a fast, strongly typed, open delegate
                Func<TTarget, TParam, TReturn> func = (Func<TTarget, TParam, TReturn>)Delegate.CreateDelegate
                    (typeof(Func<TTarget, TParam, TReturn>), method);

                // Now create a more weakly typed delegate which will call the strongly typed one
                Func<TTarget, object, object> ret = (TTarget target, object param) => func(target, (TParam)param);
                return ret;
            }
        }
        */
    ///////////////////
    /*
        public void Set<TObject,TValue>(TObject obj, TValue value, string property)
        {
            try
            {
                setters[typeof(TObject)][property](value);
            }
            catch (Exception e)
            {
                Console.WriteLine("We fucked up. Hard. When setting shit. ({0})", e.Message);
            }
        }

        public T Get<TObject, T>(TObject obj, string property)
        {
            try
            {
                return (T)getters[typeof(TObject)][property]();
            }
            catch (Exception e)
            {
                Console.WriteLine("We fucked up. Hard. When getting shit. ({0})",e.Message);
                return (T)Type.Missing;
            }
        }
        */

    public void PopulateCastingDictionaries() {}

    public static void PopulateDelegatesAll() {
      foreach (Type componentType in Component.compTypes) {
        PopulateDelegates(componentType);
      }
      PopulateDelegates(typeof (Node));
      PopulateDelegates(typeof (Component));
    }

    public static void PopulateDelegates(Type type) {
      if (!getters.ContainsKey(type)) getters[type] = new Dictionary<string, Func<object, object>>();
      if (!setters.ContainsKey(type)) setters[type] = new Dictionary<string, Action<object, object>>();

      List<PropertyInfo> propertyinfos =
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();

      foreach (PropertyInfo info in propertyinfos) {
        Info[] infoAttributes = (Info[]) info.GetCustomAttributes(typeof (Info), false);
        if (infoAttributes.Any(x => x.userLevel == UserLevel.Never)) continue;

        Type[] types = new Type[] {type, info.PropertyType};
        if (!getters[type].ContainsKey(info.Name)) {
          //try
          //{
          MethodInfo getmethod = info.GetGetMethod();

          if (getmethod != null) {
            Type methodtype = Expression.GetFuncType(types);
            //getters[type][info.Name] = Delegate.CreateDelegate(methodtype, getmethod);
            Delegate get = Delegate.CreateDelegate(methodtype, getmethod);
            getters[type][info.Name] = MagicFunc(get.Method);
          }
          //}
          //catch { }
        }
        if (!setters[type].ContainsKey(info.Name)) {
          //try
          //{
          MethodInfo setmethod = info.GetSetMethod();
          if (setmethod != null) {
            Type methodtype = Expression.GetActionType(types);
            //setters[type][info.Name] = Delegate.CreateDelegate(methodtype, setmethod);
            Delegate set = Delegate.CreateDelegate(methodtype, setmethod);
            setters[type][info.Name] = MagicAction(set.Method);
            //if (type == typeof(Movement))
            //    Console.WriteLine(propertyinfos);
          }
          //}
          //catch { }
        }
      }
    }
  }
}