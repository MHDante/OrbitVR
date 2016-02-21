using System;
using System.Collections.Generic;
using System.Linq;

namespace OrbItProcs {

  #region do not click i repeat

  // Summary:
  //     Identifies a particular key on a keyboard.
  //
  // Parameters:
  //   A:
  //     A key
  //
  //   Add:
  //     Add key
  //
  //   Apps:
  //     Applications key
  //
  //   Attn:
  //     Attn key
  //
  //   B:
  //     B key
  //
  //   Back:
  //     BACKSPACE key
  //
  //   BrowserBack:
  //     Windows 2000/XP: Browser Back key
  //
  //   BrowserFavorites:
  //     Windows 2000/XP: Browser Favorites key
  //
  //   BrowserForward:
  //     Windows 2000/XP: Browser Forward key
  //
  //   BrowserHome:
  //     Windows 2000/XP: Browser Start and Home key
  //
  //   BrowserRefresh:
  //     Windows 2000/XP: Browser Refresh key
  //
  //   BrowserSearch:
  //     Windows 2000/XP: Browser Search key
  //
  //   BrowserStop:
  //     Windows 2000/XP: Browser Stop key
  //
  //   C:
  //     C key
  //
  //   CapsLock:
  //     CAPS LOCK key
  //
  //   ChatPadGreen:
  //     Green ChatPad key
  //
  //   ChatPadOrange:
  //     Orange ChatPad key
  //
  //   Crsel:
  //     CrSel key
  //
  //   D:
  //     D key
  //
  //   D0:
  //     Used for miscellaneous characters; it can vary by keyboard.
  //
  //   D1:
  //     Used for miscellaneous characters; it can vary by keyboard.
  //
  //   D2:
  //     Used for miscellaneous characters; it can vary by keyboard.
  //
  //   D3:
  //     Used for miscellaneous characters; it can vary by keyboard.
  //
  //   D4:
  //     Used for miscellaneous characters; it can vary by keyboard.
  //
  //   D5:
  //     Used for miscellaneous characters; it can vary by keyboard.
  //
  //   D6:
  //     Used for miscellaneous characters; it can vary by keyboard.
  //
  //   D7:
  //     Used for miscellaneous characters; it can vary by keyboard.
  //
  //   D8:
  //     Used for miscellaneous characters; it can vary by keyboard.
  //
  //   D9:
  //     Used for miscellaneous characters; it can vary by keyboard.
  //
  //   Decimal:
  //     Decimal key
  //
  //   Delete:
  //     DEL key
  //
  //   Divide:
  //     Divide key
  //
  //   Down:
  //     DOWN ARROW key
  //
  //   E:
  //     E key
  //
  //   End:
  //     END key
  //
  //   Enter:
  //     ENTER key
  //
  //   EraseEof:
  //     Erase EOF key
  //
  //   Escape:
  //     ESC key
  //
  //   Execute:
  //     EXECUTE key
  //
  //   Exsel:
  //     ExSel key
  //
  //   F:
  //     F key
  //
  //   F1:
  //     F1 key
  //
  //   F10:
  //     F10 key
  //
  //   F11:
  //     F11 key
  //
  //   F12:
  //     F12 key
  //
  //   F13:
  //     F13 key
  //
  //   F14:
  //     F14 key
  //
  //   F15:
  //     F15 key
  //
  //   F16:
  //     F16 key
  //
  //   F17:
  //     F17 key
  //
  //   F18:
  //     F18 key
  //
  //   F19:
  //     F19 key
  //
  //   F2:
  //     F2 key
  //
  //   F20:
  //     F20 key
  //
  //   F21:
  //     F21 key
  //
  //   F22:
  //     F22 key
  //
  //   F23:
  //     F23 key
  //
  //   F24:
  //     F24 key
  //
  //   F3:
  //     F3 key
  //
  //   F4:
  //     F4 key
  //
  //   F5:
  //     F5 key
  //
  //   F6:
  //     F6 key
  //
  //   F7:
  //     F7 key
  //
  //   F8:
  //     F8 key
  //
  //   F9:
  //     F9 key
  //
  //   G:
  //     G key
  //
  //   H:
  //     H key
  //
  //   Help:
  //     HELP key
  //
  //   Home:
  //     HOME key
  //
  //   I:
  //     I key
  //
  //   ImeConvert:
  //     IME Convert key
  //
  //   ImeNoConvert:
  //     IME NoConvert key
  //
  //   Insert:
  //     INS key
  //
  //   J:
  //     J key
  //
  //   K:
  //     K key
  //
  //   Kana:
  //     Kana key on Japanese keyboards
  //
  //   Kanji:
  //     Kanji key on Japanese keyboards
  //
  //   L:
  //     L key
  //
  //   LaunchApplication1:
  //     Windows 2000/XP: Start Application 1 key
  //
  //   LaunchApplication2:
  //     Windows 2000/XP: Start Application 2 key
  //
  //   LaunchMail:
  //     Windows 2000/XP: Start Mail key
  //
  //   Left:
  //     LEFT ARROW key
  //
  //   LeftAlt:
  //     Left ALT key
  //
  //   LeftControl:
  //     Left CONTROL key
  //
  //   LeftShift:
  //     Left SHIFT key
  //
  //   LeftWindows:
  //     Left Windows key
  //
  //   M:
  //     M key
  //
  //   MediaNextTrack:
  //     Windows 2000/XP: Next Track key
  //
  //   MediaPlayPause:
  //     Windows 2000/XP: Play/Pause Media key
  //
  //   MediaPreviousTrack:
  //     Windows 2000/XP: Previous Track key
  //
  //   MediaStop:
  //     Windows 2000/XP: Stop Media key
  //
  //   Multiply:
  //     Multiply key
  //
  //   N:
  //     N key
  //
  //   None:
  //     Reserved
  //
  //   NumLock:
  //     NUM LOCK key
  //
  //   NumPad0:
  //     Numeric keypad 0 key
  //
  //   NumPad1:
  //     Numeric keypad 1 key
  //
  //   NumPad2:
  //     Numeric keypad 2 key
  //
  //   NumPad3:
  //     Numeric keypad 3 key
  //
  //   NumPad4:
  //     Numeric keypad 4 key
  //
  //   NumPad5:
  //     Numeric keypad 5 key
  //
  //   NumPad6:
  //     Numeric keypad 6 key
  //
  //   NumPad7:
  //     Numeric keypad 7 key
  //
  //   NumPad8:
  //     Numeric keypad 8 key
  //
  //   NumPad9:
  //     Numeric keypad 9 key
  //
  //   O:
  //     O key
  //
  //   Oem8:
  //     Used for miscellaneous characters; it can vary by keyboard.
  //
  //   OemAuto:
  //     OEM Auto key
  //
  //   OemBackslash:
  //     Windows 2000/XP: The OEM angle bracket or backslash key on the RT 102 key
  //     keyboard
  //
  //   OemClear:
  //     CLEAR key
  //
  //   OemCloseBrackets:
  //     Windows 2000/XP: The OEM close bracket key on a US standard keyboard
  //
  //   OemComma:
  //     Windows 2000/XP: For any country/region, the ',' key
  //
  //   OemCopy:
  //     OEM Copy key
  //
  //   OemEnlW:
  //     OEM Enlarge Window key
  //
  //   OemMinus:
  //     Windows 2000/XP: For any country/region, the '-' key
  //
  //   OemOpenBrackets:
  //     Windows 2000/XP: The OEM open bracket key on a US standard keyboard
  //
  //   OemPeriod:
  //     Windows 2000/XP: For any country/region, the '.' key
  //
  //   OemPipe:
  //     Windows 2000/XP: The OEM pipe key on a US standard keyboard
  //
  //   OemPlus:
  //     Windows 2000/XP: For any country/region, the '+' key
  //
  //   OemQuestion:
  //     Windows 2000/XP: The OEM question mark key on a US standard keyboard
  //
  //   OemQuotes:
  //     Windows 2000/XP: The OEM singled/double quote key on a US standard keyboard
  //
  //   OemSemicolon:
  //     Windows 2000/XP: The OEM Semicolon key on a US standard keyboard
  //
  //   OemTilde:
  //     Windows 2000/XP: The OEM tilde key on a US standard keyboard
  //
  //   P:
  //     P key
  //
  //   Pa1:
  //     PA1 key
  //
  //   PageDown:
  //     PAGE DOWN key
  //
  //   PageUp:
  //     PAGE UP key
  //
  //   Pause:
  //     PAUSE key
  //
  //   Play:
  //     Play key
  //
  //   Print:
  //     PRINT key
  //
  //   PrintScreen:
  //     PRINT SCREEN key
  //
  //   ProcessKey:
  //     Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
  //
  //   Q:
  //     Q key
  //
  //   R:
  //     R key
  //
  //   Right:
  //     RIGHT ARROW key
  //
  //   RightAlt:
  //     Right ALT key
  //
  //   RightControl:
  //     Right CONTROL key
  //
  //   RightShift:
  //     Right SHIFT key
  //
  //   RightWindows:
  //     Right Windows key
  //
  //   S:
  //     S key
  //
  //   Scroll:
  //     SCROLL LOCK key
  //
  //   Select:
  //     SELECT key
  //
  //   SelectMedia:
  //     Windows 2000/XP: Select Media key
  //
  //   Separator:
  //     Separator key
  //
  //   Sleep:
  //     Computer Sleep key
  //
  //   Space:
  //     SPACEBAR
  //
  //   Subtract:
  //     Subtract key
  //
  //   T:
  //     T key
  //
  //   Tab:
  //     TAB key
  //
  //   U:
  //     U key
  //
  //   Up:
  //     UP ARROW key
  //
  //   V:
  //     V key
  //
  //   VolumeDown:
  //     Windows 2000/XP: Volume Down key
  //
  //   VolumeMute:
  //     Windows 2000/XP: Volume Mute key
  //
  //   VolumeUp:
  //     Windows 2000/XP: Volume Up key
  //
  //   W:
  //     W key
  //
  //   X:
  //     X key
  //
  //   Y:
  //     Y key
  //
  //   Z:
  //     Z key
  //
  //   Zoom:
  //     Zoom key
  public enum KeyCodes {
    None = 0,
    Back = 8,
    Tab = 9,
    Enter = 13,
    Pause = 19,
    CapsLock = 20,
    Kana = 21,
    Kanji = 25,
    Escape = 27,
    ImeConvert = 28,
    ImeNoConvert = 29,
    Space = 32,
    PageUp = 33,
    PageDown = 34,
    End = 35,
    Home = 36,
    Left = 37,
    Up = 38,
    Right = 39,
    Down = 40,
    Select = 41,
    Print = 42,
    Execute = 43,
    PrintScreen = 44,
    Insert = 45,
    Delete = 46,
    Help = 47,
    D0 = 48,
    D1 = 49,
    D2 = 50,
    D3 = 51,
    D4 = 52,
    D5 = 53,
    D6 = 54,
    D7 = 55,
    D8 = 56,
    D9 = 57,
    A = 65,
    B = 66,
    C = 67,
    D = 68,
    E = 69,
    F = 70,
    G = 71,
    H = 72,
    I = 73,
    J = 74,
    K = 75,
    L = 76,
    M = 77,
    N = 78,
    O = 79,
    P = 80,
    Q = 81,
    R = 82,
    S = 83,
    T = 84,
    U = 85,
    V = 86,
    W = 87,
    X = 88,
    Y = 89,
    Z = 90,
    LeftWindows = 91,
    RightWindows = 92,
    Apps = 93,
    Sleep = 95,
    NumPad0 = 96,
    NumPad1 = 97,
    NumPad2 = 98,
    NumPad3 = 99,
    NumPad4 = 100,
    NumPad5 = 101,
    NumPad6 = 102,
    NumPad7 = 103,
    NumPad8 = 104,
    NumPad9 = 105,
    Multiply = 106,
    Add = 107,
    Separator = 108,
    Subtract = 109,
    Decimal = 110,
    Divide = 111,
    F1 = 112,
    F2 = 113,
    F3 = 114,
    F4 = 115,
    F5 = 116,
    F6 = 117,
    F7 = 118,
    F8 = 119,
    F9 = 120,
    F10 = 121,
    F11 = 122,
    F12 = 123,
    F13 = 124,
    F14 = 125,
    F15 = 126,
    F16 = 127,
    F17 = 128,
    F18 = 129,
    F19 = 130,
    F20 = 131,
    F21 = 132,
    F22 = 133,
    F23 = 134,
    F24 = 135,
    NumLock = 144,
    Scroll = 145,
    LeftShift = 160,
    RightShift = 161,
    LeftControl = 162,
    RightControl = 163,
    LeftAlt = 164,
    RightAlt = 165,
    BrowserBack = 166,
    BrowserForward = 167,
    BrowserRefresh = 168,
    BrowserStop = 169,
    BrowserSearch = 170,
    BrowserFavorites = 171,
    BrowserHome = 172,
    VolumeMute = 173,
    VolumeDown = 174,
    VolumeUp = 175,
    MediaNextTrack = 176,
    MediaPreviousTrack = 177,
    MediaStop = 178,
    MediaPlayPause = 179,
    LaunchMail = 180,
    SelectMedia = 181,
    LaunchApplication1 = 182,
    LaunchApplication2 = 183,
    OemSemicolon = 186,
    OemPlus = 187,
    OemComma = 188,
    OemMinus = 189,
    OemPeriod = 190,
    OemQuestion = 191,
    OemTilde = 192,
    ChatPadGreen = 202,
    ChatPadOrange = 203,
    OemOpenBrackets = 219,
    OemPipe = 220,
    OemCloseBrackets = 221,
    OemQuotes = 222,
    Oem8 = 223,
    OemBackslash = 226,
    ProcessKey = 229,
    OemCopy = 242,
    OemAuto = 243,
    OemEnlW = 244,
    Attn = 246,
    Crsel = 247,
    Exsel = 248,
    EraseEof = 249,
    Play = 250,
    Zoom = 251,
    Pa1 = 253,
    OemClear = 254,
    LeftClick = 255,
    MiddleClick = 256,
    RightClick = 257,
    ForwardClick = 258,
    BackClick = 259,
  }

