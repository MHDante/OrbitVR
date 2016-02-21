using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;
using System.Reflection;



namespace OrbItProcs
{
    /// <summary>
    /// Modifies the values of component fields by using arbitrary functions.
    /// </summary>
    [Info(UserLevel.Developer, "Modifies the values of component fields by using arbitrary functions.", CompType)]
    public class Modifier : Component
    {
        public const mtypes CompType = mtypes.affectself;
        public override mtypes compType { get { return CompType; } set { } }
        private Dictionary<string, ModifierInfo> _modifierInfos = new Dictionary<string, ModifierInfo>();
        public Dictionary<string, ModifierInfo> modifierInfos { get { return _modifierInfos; } set { _modifierInfos = value; } }

        //private ModifierInfo _modifierInfo = null;
        //public ModifierInfo modifierInfo { get { return _modifierInfo; } set { _modifierInfo = value;  } }

        public Modifier() : this(null) { }
        public Modifier(Node parent)
        {
            if (parent != null) this.parent = parent;
        }

        public void UpdateReferences()
        {
            if (parent != null)
            {
                foreach (ModifierInfo modifierInfo in modifierInfos.Values)
                {
                    for (int i = 0; i < modifierInfo.fpInfos.Keys.Count; i++)
                    {
                        if (i <= modifierInfo.fpInfos.Keys.Count)
                        {
                            //we need to fix this in the case that some objects being referenced aren't the parent node
                            modifierInfo.fpInfos[modifierInfo.fpInfos.ElementAt(i).Key].ob = parent;
                            modifierInfo.fpInfosObj[modifierInfo.fpInfosObj.ElementAt(i).Key] = parent;
                        }
                    }
                }
            }
        }

        

        public override void Initialize(Node parent)
        {
            this.parent = parent;
            if (modifierInfos.Count > 0)
            {
                UpdateReferences();
            }
        }
        /*
        public void Initialize(Node parent, ModifierInfo modifierInfo)
        {
            this.parent = parent;
            if (modifierInfo != null)
            {
                this.modifierInfo = modifierInfo;
            }
            UpdateReferences();
        }
        */

        public override void AffectSelf()
        {
            foreach (ModifierInfo modifierInfo in modifierInfos.Values)
            {
                if (modifierInfo.delegateName != "")
                {
                    MethodInfo meth = typeof(DelegateManager).GetMethod(modifierInfo.delegateName);
                    if (meth != null)
                    {
                        object[] parameters = new object[2] { modifierInfo.args, modifierInfo };

                        meth.Invoke(null, parameters); // this call is extremely slow; use a delegate
                        return;
                    }
                }
                if (modifierInfo.modifierDelegate != null)
                {
                    modifierInfo.modifierDelegate(modifierInfo.args, modifierInfo);
                    
                }
            }

        }

        public override void Draw()
        {

        }
    }
}
