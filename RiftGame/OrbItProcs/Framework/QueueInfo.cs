using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrbItProcs
{
    public class QueueInfo
    {
        /*
        queuecount
        the type
        the queue
        string identifier
        the object of the queue
        the FPInfo of the field
        //*/
        private int _queuecount = 10;
        public int queuecount { get { return _queuecount; } set { _queuecount = value; } }

        private string _id;
        public string id { get { return _id; } set { _id = value; } }    
    
        private Queue<object> _queue = new Queue<object>();
        public Queue<object> queue { get { return _queue; } set { _queue = value; } }

        private Type _elementType = typeof(object);
        public Type elementType { get { return _elementType; } set { _elementType = value; } }

        private FPInfo _fpInfo;
        public FPInfo fpInfo { get { return _fpInfo; } set { _fpInfo = value; } }
        public object obj { get { return obj; } set { obj = value; } }

        public QueueInfo(
            FPInfo fpInfo,
            object obj,
            int queuecount,
            string id = ""
            //Type type = typeof(object)
            )
        {
            this.fpInfo = fpInfo;
            this.obj = obj;
            this.queuecount = queuecount;
            this.id = id;
            //this.elementType = type;
            this.queue = new Queue<object>();
        }

        public void TriggerQueueify()
        {
            Queueify(fpInfo.GetValue(obj));
        }

        public void Queueify(object ob)
        {
            
            if (queue.Count > queuecount)
            {
                queue.Dequeue();
            }
            queue.Enqueue(ob);
        }

        public object At(int i)
        {
            object o = queue.ElementAt(i);
            //var it = Convert.ChangeType(o, elementType);
            //T item = Convert.ChangeType(o, typeof(T));
            //T item = (T)o;
            return o;
            //return 
        }
    }
}