  #endregion

  public enum Mouses {
    LeftClick = 255,
    MiddleClick = 256,
    RightClick = 257,
    ForwardClick = 258,
    BackClick = 259,
  }

  public struct KeyBundle {
    //public static KeyMouse km(KeyCodes k ) { return new KeyMouse(k); } //to be deleted
    public KeyCodes? effectiveKey;
    public KeyCodes? mod1;
    public KeyCodes? mod2;

    public KeyBundle(KeyCodes effectiveKey, KeyCodes? mod1 = null, KeyCodes? mod2 = null) {
      this.effectiveKey = effectiveKey;

      this.mod1 = mod1;
      this.mod2 = mod2;

      if (mod1 == null) {
        if (mod2 != null) {
          this.mod1 = mod1;
          this.mod2 = mod2;
          throw new ArgumentException();
        }
      }
      else {
        if (mod2 != null) {
          //this.mod2 = mod2;
          if (mod1 > mod2) {
            this.mod1 = mod2;
            this.mod2 = mod1;
          }
        }
      }
    }

    public KeyBundle(List<KeyCodes> list) {
      effectiveKey = mod1 = mod2 = null;
      for (int i = list.Count - 1; i >= 0; i--)
        //for (int i = 0; i < list.Count; i++)
      {
        if (effectiveKey == null) effectiveKey = list.ElementAt(i);
        else if (mod1 == null) mod1 = list.ElementAt(i);
        else if (mod2 == null) mod2 = list.ElementAt(i);
      }
    }

