using System;
using System.Collections.Generic;
using OrbitVR.Framework;

namespace OrbitVR.Processes {
  public delegate void ProcessMethod(Dictionary<dynamic, dynamic> args); // to be 'classoverloaded' later


  public class Process {
    protected bool _active = false;

    //removing these for now; processes can have child processes later
    //public List<Process> procs = new List<Process>();
    //Process parentprocess; //I bet you a coke -Dante (resolved section 33.32 of the skeet bible studies)


    public Dictionary<dynamic, dynamic> pargs;
    public Dictionary<dynamic, ProcessMethod> pmethods;
    //public event ProcessMethod OutOfBounds;

    public Dictionary<KeyAction, KeyBundle> processKeyActions = new Dictionary<KeyAction, KeyBundle>();

    public Process() {
      // / // / //
      //active = true;
    }

    public Room room {
      get { return OrbIt.Game.Room; }
    }

    public virtual bool active {
      get { return _active; }
      set {
        _active = value;
        if (value) OnActivate();
      }
    }
    //Todo: Local Reference.
    public ProcessManager Manager => OrbIt.Game.processManager;

    //public event Action OnUpdate;
    //public event Action OnDraw;
    public event ProcessMethod OnCreate;
    public event ProcessMethod OnDestroy;
    public event Action<Node, Node> OnCollision;

    protected void addProcessKeyAction(String name, KeyCodes k1, KeyCodes? k2 = null, KeyCodes? k3 = null,
                                       Action OnPress = null, Action OnRelease = null, Action OnHold = null) {
      KeyBundle keyBundle;
      if (k2 == null) keyBundle = new KeyBundle(k1);
      else if (k3 == null) keyBundle = new KeyBundle((KeyCodes) k2, k1);
      else keyBundle = new KeyBundle((KeyCodes) k3, (KeyCodes) k2, k1);

      var keyAction = new KeyAction(name, OnPress, new HashSet<KeyBundle>() {keyBundle});

      keyAction.releaseAction = OnRelease;
      keyAction.holdAction = OnHold;

      processKeyActions.Add(keyAction, keyBundle);
    }

    protected virtual void OnActivate() {}
    protected virtual void Create() {}
    public virtual void Update() {}
    public virtual void Draw() {}
    public virtual void Destroy() {}

    public void InvokeOnCollision(Node me, Node it) {
      if (OnCollision != null) OnCollision(me, it);
    }

    public void InvokeOnCreate() {
      if (OnCreate != null) OnCreate(pargs);
    }

    public void InvokeOnDestroy() {
      if (OnDestroy != null) OnDestroy(pargs);
    }

    /*
        public bool DetectKeyPress(Keys key)
        {
            return UserInterface.keybState.IsKeyDown(key) && UserInterface.oldKeyBState.IsKeyUp(key);
        }
        public bool DetectKeyRelease(Keys key)
        {
            return UserInterface.keybState.IsKeyUp(key) && UserInterface.oldKeyBState.IsKeyDown(key);
        }
        public bool DetectKeyDown(Keys key)
        {
            return UserInterface.keybState.IsKeyDown(key);
        }
        public void Add(Process p)
        {
            procs.Add(p);
            p.OnCreate();
        }
        public void Remove(Process p)
        {
            p.OnDestroy();
            procs.Remove(p);
        }
        */
  }
}