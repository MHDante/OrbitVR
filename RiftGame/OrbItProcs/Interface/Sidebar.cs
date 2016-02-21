using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Color = SharpDX.Color;
using SharpDX;
using SharpOVR;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.DXGI;
using SharpDX.Toolkit.Content;
using System.Reflection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using OrbItProcs;


using Component = OrbItProcs.Component;
using Console = System.Console;

namespace OrbItProcs
{
    public partial class Sidebar
    {
        EventHandler NotImplemented;
        public OrbIt game;
        public Room room { get { return game.room; } }
        
        public UserInterface ui;
        private UserLevel _userLevel = UserLevel.Debug;
        public UserLevel userLevel
        {
            get { return _userLevel; }
            set
            {
                if (value == _userLevel) return;
                _userLevel = value;
            }
        }
        public bool CreatingGroup = false;
      public string ActiveGroupName;
        public Group GetActiveGroup()
        {
            if(string.IsNullOrEmpty(ActiveGroupName)) return room.masterGroup;
          return room.masterGroup.FindGroup(ActiveGroupName);
        }

        public Node ActiveDefaultNode
        {
            get
            {
                Group g = GetActiveGroup();
                if (g != null && g.defaultNode != null)
                    return g.defaultNode;
                return null;
            }
        }
        //public InspectorInfo ActiveInspectorParent;
        
        public Sidebar(UserInterface ui)
        {
            this.game = ui.game;
            //this.room = ui.game.room;
            this.ui = ui;
            NotImplemented = delegate {
                Console.WriteLine("Not Implemented. Take a hike.");
                //throw new NotImplementedException();
            };
            
        }

        public void Initialize()
        {

        }

        void btnDeleteGroup_Click(object sender, EventArgs e)
        {
                Group g = GetActiveGroup();
                if (g == null) return;
                if (g.Name.Equals("[G0]")) return;
                
                if (g.fullSet.Contains(room.targetNode)) room.targetNode = null;
                g.DeleteGroup();
        }
        
       
        public void BuildItemsPath(InspectorInfo item, List<InspectorInfo> itemspath)
        {
            InspectorInfo temp = item;
            itemspath.Insert(0, temp);
            while (temp.parentItem != null)
            {
                temp = temp.parentItem;
                itemspath.Insert(0, temp);
            }
        }
        public void ProcessConsoleCommand(String text)
        {
            text = text.Trim();

            if (text.Equals(""))
            {
                Console.WriteLine("No Command Provided");
                //consoletextbox.Text = "";
                return;
            }
            object currentObj = game.room;



            List<String> args = text.Split(' ').ToList();
            String methodname;
            if (args.Count > 0)
            {
                methodname = args.ElementAt(0);
                args.RemoveAt(0);
            }
            else
            {
                Console.WriteLine("No Command Provided");
                return;
            }

            MethodInfo methinfo = currentObj.GetType().GetMethod(methodname);

            if (methinfo == null || methinfo.IsPrivate)
            {
                Console.WriteLine("Invalid method specification.");
                return;
            }

            ParameterInfo[] paraminfos = methinfo.GetParameters();

            int paramNum = paraminfos.Length;
            object[] finalargs = new object[paramNum];

            for(int i = 0; i < paramNum; i++)
            {

                Type ptype = paraminfos[i].ParameterType;
                if (i >= args.Count)
                {
                    if (paraminfos[i].IsOptional)
                    {
                        finalargs[i] = Type.Missing;
                        continue;
                    }
                    Console.WriteLine("Parameter Inconsistenc[ies].");
                    return;
                }
                try
                {
                  finalargs[i] = TypeDescriptor.GetConverter(ptype).ConvertFromInvariantString(args[i]);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Casting exception: " + e.Message);
                    throw e;
                }

            }
            if (methinfo.IsStatic) currentObj = null;
            try
            {
                methinfo.Invoke(currentObj, finalargs);
            }
            catch (Exception e)
            {
                Console.WriteLine("Invoking exception: " + e.Message);
                throw e;
            }
        }
        
        public void Update()
        {
        }
    }
}