    public bool isKey() {
      return (int) effectiveKey < 255;
    }
  }

  public class KeyAction {
    public HashSet<KeyBundle> bundles;
    public Action holdAction;
    public string name = "Missing Name";
    public Action pressAction;
    public Action releaseAction;

    public KeyAction(string name, Action action, HashSet<KeyBundle> bundles) {
      this.name = name;
      this.pressAction = action;
      if (bundles == null) throw new ArgumentException("bundles was null - {0}", name);
      this.bundles = bundles;
    }

    //    this.bundles.Add(bundle);
    //    this.bundles = new HashSet<KeyBundle>();
    //    this.pressAction = action;
    //    this.name = name;
    //{

    //public KeyAction(string name, Action action, KeyBundle bundle)
    //}
    //public KeyAction(HashSet<KeyBundle> bundles, string name, Action pressAction = null, Action releaseAction = null, Action holdAction = null)
    //{
    //    this.name = name;
    //    this.pressAction = pressAction;
    //    this.releaseAction = releaseAction;
    //    this.holdAction = holdAction;
    //    if (bundles == null) throw new ArgumentException("bundles was null - {0}", name);
    //    this.bundles = bundles;
    //}
    //public KeyAction(KeyBundle bundles, string name, Action pressAction = null, Action releaseAction = null, Action holdAction = null)
    //{
    //    this.name = name;
    //    this.pressAction = pressAction;
    //    this.releaseAction = releaseAction;
    //    this.holdAction = holdAction;
    //    this.bundles = new HashSet<KeyBundle>();
    //    this.bundles.Add(bundles);
    //}
  }

