using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Collections.ObjectModel;

using Color = SharpDX.Color;
using SharpDX;
using SharpOVR;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.DXGI;
using SharpDX.Toolkit.Content;

namespace OrbItProcs
{
    public class Testing
    {
        public static Room room;
        public Redirector redirector;
        public static DateTime before;
        public static bool timerStarted = false;

        public static int count = 0;
        public static int modcount = 10;
        public static bool usingMod = false;
        //public static Stopwatch this[string name]
        //{
        //    get
        //    {
        //        if (!stopwatches.ContainsKey(name)) stopwatches[name] = new Stopwatch();
        //        return stopwatches[name];
        //    }
        //    set
        //    {
        //        stopwatches[name] = value;
        //    }
        //}

        public static Stopwatch stopwatch = new Stopwatch();
        public static Stopwatch dummy = new Stopwatch();

        public static Dictionary<string, Stopwatch> stopwatches = new Dictionary<string, Stopwatch>();
        //public static Dictionary<string, bool> timerStarteds = new Dictionary<string, bool>();

        public Testing()
        {
            room = OrbIt.game.room;

            Redirector.PopulateDelegatesAll();
            redirector = new Redirector();

            //obints.CollectionChanged += (s, e) => { };
        }

        public ObservableHashSet<int> obints = new ObservableHashSet<int>();
        public HashSet<int> hints = new HashSet<int>();
        public List<int> lints = new List<int>();
        public ObservableCollection<int> oblist = new ObservableCollection<int>();

        public static void shittest()
        {
            Shite ss = new Shite(Shite.pieces.Button | Shite.pieces.Switch);
            Console.WriteLine(ss.win);
            ss = new Shite(Shite.pieces.Button | Shite.pieces.Lever);
            Console.WriteLine(ss.win);
            ss = new Shite(Shite.pieces.Switch | Shite.pieces.Lever);
            Console.WriteLine(ss.win);
            ss = new Shite(Shite.pieces.Button);
            Console.WriteLine(ss.win);
            ss = new Shite(Shite.pieces.Lever);
            Console.WriteLine(ss.win);
            ss = new Shite(Shite.pieces.Switch);
            Console.WriteLine(ss.win);
            ss = new Shite(Shite.pieces.Button | Shite.pieces.Switch | Shite.pieces.Lever);
            Console.WriteLine(ss.win);

            return;
            dynamic s = new { s = "s" };
            try
            {
                s = shit(s);
            }
            catch (StackOverflowException e)
            {
                shittest();
            }
            //Console.WriteLine(s.GetType());
        }
        static int c = 0;
        static dynamic shit(dynamic other)
        {
            //if (other == null) return null;
            dynamic v = null;
            string type = other.GetType().ToString();
            Console.Write(c + ": " + other.GetType());
            if (c++ == 200)
                return null;
            v = new { a = other, num = c };
            Console.WriteLine("\t" + (other.GetType() == v.GetType()));
            var vv = shit(v);
            if (v != null)
                Console.WriteLine(v.GetType());
            
            return v;
        }
        public static void sawtoothTest()
        {
            for (int i = -10; i <= 10; i++)
            {

                float f = i / 10f;
                float val = GMath.Sawtooth(f, 1);
                Console.WriteLine("st: {0} % 1 = {1}", f, val);
            }
        }
        public static void modulusTest()
        {
            for(int i = -10; i <= 10; i++)
            {
                Console.WriteLine("{0} % 5 = {1}", i, i % 5);
            }
        }

        public static void modInc()
        {
            count++;
        }
        public static Stopwatch w(string name)
        {
            if (usingMod && count % modcount != 0) return dummy;
            if (!stopwatches.ContainsKey(name)) stopwatches[name] = new Stopwatch();
            return stopwatches[name];
        }

        public static void RestartTimer(string timer)
        {
            if (usingMod && count % modcount != 0) return;
            if (!stopwatches.ContainsKey(timer)) stopwatches[timer] = new Stopwatch();
            else stopwatches[timer].Restart();
        }

