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
//

namespace OrbItProcs
{
    public class DelegateManager
    {
        //fuck, we are doing way too many checks. short shit out upon adding modInfo/args?
        public DelegateManager()
        { }


        public static void Mod (Dictionary<string, dynamic> args, ModifierInfo mi)
        {
            float o1;// = (float)mi.fpInfos["o1"].GetValue(); //if you'd like to use output in the calculation, include this call (o1 always exists)
            float m1 = Defaultered("m1", mi, 5);
            int mod = (int)(Defaultered("mod",args,10));
            o1 = m1 % mod;
            mi.fpInfos["o1"].SetValue(o1);
        }

        public static void TriangleArgs (Dictionary<string, dynamic> args, ModifierInfo mi)
        {
            float o1;// = (float)mi.fpInfos["o1"].GetValue(); //if you'd like to use output in the calculation, include this call (o1 always exists)
            float m1 = Defaultered("m1",mi, 5);
            int mod = (int)(Defaultered("mod", args, 10)) * 10;
            //int mod = 100;
            //scale = ((float)Math.Pow((double)(((int)position.X / 10) % mod) - (mod / 2),2)/2)/(mod*5)+0.5f;
            //scale = ((float)Math.Abs((double)(((int)position.X / 10) % mod) - (mod / 2)) / (mod * 5)) + 0.5f;
            o1 = (mod - (float)Math.Abs(((int)(m1) % (2 * mod) - mod))) / (mod / 5) + 0.5f;
            mi.fpInfos["o1"].SetValue(o1);
        }
        public static float Triangle(float num, float mod)
        {
            //return (mod - (float)Math.Abs(((int)(num) % (2 * mod) - mod))) / (mod / 5) + 0.5f;

            float a = Math.Abs(num) % (2 * mod);
            float b = a - mod;
            float c = Math.Abs(b);
            float d = mod - c;
            return d;

            //x = m - abs(i % (2*m) - m)
            //return mod - Math.Abs(num % (2 * mod) - mod);
        }

        public static void VelocityToOutput (Dictionary<string, dynamic> args, ModifierInfo mi)
        {
            Vector2 velocity = ((Node)mi.fpInfos["v1"].ob).body.velocity;
            float max = (Defaultered("max", args, 2f));
            float min = (Defaultered("min", args, 0.1f));
            float highest = (Defaultered("highest", args, 20f));
            float o1 = velocity.Length() / highest;
            o1 = o1 * (max - min) + min;
            mi.fpInfos["v1"].SetValue(o1);

        }

        public static void VectorSine(Dictionary<string, dynamic> args, ModifierInfo mi)
        {
            Vector2 vector = ((Vector2)mi.fpInfos["v1"].GetValue());
            float amp = (Defaultered("amp", args, (OrbIt.ScreenHeight / 5)));
            float period = (Defaultered("period", args, (OrbIt.ScreenWidth / 4)));
            //float min = (Defaultered("min", args, 0.1f));
            //float highest = (Defaultered("highest", args, 20f));
            //float o1 = velocity.Length() / highest;
            //o1 = o1 * (max - min) + min;
            vector.Y = (float)Math.Sin(vector.X / (period / (Math.PI * 2))) * amp + OrbIt.ScreenHeight / 2;
            
            mi.fpInfos["v1"].SetValue(vector);

        }
        public static void VectorSineComposite(Dictionary<string, dynamic> args, ModifierInfo mi)
        {
            Vector2 vector = ((Vector2)mi.fpInfos["v1"].GetValue());
            float timer = (Defaultered("m1", mi, -9999));
            float amp = (Defaultered("amp", args, (OrbIt.ScreenHeight / 5)));
            float period = (Defaultered("period", args, (OrbIt.ScreenWidth / 4)));
            float vshift = (Defaultered("vshift", args, (OrbIt.ScreenHeight / 2)));
            int times = (int)(Defaultered("composite", args, 2));
            //float min = (Defaultered("min", args, 0.1f));
            //float highest = (Defaultered("highest", args, 20f));
            //float o1 = velocity.Length() / highest;
            //o1 = o1 * (max - min) + min;
            //vector.Y = (float)Math.Sin(vector.X / (period / (Math.PI * 2))) * amp + Game1.sHeight / 2;

            //vector.Y = SineComposite(vector.X, amp, period, vshift, times);
            //float test = args["test"];
            //test++;
            //args["test"] = test;
            float x = timer;
            if (x == -9999)
            {
                x = vector.X;
            }

            args["yval"] = SineComposite(x, amp, period, vshift, times);

            mi.fpInfos["v1"].SetValue(vector);

        }
        