  public enum KeySwitchMethod {
    Overwrite,
    Switch,
    TempSwitch,
    TempOverwrite,
  }

  //================================================== KEY MANAGER ==========================================
  public class KeyManager {
    public static int HoldCounter = 0;

    public static KeyboardState newKeyboardState, oldKeyboardState;

    public static MouseState newMouseState, oldMouseState;

    public Dictionary<KeyBundle, KeyAction> _Keybinds = new Dictionary<KeyBundle, KeyAction>();

    public bool MouseInGameBox = true;

    public List<Process> PermanentProcesses = new List<Process>();

    public Dictionary<KeyBundle, KeyAction> PressedBundles = new Dictionary<KeyBundle, KeyAction>();
    public List<KeyCodes> PressedKeys = new List<KeyCodes>();

    public Stack<KeyCodes> PressedKeysS = new Stack<KeyCodes>(); //max of three KeyMouse detected at a time

    //don't call me generic
    public Dictionary<Process, List<Tuple<KeyBundle, KeyAction>>> ReplacedBundles =
      new Dictionary<Process, List<Tuple<KeyBundle, KeyAction>>>();

    public Process TemporaryProcess = null;

    public UserInterface ui;

    public KeyManager(UserInterface ui, Dictionary<KeyBundle, KeyAction> Keybinds = null) {
      this.ui = ui;
      //this.newKeyboardState = Keyboard.GetState();
      oldKeyboardState = Keyboard.GetState();
      oldMouseState = Mouse.GetState();

      if (Keybinds != null) {
        this.Keybinds = Keybinds;
      }
    }