        public static void PrintTimer(string timer, bool reset = true)
        {
            if (usingMod && count % modcount != 0) return;
            if (!stopwatches.ContainsKey(timer)) { /*Console.WriteLine(timer + " doesn't have a stopwatch.");*/ return; }
            Console.WriteLine(timer + "\t:\t" + stopwatches[timer].Elapsed + "\t" + stopwatches[timer].ElapsedMilliseconds + "\t" +stopwatches[timer].ElapsedTicks);
            if (reset) stopwatches[timer].Reset();
        }

        public static float ByteToMegabyte(int i)
        {
            return (float)i / 1024f / 1024f;
        }

        public static void OldStartTimer()
        {
            timerStarted = true;
            before = DateTime.Now;
        }
        public static void OldStopTimer(string message = "")
        {
            DateTime after = DateTime.Now;

            if (!timerStarted)
            {
                //Console.WriteLine("Timer was not previously started, so timer cannot be stopped.");
                return;
            }
            if (before == null) return;
            int mill = after.Millisecond - before.Millisecond;
            if (mill < 0) mill += 1000;
            Console.WriteLine(" {0}: {1}", message, mill);
            timerStarted = false;
        }
        private static int standardizedCounter = 0;

        public static void StandardizedTesting2(int max)
        {
            if (++standardizedCounter <= max)
            {
                Room room = OrbIt.game.room;
                Dictionary<dynamic, dynamic> standardDictionary = new Dictionary<dynamic, dynamic>(){
                    { nodeE.position, new Vector2(room.worldWidth / 2, room.worldHeight / 2) },
                    //{ typeof(gravity, true },
                };
                for (int i = 0; i < 10; i++)
                    OrbIt.game.room.spawnNode(standardDictionary);
            }
            else
            {
                OrbIt.game.Exit();
            }
        }
        
        public static void StandardizedTesting(int max)
        {

            if (++standardizedCounter <= max)
            {
                Room room = OrbIt.game.room;
                Dictionary<dynamic, dynamic> standardDictionary = new Dictionary<dynamic, dynamic>(){
                    { nodeE.position, new Vector2(room.worldWidth / 2, room.worldHeight / 2) },
                    //{ typeof(gravity, true },
                };

                OrbIt.game.room.spawnNode(standardDictionary);
            }
            else
            {
                OrbIt.game.Exit();
            }
        }

        public static void TestCountArray()
        {
            //Weird.
            int length = 1000000;
            int[] ints = new int[length];
            IndexArray<int> counted = new IndexArray<int>(new int[length]);
            List<int> list = new List<int>();
            w("count").Start();
            for (int i = 0; i < counted.index; i++)
            {
                counted.array[i] = i;
            }
            w("count").Stop();
            int len = ints.Length;
            w("array").Start();
            for (int i = 0; i < len; i++)
            {
                ints[i] = i;
            }
            w("array").Stop();

            w("listadd").Start();
            for (int i = 0; i < length; i++)
            {
                list.Add(i);
            }
            w("listadd").Stop();
            w("listmod").Start();
            for (int i = 0; i < length; i++)
            {
                list[i] = i+1;
            }
            w("listmod").Stop();
            
            PrintTimer("count");
            PrintTimer("array");
            PrintTimer("listadd");
            PrintTimer("listmod");
        }

        public static void IndexsGridsystem()
        {
            string[] ind = new string[room.masterGroup.fullSet.Count];
            int c = 0;
            Testing.w("oldindex").Start();
            for (int i = 0; i < 100; i++)
            {
                foreach (var n in room.masterGroup.fullSet)
                {
                    Tuple<int, int> t = room.gridsystemAffect.getIndexs(n.body);
                    if (i == 0)
                        ind[c] += t.Item1 + "," + t.Item2 + " ";
                }
            }
            Testing.PrintTimer("oldindex");
            Testing.w("newindex").Start();
            for (int i = 0; i < 100; i++)
            {
                foreach (var n in room.masterGroup.fullSet)
                {
                    Tuple<int, int> t = room.gridsystemAffect.getIndexsNew(n.body);
                    if (i == 0)
                        ind[c] += t.Item1 + "," + t.Item2 + " ";
                }
            }
            Testing.PrintTimer("newindex");
        }