        public static float SineComposite(float x, float amp, float period, float vshift, int times)
        {
            period = (float)(Math.PI * 2) / period;
            float y = 1;
            for(int i = 0; i < times; i++)
            {
                y = y * (float)(Math.Sin(x * period / Math.Pow(2,i)));
            }
            y = y * amp + vshift;
            //Console.WriteLine(y);
            return y;

        }


        public static void ChangeArg(Node parent, string infoname, string argname, object value)
        {
            if (parent != null
                    && parent.HasComp<Modifier>()
                    && parent.Comp<Modifier>().modifierInfos.ContainsKey(infoname)
                    && parent.Comp<Modifier>().modifierInfos[infoname].args.ContainsKey(argname))
            {
                parent.Comp<Modifier>().modifierInfos[infoname].args[argname] = value;
            }
        }

        //failed generic experiment -- for now
        /*
        public T SuppliedOrDefault<T>(string id, ModifierInfo mi, T defaultval)
        {
            T ret;
            if (mi.fpInfos.ContainsKey(id))
            {
                var val = mi.fpInfos[id].GetValue();
                if (typeof(T) == val.GetType())
                {
                    return Convert.ChangeType(val, typeof(T));
                }
                else
                {

                }
            }
            else
            {
                ret = defaultval;
            }
            return ret;

        }
        */
        public static float Defaultered(string id, Dictionary<string, dynamic> args, float defaultval)
        {
            float ret;
            if (args.ContainsKey(id))
            {
                var val = args[id];
                if (val.GetType() == typeof(float) || val.GetType() == typeof(int))
                {
                    return (float)val;
                }
                else if (val.GetType() == typeof(Vector2))
                {
                    Vector2 vect = (Vector2)(val);
                    ret = (vect.X + vect.Y) / 10;
                }
                else
                {
                    ret = defaultval;
                }
            }
            else
            {
                ret = defaultval;
            }
            return ret;

        }

        public static float Defaultered(string id, ModifierInfo mi, float defaultval)
        {
            float ret;
            if (mi.fpInfos.ContainsKey(id))
            {
                var val = mi.fpInfos[id].GetValue();
                if (val.GetType() == typeof(float))
                {
                    //Console.WriteLine(val.GetType());
                    return (float)val;
                }
                else if (val.GetType() == typeof(int))
                {
                    return (int)val;
                }
                else if (val.GetType() == typeof(Vector2))
                {
                    Vector2 vect = (Vector2)(val);
                    ret = (vect.X + vect.Y) / 10;
                }
                else
                {
                    ret = defaultval;
                }
            }
            else
            {
                ret = defaultval;
            }
            return ret;

        }
        public static float checkFloat(string id, ModifierInfo mi)
        {
            float ret = 0;
            //Type t = mi.fpInfos[id].GetValue().GetType();
            var val = mi.fpInfos[id].GetValue();
            if (val is Vector2) //not sure if this works, might need a typeof
            {
                Vector2 vect = (Vector2)(val);
                ret = (vect.X + vect.Y) / 10;
            }
            else
            {
                ret = (float)val;
            }
            return ret;
        }
    }
}