    //20 references
    public Dictionary<KeyBundle, KeyAction> Keybinds {
      get { return _Keybinds; }
      set { _Keybinds = value; }
    }

    public void Add(string name, KeyBundle bundle, Action action, Action holdAction = null) {
      KeyAction ka = new KeyAction(name, action, new HashSet<KeyBundle>() {bundle});
      Keybinds.Add(bundle, ka);
    }

    public void Add(string name, HashSet<KeyBundle> bundles, Action action) {
      KeyAction ka = new KeyAction(name, action, bundles);

      foreach (KeyBundle b in bundles) {
        Keybinds.Add(b, ka);
      }
    }

    public void AddProcess(Process p, KeySwitchMethod switchmethod) {
      if (p != null && p.processKeyActions != null) {
        if (switchmethod.In(KeySwitchMethod.Switch, KeySwitchMethod.TempOverwrite, KeySwitchMethod.TempSwitch)) {
          throw new NotImplementedException("Don't make a fus, implement your bus.");
        }

        foreach (KeyAction a in p.processKeyActions.Keys) {
          Keybinds[p.processKeyActions[a]] = a;
        }
      }
    }

    public void AddProcess(ProcessManager pm, Process p, bool Temporary = true) {
      if (p == null) throw new SystemException("Process parameter was null"); //well ya see kids...
      if (p.processKeyActions == null) throw new SystemException("Process parameter had no keyactions");
      p.active = true;
      if (Temporary) {
        if (TemporaryProcess != p) {
          //remove current temporary process and reinstate the keybinds it had replaced when it was added
          RemoveTemporaryProcess(pm);

          ReplacedBundles[p] = new List<Tuple<KeyBundle, KeyAction>>();
          foreach (KeyAction a in p.processKeyActions.Keys) {
            //store previous keyaction in replacedbundles
            if (Keybinds.ContainsKey(p.processKeyActions[a])) {
              ReplacedBundles[p].Add(new Tuple<KeyBundle, KeyAction>(p.processKeyActions[a],
                Keybinds[p.processKeyActions[a]]));
            }
            //insert new keybind
            Keybinds[p.processKeyActions[a]] = a;
          }
          TemporaryProcess = p;
          if (!pm.activeProcesses.Contains(p))
            pm.activeProcesses.Add(p);
        }
      }
      else //permanent process
      {
        if (!PermanentProcesses.Contains(p)) {
          PermanentProcesses.Add(p);
          foreach (KeyAction a in p.processKeyActions.Keys) {
            //insert new keybind
            Keybinds[p.processKeyActions[a]] = a;
          }
        }
      }
      //if (ui.sidebar != null) ui.sidebar.UpdateProcessView();
    }

