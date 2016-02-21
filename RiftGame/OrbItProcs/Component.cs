using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Component = OrbItProcs.Component;

namespace OrbItProcs
{
    [Flags]
    public enum mtypes
    {
        none = 0,
        initialize = 1,
        affectother = 2,
        affectself = 4,
        draw = 8,
        exclusiveLinker = 32,
        essential = 64,
        tracer = 128,
        playercontrol = 256,
        aicontrol = 512,
        item = 1024,
    };

    public abstract class Component {
        public virtual mtypes compType { get; set; }
        protected bool _active = false;
        [Info(UserLevel.Developer)]
        public virtual bool active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                if (parent != null && parent.HasComp(this.GetType()))
                {
                    parent.triggerSortLists();
                }
            }
        }
        public virtual Node parent { get; set; }
        public Room room { get { if (parent != null) return parent.room; return null; } }
        private bool _CallDraw = true;
        public bool CallDraw { get { return _CallDraw; } set { _CallDraw = value; } }

        public HashSet<Node> exclusions = new HashSet<Node>();

        protected float timePassed = 0;
        protected float maxTime = -1;
        protected bool IsDecaying = false;

        public void SetDecayMaxTime(int seconds, bool isDecaying = true)
        {
            IsDecaying = isDecaying;
            maxTime = seconds * 1000;
        }
        public void CaluclateDecay()
        {
            if (!IsDecaying) return;
            timePassed += OrbIt.gametime.ElapsedGameTime.Milliseconds;
            if (timePassed > maxTime)
            {
                parent.RemoveComponent(this.GetType());
            }
        }
        public bool isEssential()
        {
            return (compType & mtypes.essential) == mtypes.essential;
        }
        public bool hasCompType(mtypes methodtype)
        {
            return (compType & methodtype) == methodtype;
        }
        public virtual void Initialize(Node parent) { this.parent = parent; }
        public virtual void AfterCloning() { }
        public virtual void OnSpawn() { }
        public virtual void AffectOther(Node other) {  }
        public virtual void AffectSelf() { }
        public virtual void Draw() { }
        public virtual void PlayerControl(Input input) { }
        public virtual void AIControl(AIMode aiMode) { }
        public virtual void OnRemove(Node other) { }
        public virtual void InitializeLists() { }
        public bool IsItem()
        {
            return (compType & mtypes.item) == mtypes.item;
        }

        public virtual Texture2D getTexture()
        {
            if (parent != null)
            {
                return parent.getTexture();
            }
            return null;
        }

        public static Component GenerateComponent(Type t, Node par)
        {
            Component component = (Component)Activator.CreateInstance(t, par);
            return component;
        }
        
        public Component CreateClone(Node par)
        {
            Component comp = (Component)Activator.CreateInstance(this.GetType(), par);
            CloneComponent(this, comp);
            return comp;
        }
        public static void CloneComponent(Component sourceComp, Component destComp)
        {
            List<FieldInfo> fields = sourceComp.GetType().GetFields().ToList();
            fields.AddRange(sourceComp.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList());
            List<PropertyInfo> properties = sourceComp.GetType().GetProperties().ToList();
            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(ModifierInfo)) continue;
                if (property.PropertyType == typeof(Node))
                {
                    var cust = property.GetCustomAttributes(typeof(CopyNodeProperty), false);
                    if (cust.Length > 0)
                    {
                        Node n = (Node)property.GetValue(sourceComp, null);
                        Node nclone = n.CreateClone(sourceComp.room);
                        property.SetValue(destComp, nclone, null);
                        //Console.WriteLine("CLONING : " + property.Name);
                    }
                    continue;
                }
                if (Utils.isToggle(property.PropertyType))
                {
                    dynamic tog = property.GetValue(sourceComp, null);
                    dynamic newtog = tog.Clone();
                    property.SetValue(destComp, newtog, null);
                    continue;
                }
                if (property.PropertyType.IsClass)
                {
                    if (!typeof(Delegate).IsAssignableFrom(property.PropertyType) && !(property.PropertyType == typeof(Link)))
                    {
                        //Console.WriteLine("We should be aware of this.");
                    }
                }
                if (property.GetSetMethod() != null)
                property.SetValue(destComp, property.GetValue(sourceComp, null), null);
            }
            foreach (FieldInfo field in fields)
            {
                if (field.Name.Equals("shape")) continue;
                if (field.FieldType == typeof(Dictionary<string,ModifierInfo>))
                {
                    Modifier mod = (Modifier) sourceComp;
                   
                    Dictionary<string, ModifierInfo> newmodinfos = new Dictionary<string, ModifierInfo>();
                    foreach (KeyValuePair<string, ModifierInfo> kvp in mod.modifierInfos)
                    {
                        string key = kvp.Key;
                        ModifierInfo modifierInfo = kvp.Value;
                        Dictionary<string, FPInfo> newFpInfos = new Dictionary<string, FPInfo>();
                        Dictionary<string, object> newFpInfosObj = new Dictionary<string, object>();
                        foreach (string key2 in modifierInfo.fpInfos.Keys)
                        {
                            FPInfo fpinfo = new FPInfo(modifierInfo.fpInfos[key2]);

                            newFpInfos.Add(key2, fpinfo);
                            newFpInfosObj.Add(key2, null);
                        }
                        Dictionary<string, dynamic> newargs = new Dictionary<string, dynamic>();
                        foreach (string key2 in modifierInfo.args.Keys)
                        {
                            newargs.Add(key2, modifierInfo.args[key2]); //by reference (for now)
                        }

                        ModifierInfo modInfo = new ModifierInfo(newFpInfos, newFpInfosObj, newargs, modifierInfo.modifierDelegate);
                        modInfo.delegateName = modifierInfo.delegateName;
                        newmodinfos.Add(key, modInfo);
                    }
                    field.SetValue(destComp, newmodinfos);

                }
                //no longer checking for dictionaries, parent(Node)
                if ((field.FieldType == typeof(int))
                    || (field.FieldType == typeof(Single))
                    || (field.FieldType == typeof(bool))
                    || (field.FieldType == typeof(string)))
                {
                    field.SetValue(destComp, field.GetValue(sourceComp));
                }
                else if (field.FieldType == typeof(Vector2))
                {
                    Vector2 vect = (Vector2)field.GetValue(sourceComp);
                    Vector2 newvect = new Vector2(vect.X, vect.Y);
                    field.SetValue(destComp, newvect);
                }
                else if (field.FieldType == typeof(Color))
                {
                    Color col = (Color)field.GetValue(sourceComp);
                    Color newcol = new Color(col.R, col.G, col.B, col.A);
                    field.SetValue(destComp, newcol);
                }
                else
                {
                    //this would be an object field
                    if (field.Name.Equals("room"))
                    {
                        field.SetValue(destComp, field.GetValue(sourceComp));
                    }
                }
                //field.SetValue(newobj, field.GetValue(obj));
            }
            destComp.InitializeLists();
            destComp.AfterCloning();
        }

        //this is NOT clone component
       public static void CloneObject(object sourceObject, object destObject)
       {
           List<FieldInfo> fields = sourceObject.GetType().GetFields().ToList();
           fields.AddRange(sourceObject.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList());
           List<PropertyInfo> properties = sourceObject.GetType().GetProperties().ToList();
           foreach (PropertyInfo property in properties)
           {
               if (property.PropertyType == typeof(ModifierInfo)) continue;
               if (property.PropertyType == typeof(Node)) continue;
               if (property.GetSetMethod() != null)
               {
                   property.SetValue(destObject, property.GetValue(sourceObject, null), null);
               }
           }
           foreach (FieldInfo field in fields)
           {
               if (field.Name.Equals("shape")) continue;
               //no longer checking for dictionaries, parent(Node)
               if ((field.FieldType == typeof(int))
                   || (field.FieldType == typeof(Single))
                   || (field.FieldType == typeof(bool))
                   || (field.FieldType == typeof(string)))
               {
                   field.SetValue(destObject, field.GetValue(sourceObject));
               }
               else if (field.FieldType == typeof(Vector2))
               {
                   Vector2 vect = (Vector2)field.GetValue(sourceObject);
                   Vector2 newvect = new Vector2(vect.X, vect.Y);
                   field.SetValue(destObject, newvect);
               }
               else if (field.FieldType == typeof(Color))
               {
                   Color col = (Color)field.GetValue(sourceObject);
                   Color newcol = new Color(col.R, col.G, col.B, col.A);
                   field.SetValue(destObject, newcol);
               }
               else if (field.FieldType == typeof(Room))
               {
                    field.SetValue(destObject, field.GetValue(sourceObject));
               }
           }

           MethodInfo mInfo = destObject.GetType().GetMethod("InitializeLists");
           if (mInfo != null) mInfo.Invoke(destObject, null);
           mInfo = destObject.GetType().GetMethod("AfterCloning");
           if (mInfo != null) mInfo.Invoke(destObject, null);
           
           //destObject.InitializeLists();
           //destObject.AfterCloning();
       }


       public static HashSet<Type> compTypes;
       public static Dictionary<Type, Info> compInfos;
       static Component()
       {
           compTypes = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(assembly => assembly.GetTypes())
                      .Where(type => type.IsSubclassOf(typeof(Component))).ToHashSet();


           compInfos = new Dictionary<Type, Info>();
           foreach (Type t in compTypes)
           {
               Info info = Utils.GetInfoType(t);
               if (info == null) continue;
               compInfos[t] = info;
           }
       }

    }

    
}