        public void NotEvenOnce()
        {
            Action<string> d = OldStopTimer;
            Action<string> dd = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), d.Method);
            Type t = typeof(Node);
            MethodInfo mi = t.GetMethod("RemoveTag");
            Collision col = new Collision();
            //dynamic meth = Delegate.CreateDelegate(mi.)
        }

        public void Gridsystem()
        {
            for (int i = 0; i < 4; i++)
                room.gridsystemAffect.testRetrieve(10, 10, i);
        }

        public void TriangleTest2()
        {
            float mod = 50;
            for(int i = -200; i < 201; i++)
            {
                float output = DelegateManager.Triangle(i, mod);
                Console.WriteLine("{0} :  {1}  =  {2}", i, mod, output);
            }

        }

        public static void TestHues()
        {
            //float hue = 0;
            for(int i = 0; i < 360; i++)
            {
                Color col = ColorChanger.getColorFromHSV((float)i);
                Room room = OrbIt.game.room;
                float thickness = (float)room.worldWidth / 360f;
                room.camera.DrawLine(new Vector2(thickness * i, 0), new Vector2(thickness * i, room.worldHeight), thickness, col, Layers.Over5);
            }
        }


        public void IntToColor(int i)
        {
            int r = (i / (255 * 255)) % 255;
            int g = (i / 255) % 255;
            int b = i % 255;

            string s = string.Format("{0}\t{1}\t{2}", r, g, b);
            Console.WriteLine(s);

        }

        public void ColorsTest()
        {
            int max = 255 * 255 * 255;
            for(int i = max/2; i<max;i++)
            {
                if (i % 100 == 0) IntToColor(i);
            }
        }

        public void LoopTesting()
        {
            OldStartTimer();
            int it = 40000000;
            dynamic a = 0;
            for(int i = 0; i < it; i++)
            {
                a = a * a;
            }
            string output = "::" + it + " iterations."; 
            OldStopTimer(output);
        }

        private static int NewMethod(int a)
        {
            a++;
            return a;
        }


        public void ForLoops()
        {
            HashSet<int> ints = new HashSet<int>();
            for(int i = 0; i< 100000; i++)
            {
                ints.Add(i);
            }
            int count = 0;
            OldStartTimer();
            foreach (int i in ints)
            {
                count += i;
            }
            OldStopTimer("foreach:");
            Console.WriteLine(count);
            OldStartTimer();
            ints.ToList().ForEach(i => count += i);
            OldStopTimer("tolist:");
            Console.WriteLine(count);
        }

        public void KeyPresses(KeyManager kbs)
        {
            OldStartTimer();
            kbs.Update();
            OldStopTimer("keypresses:");
        }

        public void KeyManagerTest(Action a)
        {
            OldStartTimer();
            a();
            OldStopTimer("KeyManager");
        }

        public void NormalizeTest()
        {
            OldStartTimer();
            int count = 0;
            for(int i = 0; i < 100; i++)
            {
                
                Vector2 v = new Vector2((float)Utils.random.Next(10000) / (float)Utils.random.Next(10000), (float)Utils.random.Next(10000) / (float)Utils.random.Next(10000));
                Vector2 vv = v;
                v.Normalize();
                VMath.NormalizeSafe(ref vv);
                Console.WriteLine("B: x:{0} , y:{1}", vv.X, vv.Y);
                Console.WriteLine("c: v:{0} , vv:{1}", v.Length(), vv.Length());
                if (v != vv)
                {
                    //Console.WriteLine("v:{0}\n : vv:{1}",v,vv);
                    count++;
                }
            }
            Console.WriteLine("COUNT-----------------:" + count);
            OldStopTimer("Normalizes:");
        }

        public void TestHashSet()
        {
            OldStartTimer();
            for (int i = 0; i < 100000; i++)
            {
                hints.Add(i);
            }
            OldStopTimer("HashSetAdd");
            OldStartTimer();
            for (int i = 0; i < 100000; i++)
            {
                obints.Add(i);
            }
            OldStopTimer(" ObsHashSetAdd");
            
            OldStartTimer();
            for (int i = 0; i < 100000; i++)
            {
                lints.Add(i);
            }
            OldStopTimer(" ListAdd");

            OldStartTimer();
            for (int i = 0; i < 100000; i++)
            {
                oblist.Add(i);
            }
            OldStopTimer(" ObsListAdd");
            Console.WriteLine("");
            
            ///
            OldStartTimer();
            for (int i = 0; i < 100000; i++)
            {
                hints.Contains(i);
            }
            OldStopTimer("HashSetContains");
            OldStartTimer();
            for (int i = 0; i < 100000; i++)
            {
                obints.Contains(i);
            }
            OldStopTimer(" ObsHashSetContains");
            /*
            StartTimer();
            for (int i = 0; i < 100000; i++)
            {
                //lints.Contains(i);
            }
            StopTimer("ListContains");
            */
            Console.WriteLine("");

            ///
            OldStartTimer();
            for (int i = 0; i < 100000; i++)
            {
                hints.Remove(i);
            }
            OldStopTimer("HashSetRemove");
            OldStartTimer();
            for (int i = 0; i < 100000; i++)
            {
                obints.Remove(i);
            }
            OldStopTimer(" ObsHashSetRemove");
            /*
            StartTimer();
            for (int i = 0; i < 100000; i++)
            {
                //lints.Remove(i);
            }
            StopTimer("ListRemove");
            */
            Console.WriteLine("");

            HashSet<int> excludeset = new HashSet<int>() { 5 };
            //HashSet<int> newset;
            List<int> excludeInt = new List<int>() { 5 };
            OldStartTimer();
            for (int i = 0; i < 1000; i++)
            {
                //hints.ExceptWith(excludeset);
            }

        }

        public void WhereTest()
        {
            var originalSet = new HashSet<int>(){ 1, 2, 3, 4, 5 };
            var excludeListForA = new HashSet<int>() { 1, 2, 3 }; // <-- Note that A is in it's own list!

            // To update D and E:
            var setToActOn = originalSet.Where(element => !excludeListForA.Contains(element));
            foreach (int i in setToActOn)
            {
                Console.WriteLine(i);
            }
        }

        public void TriangleTest()
        {
            int mod = 10;
            for(int i = 0; i < 100; i++)
            {
                Console.WriteLine("{0} : {1}", i, DelegateManager.Triangle(i, mod));
            }

        }

        public void TestRedirect()
        {

            Movement move = new Movement();
            Collision col = new Collision();
            Node nod = new Node(OrbIt.game.room);
            move.active = true;
            col.active = true;
            nod.active = true;

            Console.WriteLine(move.active + " " + col.active + " " + nod.active);

            

            redirector.AssignObjectToPropertySet("active", move);
            redirector.active = false;

            redirector.AssignObjectToPropertySet("active", col);
            redirector.active = false;

            redirector.AssignObjectToPropertySet("active", nod);
            redirector.active = false;

            Console.WriteLine(move.active + " " + col.active + " " + nod.active);

        }

        public void TestOnClick()
        {
            //testing to see how long it takes to generate all the getter/setter delegates

            object transformobj = room.defaultNode.body;
            dynamic nodedynamic = room.defaultNode;
            List<Func<Node, float>> delList = new List<Func<Node, float>>();
            //float total = 0;

            Movement movement = new Movement();

            //redirector.TargetObject = movement;
            //redirector.PropertyToObject["active"] = movement;
            redirector.AssignObjectToPropertiesAll(movement);
            PropertyInfo pinfo = movement.GetType().GetProperty("active");
            //Action<object, object> movementsetter = redirector.setters[typeof(Movement)]["active"];
            //Console.WriteLine(":::" + movement.active);
            //bool a = redirector.active;
            //bool a = false;

            OldStartTimer();
            for (int i = 0; i < 100000; i++)
            {
                //if (i > 0) if (i > 1) if (i > 2) if (i > 3) if (i > 4) total++;

                //delList.Add(getDel);
                //float slow = (float)minfo.Invoke((Transform)transformobj, new object[] { });
                //float mass = getDel(room.defaultNode);
                //float mass2 = getDel((Transform)transformobj); //doesn't work because it's of type Object at compile time
                //float mass2 = getDel(nodedynamic);
                //total += mass;
                //gotten = room.defaultNode.GetComponent<Movement>(); //generic method to grab components
                //gotten = room.defaultNode.comps[comp.movement];
                //bool act = gotten.active;
                //gotten.active = true;
                redirector.active = false; //21m(impossible)... 24m(new) ... 19m (newer) ... 16m(newest)
                //a = redirector.active;
                //pinfo.SetValue(movement, false, null); //34m
                //movementsetter(movement, false); //4m(old)......... 6m(new)
                //movement.active = false;

            }
            //Movement move = room.defaultNode.comps[comp.movement];
            OldStopTimer();
            //Console.WriteLine(total);
            /* //this code won't run right now, but it represents the ability to make a specific generic method based on type variables from another generic method, and then invoke it... (this is slow)
            MethodInfo method = GetType().GetMethod("DoesEntityExist")
                             .MakeGenericMethod(new Type[] { typeof(Type) });
            method.Invoke(this, new object[] { dt, mill });
            */

            //gotten.fallOff();

            /////////////////////////////////////////////////////////////////////////////
        }

        public void OldTests()
        {
            //////////////////////////////////////////////////////////////////////////////////////
            List<int> ints = new List<int> { 1, 2, 3 };
            ints.ForEach(delegate(int i) { if (i == 2) ints.Remove(i); }); //COOL: NO ENUMERATION WAS MODIFIED ERROR
            ints.ForEach(delegate(int i) { Console.WriteLine(i); });

            MethodInfo testmethod = room.GetType().GetMethod("test");
            Action<Room, int, float, string> del = (Action<Room, int, float, string>)Delegate.CreateDelegate(typeof(Action<Room, int, float, string>), testmethod);
            del(room, 1, 0.3f, "Action worked.");

            Action<int, float, string> del2 = (Action<int, float, string>)Delegate.CreateDelegate(typeof(Action<int, float, string>), room, testmethod);
            //target is bound to 'room' in this example due to the overload of CreateDelegate used.
            del2(2, 3.3f, "Action worked again.");

            PropertyInfo pinfo = typeof(Component).GetProperty("active");
            MethodInfo minfo = pinfo.GetGetMethod();
            Console.WriteLine("{0}", minfo.ReturnType);

            Movement tester = new Movement();
            tester.active = true;

            bool ret = (bool)minfo.Invoke(tester, new object[] { }); //VERY expensive (slow)
            Console.WriteLine("{0}", ret);



            Func<Component, bool> delGet = (Func<Component, bool>)Delegate.CreateDelegate(typeof(Func<Component, bool>), minfo);
            Console.WriteLine("{0}", delGet(tester)); //very fast, and no cast or creation of empty args array required

            minfo = pinfo.GetSetMethod();
            //Console.WriteLine("{0} {1}", minfo.ReturnType, minfo.GetParameters()[0].ParameterType);

            Action<Component, bool> delSet = (Action<Component, bool>)Delegate.CreateDelegate(typeof(Action<Component, bool>), minfo);
            delSet(tester, false);
            Console.WriteLine("Here we go: {0}", delGet(tester));
            delSet(tester, true);
            /////////////////////////////////////////////////////////////////////////////////////////
            /*
            //gets all types that are a subclass of Component
            List<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(assembly => assembly.GetTypes())
                       .Where(type => type.IsSubclassOf(typeof(Component))).ToList();
            foreach (Type t in types) Console.WriteLine(t);
            */

            //room.defaultNode.Update(new GameTime()); //for testing

            //MODIFIER ADDITION
            /*
            room.defaultNode.addComponent(comp.modifier, true); //room.defaultNode.comps[comp.modifier].active = false;
            ModifierInfo modinfo = new ModifierInfo();
            modinfo.AddFPInfoFromString("o1", "scale", room.defaultNode);
            modinfo.AddFPInfoFromString("m1", "position", room.defaultNode);
            modinfo.AddFPInfoFromString("v1", "position", room.defaultNode);

            modinfo.args.Add("mod", 4.0f);
            modinfo.args.Add("times", 3.0f);
            modinfo.args.Add("test", 3.0f);
            
            //modinfo.delegateName = "Mod";
            //modinfo.delegateName = "Triangle";
            //modinfo.delegateName = "VelocityToOutput";
            //modinfo.delegateName = "VectorSine";
            modinfo.delegateName = "VectorSineComposite";

            room.defaultNode.comps[comp.modifier].modifierInfos["sinecomposite"] = modinfo;
            */

            ObservableHashSet<int> obints = new ObservableHashSet<int>();
            obints.CollectionChanged += (s, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (int i in e.NewItems)
                    {
                        Console.WriteLine("Added:" + i);
                    }
                }
            };
            obints.Add(6);
        }
    }
}