    public void RemoveTemporaryProcess(ProcessManager pm) {
      if (TemporaryProcess == null) return; //throw new SystemException("Temporary process was null");
      TemporaryProcess.active = false;
      foreach (KeyAction ka in TemporaryProcess.processKeyActions.Keys) {
        KeyBundle kb = TemporaryProcess.processKeyActions[ka];
        if (Keybinds.ContainsKey(kb) && ka == Keybinds[kb]) {
          Keybinds.Remove(TemporaryProcess.processKeyActions[ka]);
        }
      }
      if (ReplacedBundles.ContainsKey(TemporaryProcess)) {
        List<Tuple<KeyBundle, KeyAction>> list = ReplacedBundles[TemporaryProcess];
        foreach (Tuple<KeyBundle, KeyAction> tup in list) {
          if (Keybinds.ContainsKey(tup.Item1))
            continue; //maybe shouldn't do this; doesn't replace if something took the slot
          Keybinds.Add(tup.Item1, tup.Item2);
        }
      }
      pm.activeProcesses.Remove(TemporaryProcess);

      ReplacedBundles.Remove(TemporaryProcess);
      TemporaryProcess = null; //maybe disable temporary process? or should we let caller do that
    }


    // processes can now spawn their own keyactions
    //public void Add(KeyBundle keyarr, Process process, bool AddBothCombinations = false)
    //{
    //    Keybinds.Add(keyarr, process);
    //    if (AddBothCombinations && keyarr.mod2 != null) //adds the process to both combinations (123, 213)
    //    {
    //        KeyBundle kb = new KeyBundle(keyarr.mod1, keyarr.effectiveKey, keyarr.mod2);
    //        Keybinds.Add(kb, process);
    //    }
    //}

    //public void Add(KeyBundle keyarr, Action action, bool AddBothCombinations = false)
    //{
    //    Keybinds.Add(keyarr, action);
    //    if (AddBothCombinations && keyarr.mod2 != null) //adds the action to both combinations (123, 213)
    //    {
    //        KeyBundle kb = new KeyBundle(keyarr.mod1, keyarr.effectiveKey, keyarr.mod2);
    //        Keybinds.Add(kb, action);
    //    }
    //}
    public void Update() {
      newKeyboardState = Keyboard.GetState();
      newMouseState = Mouse.GetState();

      MouseInGameBox = true;

      if (newMouseState.X >= 0 && newMouseState.Y >= 0) //todo:check that the game window is active
      {
        ProcessKeyboard();
        ProcessMouse();
        ProcessHolds();
      }

      foreach (var p in PermanentProcesses) {
        //if (p.active)
        p.Update();
      }
      if (TemporaryProcess != null) // && TemporaryProcess.active
      {
        TemporaryProcess.Update();
      }

      oldKeyboardState = newKeyboardState;
      oldMouseState = newMouseState;
    }

