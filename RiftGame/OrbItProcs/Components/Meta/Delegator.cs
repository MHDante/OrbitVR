using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace OrbItProcs
{
    public class MethodEntry
    {
        public Delegator delegator;

        public string name { get; set; }
        private bool _enabled = false;
        public bool enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (delegator != null)
                {
                    if (value && !_enabled)
                    {
                        delegator.AddMethodEntry(this);
                    }
                    else if (!value && _enabled)
                    {
                        delegator.RemoveMethodEntry(this);
                    }
                }
                _enabled = value;
            }
        }

        public MethodEntry(Delegator delegator, string name, bool enabled = false)
        {
            this.delegator = delegator;
            this.name = name;
            this.enabled = _enabled;
        }

        public override string ToString()
        {
            return name + ":" + _enabled;
        }

    }

    public static class DelegatorMethods
    {
        public static Dictionary<string, dynamic> delegates = new Dictionary<string, dynamic>();
        public static Dictionary<string, Type> delegateTypes = new Dictionary<string, Type>();

        static DelegatorMethods()
        {
            Type type = typeof(DelegatorMethods);
            foreach(var meth in type.GetMethods())
            {
                //if (meth.Name.Equals("InitializeDelegateMethods")) continue; //add more non-delegate compType here
                //delegates[meth.Name] = Delegate.CreateDelegate(meth.typ)
                Type methType = null;
                if (meth.ReturnType != typeof(void)) continue;
                ParameterInfo[] parameters = meth.GetParameters();
                if (parameters.Length == 0) continue;
                if (parameters[0].ParameterType != typeof(Node)) continue;
                if (parameters.Length == 1)
                {
                    methType = typeof(Action<Node>);

                }
                else if (parameters.Length == 2)
                {
                    if (parameters[1].ParameterType == typeof(Node))
                    {
                        methType = typeof(Action<Node, Node>);
                    }
                    else if (parameters[1].ParameterType == typeof(DataStore))
                    {
                        methType = typeof(Action<Node, DataStore>);
                    }
                    else if (parameters[1].ParameterType == typeof(SpriteBatch))
                    {
                        methType = typeof(Action<Node, SpriteBatch>);
                    }
                }
                else if (parameters.Length == 3)
                {
                    if (parameters[1].ParameterType == typeof(Node) && parameters[2].ParameterType == typeof(DataStore))
                    {
                        methType = typeof(Action<Node, Node, DataStore>);
                    }
                    else if (parameters[1].ParameterType == typeof(SpriteBatch) && parameters[2].ParameterType == typeof(DataStore))
                    {
                        methType = typeof(Action<Node, SpriteBatch, DataStore>);
                    }
                }
                if (methType != null)
                {
                    dynamic del = Delegate.CreateDelegate(methType, meth);
                    delegates[meth.Name] = del;
                    delegateTypes[meth.Name] = methType;
                }
            }
        }

        public static DataStore getDataStore(string name)
        {
            if (name.Equals("SuddenStop"))
            {
                DataStore dd = new DataStore() {
                    { "count", 0 },
                    { "max", 1000 },
                    { "multiplier", 1.01f },
                };
                return dd;
            }

            return null;
        }

        //AFFECT OTHER
        public static void SwitchVelocities(Node parent, Node other)
        {
            int rand = Utils.random.Next(10);
            if (rand == 0)
            {
                Vector2 tempVelocity = parent.body.velocity;
                parent.body.velocity = other.body.velocity;
                other.body.velocity = tempVelocity;
                //Console.WriteLine(rand);
            }
            //
        }

        //AFFECT OTHER DATASTORE

        //AFFECT SELF

        //AFFECT SELF DATASTORE
        public static void SuddenStop(Node parent, DataStore d)
        {
            if (d["count"]++ > d["max"])
            {
                parent.body.velocity /= (d["multiplier"] * d["max"]);
                d["count"] = 0;
            }
            else
            {
                parent.body.velocity *= d["multiplier"];
            }
        }

        //DRAW

        //DRAW DATASTORE

    }
    /// <summary>
    /// Special component that tests out a variety of Component prototypes and mini Components.
    /// </summary>
    [Info(UserLevel.Developer, "Special component that tests out a variety of Component prototypes and mini Components.", mtypes.none)]
    public class Delegator : Component
    {
        private mtypes CompType = mtypes.none;
        public override mtypes compType { get { return CompType; } set { CompType = value; } }

        private Dictionary<string, Action<Node, Node>> aOtherDelegates;
        private Dictionary<string, Action<Node>> aSelfDelegates;
        private Dictionary<string, Action<Node>> drawDelegates;
        private Dictionary<string, Action<Node, Node, DataStore>> aOtherDelegatesDS;
        private Dictionary<string, Action<Node, DataStore>> aSelfDelegatesDS;
        private Dictionary<string, Action<Node, DataStore>> drawDelegatesDS;

        public Dictionary<string, MethodEntry> delegates { get; set; }

        public Dictionary<string, DataStore> datastores { get; set; }

        public Delegator() :this(null) { }

        public Delegator(Node parent)
        {
            this.parent = parent;
            datastores = new Dictionary<string, DataStore>();
            delegates = new Dictionary<string, MethodEntry>();
            foreach(string key in DelegatorMethods.delegates.Keys)
            {
                delegates[key] = new MethodEntry(this, key);
            }
        }
        public override void AfterCloning()
        {
            if (delegates == null) delegates = new Dictionary<string, MethodEntry>();
            foreach (string key in DelegatorMethods.delegates.Keys)
            {
                if (!delegates.ContainsKey(key))
                {
                    delegates[key] = new MethodEntry(this, key);
                }
                else
                {
                    MethodEntry me = delegates[key];
                    delegates[key] = new MethodEntry(this, key);
                    delegates[key].enabled = me.enabled;
                }
            }
        }
        public void AddMethodEntry(MethodEntry methodEntry)
        {
            if (methodEntry == null) return;
            string n = methodEntry.name;
            if (!DelegatorMethods.delegates.ContainsKey(n) || !DelegatorMethods.delegateTypes.ContainsKey(n)) return;
            Type type = DelegatorMethods.delegateTypes[n];
            dynamic del = DelegatorMethods.delegates[n];
            if (type == typeof(Action<Node>))
            {
                AddAffectSelf(n, del);
            }
            else if (type == typeof(Action<Node, Node>))
            {
                AddAffectOther(n, del);
            }
            else if (type == typeof(Action<Node>))
            {
                AddDraw(n, del);
            }
            else
            {
                DataStore ds = DelegatorMethods.getDataStore(n);
                if (ds == null) return;
                if (type == typeof(Action<Node, DataStore>))
                {
                    AddAffectSelfAndDS(n, del, ds);
                }
                else if (type == typeof(Action<Node, Node, DataStore>))
                {
                    AddAffectOtherAndDS(n, del, ds);
                }
                else if (type == typeof(Action<Node, SpriteBatch, DataStore>))
                {
                    AddDrawAndDS(n, del, ds);
                }
            }
        }
        public void RemoveMethodEntry(MethodEntry methodEntry)
        {
            if (methodEntry == null) return;
            string n = methodEntry.name;
            if (!DelegatorMethods.delegates.ContainsKey(n) || !DelegatorMethods.delegateTypes.ContainsKey(n)) return;
            Type type = DelegatorMethods.delegateTypes[n];
            if (type == typeof(Action<Node>))
            {
                if (aSelfDelegates != null && aSelfDelegates.ContainsKey(n)) aSelfDelegates.Remove(n);
                if ((aSelfDelegates != null || aSelfDelegates.Count == 0) && (aSelfDelegatesDS == null || aSelfDelegatesDS.Count == 0)) compType &= ~mtypes.affectself;
            }
            else if (type == typeof(Action<Node, Node>))
            {
                if (aOtherDelegates != null && aOtherDelegates.ContainsKey(n)) aOtherDelegates.Remove(n);
                if ((aOtherDelegates == null || aOtherDelegates.Count == 0) && (aOtherDelegatesDS == null || aOtherDelegatesDS.Count == 0)) compType &= ~mtypes.affectother;
            }
            else if (type == typeof(Action<Node, SpriteBatch>))
            {
                if (drawDelegates != null && drawDelegates.ContainsKey(n)) drawDelegates.Remove(n);
                if ((drawDelegates == null || drawDelegates.Count == 0) && (drawDelegatesDS == null || drawDelegatesDS.Count == 0)) compType &= ~(mtypes.draw);
            }
            else
            {
                if (type == typeof(Action<Node, DataStore>))
                {
                    if (aSelfDelegatesDS != null && aSelfDelegatesDS.ContainsKey(n)) aSelfDelegatesDS.Remove(n);
                    if ((aSelfDelegatesDS == null || aSelfDelegatesDS.Count == 0) && (aSelfDelegates == null || aSelfDelegates.Count == 0)) compType &= ~mtypes.affectself;
                }
                else if (type == typeof(Action<Node, Node, DataStore>))
                {
                    if (aOtherDelegatesDS != null && aOtherDelegatesDS.ContainsKey(n)) aOtherDelegatesDS.Remove(n);
                    if ((aOtherDelegatesDS == null || aOtherDelegatesDS.Count == 0) && (aOtherDelegates == null || aOtherDelegates.Count == 0)) compType &= ~mtypes.affectother;
                }
                else if (type == typeof(Action<Node, SpriteBatch, DataStore>))
                {
                    if (drawDelegatesDS != null && drawDelegatesDS.ContainsKey(n)) drawDelegatesDS.Remove(n);
                    if ((drawDelegatesDS == null || drawDelegatesDS.Count == 0) && (drawDelegates == null || drawDelegates.Count == 0)) compType &= ~(mtypes.draw);
                }
            }
            if (datastores.ContainsKey(n)) datastores.Remove(n);
        }


        public void AddAffectOther(string name, Action<Node, Node> del)
        {
            if (aOtherDelegates == null) aOtherDelegates = new Dictionary<string, Action<Node, Node>>();
            if (aOtherDelegates.ContainsKey(name)) return;
            aOtherDelegates[name] = del;
            DataStore ds = new DataStore();
            ds["active"] = true;
            //overwrites previous if existed
            datastores[name] = ds;
            if ((compType & mtypes.affectother) != mtypes.affectother)
            {
                compType = compType | mtypes.affectother;
            }
            if (parent != null) parent.SortComponentListsUpdate();
        }
        public void AddAffectOtherAndDS(string name, Action<Node, Node, DataStore> del, DataStore ds)
        {
            if (aOtherDelegatesDS == null) aOtherDelegatesDS = null;
            if (aOtherDelegatesDS.ContainsKey(name)) return;
            aOtherDelegatesDS[name] = del;
            if (ds == null)
            {
                ds = new DataStore();
            }
            if (ds.ContainsKey("active"))
            {
                if (!ds["active"] is bool) throw new SystemException("In a DataStore, active must be bool");
            }
            else
            {
                ds["active"] = true;
            }
            //overwrites previous if existed
            datastores[name] = ds;
            if ((compType & mtypes.affectother) != mtypes.affectother)
            {
                compType = compType | mtypes.affectother;
            }
            if (parent != null) parent.SortComponentListsUpdate();
        }
        //
        public void AddAffectSelf(string name, Action<Node> del)
        {
            if (aSelfDelegates == null) aSelfDelegates = new Dictionary<string, Action<Node>>();
            if (aSelfDelegates.ContainsKey(name)) return;
            aSelfDelegates[name] = del;
            DataStore ds = new DataStore();
            ds["active"] = true;
            //overwrites previous if existed
            datastores[name] = ds;
            if ((compType & mtypes.affectself) != mtypes.affectself)
            {
                compType = compType | mtypes.affectself;
            }
            if (parent != null) parent.SortComponentListsUpdate();

        }
        public void AddAffectSelfAndDS(string name, Action<Node, DataStore> del, DataStore ds)
        {
            if (aSelfDelegatesDS == null) aSelfDelegatesDS = new Dictionary<string, Action<Node, DataStore>>();
            if (aSelfDelegatesDS.ContainsKey(name)) return;
            aSelfDelegatesDS[name] = del;
            if (ds == null)
            {
                ds = new DataStore();
            }
            if (ds.ContainsKey("active"))
            {
                if (!ds["active"] is bool) throw new SystemException("In a DataStore, active must be bool");
            }
            else
            {
                ds["active"] = true;
            }
            //overwrites previous if existed
            datastores[name] = ds;
            if ((compType & mtypes.affectself) != mtypes.affectself)
            {
                compType = compType | mtypes.affectself;
            }
            if (parent != null) parent.SortComponentListsUpdate();
        }
        //
        public void AddDraw(string name, Action<Node> del)
        {
            if (drawDelegates == null) drawDelegates = new Dictionary<string, Action<Node>>();
            if (drawDelegates.ContainsKey(name)) return;
            drawDelegates[name] = del;
            DataStore ds = new DataStore();
            ds["active"] = true;
            //overwrites previous if existed
            datastores[name] = ds;
            if ((compType & mtypes.affectself) != mtypes.affectself)
            {
                compType = compType | mtypes.affectself;
            }
            if (parent != null) parent.SortComponentListsUpdate();
        }
        public void AddDrawAndDS(string name, Action<Node, DataStore> del, DataStore ds)
        {
            if (drawDelegatesDS == null) drawDelegatesDS = new Dictionary<string, Action<Node, DataStore>>();
            if (drawDelegatesDS.ContainsKey(name)) return;
            drawDelegatesDS[name] = del;
            if (ds == null)
            {
                ds = new DataStore();
            }
            if (ds.ContainsKey("active"))
            {
                if (!ds["active"] is bool) throw new SystemException("In a DataStore, active must be bool");
            }
            else
            {
                ds["active"] = true;
            }
            //overwrites previous if existed
            datastores[name] = ds;

            ///WHAT THE FUCK.
            //if ((compType & mtypes.draw) != mtypes.draw)
            //{
            //    compType = compType | mtypes.minordraw;
            //}
            if (parent != null) parent.SortComponentListsDraw();
        }
        //
        public override void AffectOther(Node other)
        {
            if (!active) return;
            if (aOtherDelegates != null)
            {
                foreach (string key in aOtherDelegates.Keys)
                {
                    aOtherDelegates[key](parent, other);
                }
            }
            if (aOtherDelegatesDS != null)
            {
                foreach (string key in aOtherDelegatesDS.Keys)
                {
                    DataStore ds = null;
                    if (datastores.ContainsKey(key))
                    {
                        ds = datastores[key];
                    }
                    aOtherDelegatesDS[key](parent, other, ds);
                }
            }
        }

        public override void AffectSelf()
        {
            if (!active) return;
            if (aSelfDelegates != null)
            {
                foreach (string key in aSelfDelegates.Keys)
                {
                    aSelfDelegates[key](parent);
                }
            }
            if (aSelfDelegatesDS != null)
            {
                foreach (string key in aSelfDelegatesDS.Keys)
                {
                    DataStore ds = null;
                    if (datastores.ContainsKey(key))
                    {
                        ds = datastores[key];
                    }
                    aSelfDelegatesDS[key](parent, ds);
                }
            }
        }
        public override void Draw()
        {
            if (!active) return;
            if (drawDelegates != null)
            {
                foreach (string key in drawDelegates.Keys)
                {
                    drawDelegates[key](parent);
                }
            }
            if (drawDelegatesDS != null)
            {
                foreach (string key in drawDelegatesDS.Keys)
                {
                    DataStore ds = null;
                    if (datastores.ContainsKey(key))
                    {
                        ds = datastores[key];
                    }
                    drawDelegatesDS[key](parent, ds);
                }
            }
        }
        

    }
}