    public void ProcessHolds() {
      foreach (KeyBundle kb in PressedBundles.Keys.ToList()) {
        if (kb.isKey()) {
          Keys k = (Keys) kb.effectiveKey;
          if (KeyDownEvent(k)) {
            if (PressedBundles[kb].holdAction != null) PressedBundles[kb].holdAction();
          }
          else {
            if (PressedBundles[kb].releaseAction != null) PressedBundles[kb].releaseAction();
            PressedBundles.Remove(kb);
          }
        }
        else {
          if (CheckMouseDown(kb.effectiveKey)) {
            if (PressedBundles[kb].holdAction != null) PressedBundles[kb].holdAction();
            //Console.WriteLine("Exec Hold action: " + HoldCounter++);
          }
          else {
            if (PressedBundles[kb].releaseAction != null) PressedBundles[kb].releaseAction();
            PressedBundles.Remove(kb);
          }
        }
      }
    }

    public bool CheckMouseDown(KeyCodes? keycodes) {
      if (keycodes == KeyCodes.LeftClick) {
        return newMouseState.LeftButton == ButtonState.Pressed;
      }
      else if (keycodes == KeyCodes.RightClick) {
        return newMouseState.RightButton == ButtonState.Pressed;
      }
      else if (keycodes == KeyCodes.MiddleClick) {
        return newMouseState.MiddleButton == ButtonState.Pressed;
      }
      else if (keycodes == KeyCodes.BackClick) {
        return newMouseState.XButton1 == ButtonState.Pressed;
      }
      else if (keycodes == KeyCodes.ForwardClick) {
        return newMouseState.XButton2 == ButtonState.Pressed;
      }
      return false;
    }

    public void ProcessMouse() {
      if (UserInterface.GameInputDisabled) return;
      DetectMouseButton(newMouseState.LeftButton, oldMouseState.LeftButton, KeyCodes.LeftClick);
      DetectMouseButton(newMouseState.RightButton, oldMouseState.RightButton, KeyCodes.RightClick);
      DetectMouseButton(newMouseState.MiddleButton, oldMouseState.MiddleButton, KeyCodes.MiddleClick);
      DetectMouseButton(newMouseState.XButton1, oldMouseState.XButton1, KeyCodes.BackClick);
      DetectMouseButton(newMouseState.XButton2, oldMouseState.XButton2, KeyCodes.ForwardClick);


      //bool pressbool = newMouseState.LeftButton == ButtonState.Pressed;// && oldMouseState.LeftButton == ButtonState.Released;
      //bool releasebool = newMouseState.LeftButton == ButtonState.Released;// && oldMouseState.LeftButton == ButtonState.Pressed;
      //Console.WriteLine("pressed : {0}   |    released : ", newMouseState.LeftButton);
    }

    public void DetectMouseButton(ButtonState newButtonState, ButtonState oldButtonState, KeyCodes press) {
      bool pressbool = newButtonState == ButtonState.Pressed && oldButtonState == ButtonState.Released;
      bool releasebool = newButtonState == ButtonState.Released && oldButtonState == ButtonState.Pressed;

      bool event1 = newButtonState != oldButtonState;


      if (pressbool || releasebool) {
        //Console.WriteLine("----------------");
      }
      if (event1 && !UserInterface.tomShaneWasClicked) {
        //Console.WriteLine(newButtonState);

        if (newButtonState == ButtonState.Pressed) {
          KeyCodes kc = (KeyCodes) press;
          //Console.WriteLine("Yp");
          if (!PressedKeys.Contains(kc) && PressedKeys.Count < 3) {
            PressedKeys.Add(kc);
            TryAction();
            //Console.WriteLine("New");
          }
          //PrintPressedKeys();
          //Console.WriteLine("key__press");
        }
        if (newButtonState == ButtonState.Released) {
          KeyCodes kc = (KeyCodes) press;
          //while (PressedKeys.ElementAt(PressedKeys.Count) != kc) PressedKeys.Pop();
          //PressedKeys.Pop();
          PressedKeys.Remove(kc);

          //Console.WriteLine("Keyrelease");
          //PrintPressedKeys(true);
        }
      }
    }


    public void ProcessKeyboard() {
      foreach (Keys k in Enum.GetValues(typeof (Keys))) {
        if (KeyPressEvent(k)) {
          KeyCodes kc = (KeyCodes) k;
          //Console.WriteLine(kc); //======================//======================//======================//======================//======================
          if (!PressedKeys.Contains(kc) && PressedKeys.Count < 3) {
            //PressedKeys.Push(kc);
            PressedKeys.Add(kc);
            TryAction();
          }
          //PrintPressedKeys();
        }
      }

      for (int i = PressedKeys.Count - 1; i >= 0; i--) {
        KeyCodes key = PressedKeys.ElementAt(i);
        if (KeyReleaseEvent((Keys) key)) {
          //while (PressedKeys.ElementAt(PressedKeys.Count) != key) PressedKeys.Pop();
          //PressedKeys.Pop();
          PressedKeys.Remove(key);
          //PrintPressedKeys(true);
          break;
        }
      }
    }

    public void TryAction() {
      KeyBundle kb = new KeyBundle(PressedKeys);
      string m1 = "none";
      string m2 = "none";
      if (kb.mod1 != null) m1 = ((KeyCodes) kb.mod1).ToString();
      if (kb.mod2 != null) m2 = ((KeyCodes) kb.mod2).ToString();

      //Console.WriteLine("ex: {0}\t\tmod1: {1}\t\tmod2: {2}", kb.effectiveKey, m1, m2);
      //Console.WriteLine("ex: {0}\t\tmod1: {1}\t\tmod2: {2} \t\t extra: {3}", PressedKeys.ElementAt(0), PressedKeys.ElementAt(1), PressedKeys.ElementAt(2), PressedKeys.ElementAt(3));
      //Console.WriteLine("ex: {0}", PressedKeys.Count);


      if (!Keybinds.ContainsKey(kb)) {
        KeyCodes? temp = null;

        if (kb.mod2 != null) {
          temp = kb.mod2;
          kb.mod2 = null;
        }

        if (!Keybinds.ContainsKey(kb)) {
          if (kb.mod1 != null) kb.mod1 = temp;
          if (!Keybinds.ContainsKey(kb)) {
            kb.mod1 = null;
            if (!Keybinds.ContainsKey(kb)) {
              return;
            }
          }
        }
      }

      KeyAction ka = Keybinds[kb];
      if (ka != null) {
        if (MouseInGameBox ||
            (kb.effectiveKey != KeyCodes.LeftClick && kb.effectiveKey != KeyCodes.RightClick &&
             kb.effectiveKey != KeyCodes.MiddleClick)) {
          if (ka.pressAction != null) {
            ka.pressAction();
          }
          //if (!PressedBundles.ContainsKey(kb)) 
          PressedBundles.Add(kb, ka); //exception
        }
      }
    }

    public void PrintPressedKeys(bool released = false) {
      string s = "";
      if (released) s += "Release: ";
      else s += "Pressed: ";

      //for (int i = PressedKeys.Count - 1; i >= 0; i--)
      for (int i = 0; i < PressedKeys.Count; i++) {
        s += PressedKeys.ElementAt(i) + " ";
      }

      Console.WriteLine(s);
    }

    public bool KeyPressEvent(Keys key) {
      return newKeyboardState.IsKeyDown(key) && !oldKeyboardState.IsKeyDown(key);
    }

    public bool KeyReleaseEvent(Keys key) {
      return !newKeyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyDown(key);
    }

    public bool KeyDownEvent(Keys key) {
      return newKeyboardState.IsKeyDown(key);
    }


    public void addGlobalKeyAction(String name, KeyCodes k1, KeyCodes? k2 = null, KeyCodes? k3 = null,
      Action OnPress = null, Action OnRelease = null, Action OnHold = null) {
      KeyBundle keyBundle;
      if (k2 == null) keyBundle = new KeyBundle(k1);
      else if (k3 == null) keyBundle = new KeyBundle((KeyCodes) k2, k1);
      else keyBundle = new KeyBundle((KeyCodes) k3, (KeyCodes) k2, k1);

      var keyAction = new KeyAction(name, OnPress, new HashSet<KeyBundle>() {keyBundle});

      keyAction.releaseAction = OnRelease;
      keyAction.holdAction = OnHold;

      Keybinds.Add(keyBundle, keyAction);
    }
  }
}