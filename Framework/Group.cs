﻿using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace OrbitVR.Framework {
  public enum GroupState {
    off,
    drawingOnly,
    updatingOnly,
    on
  };

  public class Group {
    public static int GroupNumber = 2;

    public static Dictionary<int, Color> IntToColor = new Dictionary<int, Color>() {
      {0, Color.White},
      {1, Color.Green},
      {2, Color.Red},
      {3, Color.Blue},
      {4, Color.Purple},
      {5, Color.RosyBrown},
      {6, Color.YellowGreen},
      {7, Color.DarkGreen},
      {8, Color.LightBlue},
      {9, Color.Violet},
    };

    private Dictionary<string, Group> _childGroups;
    private Node _defaultNode = null;
    //public GroupState groupState { get; set; }

    private bool _Disabled = false;
    private string _Name;

    private ObservableHashSet<Link> _SourceLinks = new ObservableHashSet<Link>();

    private ObservableHashSet<Link> _TargetLinks = new ObservableHashSet<Link>();
    public Room room;

    public int GroupId { get; set; }
    public Group ParentGroup { get; set; }
    //
    public ObservableHashSet<Node> fullSet { get; set; }
    public ObservableHashSet<Node> entities { get; set; }
    public ObservableHashSet<Node> inherited { get; set; }

    public Dictionary<string, Group> childGroups {
      get { return _childGroups; }
      set {
        _childGroups = value;
        foreach (Group g in _childGroups.Values) {
          g.ParentGroup = this;
        }
      }
    }

    public Node defaultNode {
      get { return _defaultNode; }
      set {
        _defaultNode = value;
        if (value != null) {
          value.group = this;
          value.collision.RemoveCollidersFromSet();
        }
      }
    }

    public string Name {
      get { return _Name; }
      set {
        if (_Name != null && _Name.Equals("master")) return;
        _Name = value;
      }
    } //cannot rename main group
    public bool Spawnable { get; set; }

    public bool Disabled {
      get { return _Disabled; }
      set {
        _Disabled = value;
        if (value) {
          if (ParentGroup != null) {
            foreach (Node n in fullSet) {
              if (ParentGroup.inherited.Contains(n)) ParentGroup.inherited.Remove(n);
              if (room.CollisionManager.CollisionSetCircle.Contains(n.body))
                room.CollisionManager.CollisionSetCircle.Remove(n.body);
              if (room.CollisionManager.CollisionSetPolygon.Contains(n.body))
                room.CollisionManager.CollisionSetPolygon.Remove(n.body);
            }
            Spawnable = false;
          }
        }
        else {
          if (ParentGroup != null) {
            foreach (Node n in fullSet) {
              ParentGroup.inherited.Add(n);
              n.collision.UpdateCollisionSet();
            }
            Spawnable = true;
          }
        }
      }
    }

    public ObservableHashSet<Link> SourceLinks {
      get { return _SourceLinks; }
      set { _SourceLinks = value; }
    }

    public ObservableHashSet<Link> TargetLinks {
      get { return _TargetLinks; }
      set { _TargetLinks = value; }
    }

    public List<Group> groupPath { get; set; }

    public Group()
      : this(null) {}

    public Group(Room room, Node defaultNode = null, string Name = "", bool Spawnable = true,
                 ObservableHashSet<Node> entities = null) {
      this.room = room ?? OrbIt.Game.Room;

      GroupId = -1;
      this.defaultNode = defaultNode ?? this.room.DefaultNode;
      this.entities = entities ?? new ObservableHashSet<Node>();
      this.inherited = new ObservableHashSet<Node>();
      this.fullSet = new ObservableHashSet<Node>();
      if (entities != null) {
        foreach (Node e in entities) {
          fullSet.Add(e);
        }
      }

      //this.groupState = groupState;
      this.Spawnable = Spawnable;
      this.childGroups = new Dictionary<string, Group>();
      this.entities.CollectionChanged += entities_CollectionChanged;
      this.inherited.CollectionChanged += entities_CollectionChanged;

      if (Name.Equals("")) {
        this.GroupId = GroupNumber;
        Name = "Group" + GroupNumber; //maybe a check that the name is unique
        GroupNumber++;
      }
      this.Name = Name;

      groupPath = new List<Group>();
    }


    void entities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
      if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
        //bool ui = OrbIt.ui != null && OrbIt.ui.sidebar.cbListPicker != null;
        foreach (Node n in e.NewItems) {
          if (ParentGroup != null && !ParentGroup.entities.Contains(n)) {
            ParentGroup.inherited.Add(n);
          }
          fullSet.Add(n);
        }
      }
      else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove) {
        foreach (Node n in e.OldItems) {
          if (sender != fullSet) {
            if (!entities.Contains(n) && !inherited.Contains(n)) {
              fullSet.Remove(n);
            }
          }
          if (ParentGroup != null && ParentGroup.inherited.Contains(n)) {
            ParentGroup.inherited.Remove(n);
          }
          if (n.group == this) n.group = null;
        }
      }
    }

    public void EmptyGroup() {
      //bool isold = OrbIt.ui.sidebar.cbListPicker.Text.Equals(Name); // if there is a crash in this line, we removed (room.game.IsOldUI && 
      foreach (Node n in fullSet.ToList()) {
        DeleteEntity(n);
        //if (isold)
        //{
        //    OrbIt.ui.sidebar.lstMain.Items.Remove(n);
        //    OrbIt.ui.sidebar.SyncTitleNumber(this);
        //}
      }
    }

    public void ForEachFullSet(Action<Node> action) {
      //fullSet.ToList().ForEach(action);
      foreach (var n in fullSet) // ToList()
      {
        action(n);
      }
    }

    //adds entity to current group and all parent groups
    public void IncludeEntity(Node entity) {
      entities.Add(entity);
      entity.group = this;
      if (entity.collision.active) {
        //room.CollisionSet.Add(entity);
        entity.collision.UpdateCollisionSet();
      }
      //if (OrbIt.ui.sidebar.cbListPicker.Text.Equals(Name)) // if there is a crash in this line, we removed (room.game.IsOldUI && 
      //{
      //    OrbIt.ui.sidebar.lstMain.Items.Add(entity);
      //    OrbIt.ui.sidebar.SyncTitleNumber(this);
      //}
    }

    //removes entity from current group and all child groups
    public void DiscludeEntity(Node entity) {
      entity.collision.RemoveCollidersFromSet();
      entities.Remove(entity);
      if (inherited.Contains(entity))
        inherited.Remove(entity);
      //fullSet.Remove(entity);
      foreach (Group g in groupPath) {
        g.inherited.Remove(entity);
        //if (g.fullSet.Contains(entity))
        //    System.Diagnostics.Debugger.Break();
      }
      //if (childGroups.Count > 0)
      //{
      //    foreach (Group childgroup in childGroups.Values.ToList())
      //    {
      //        if (childgroup.fullSet.Contains(entity))
      //            childgroup.DiscludeEntity(entity);
      //    }
      //}
    }

    //removes entity from all groups, starting from the highest root
    public void DeleteEntity(Node entity) {
      //entity.active = false;
      //Group root = this;
      //while (root.parentGroup != null)
      //{
      //    root = root.parentGroup;
      //}
      DiscludeEntity(entity);
      //DiscludeEntity(entity);
      //entities.Remove(entity);
      entity.OnDelete();
      entity.group = null;
    }

    public Group FindGroup(string name) {
      Group root = this;
      while (root.ParentGroup != null) {
        root = root.ParentGroup;
      }
      Group result = root.FindGroupRecurse(name);
      if (result != null) return result;
      return root;
    }

    private Group FindGroupRecurse(string name) {
      if (Name.Equals(name)) return this;
      if (childGroups.Count == 0) return null;

      foreach (Group g in childGroups.Values) {
        Group result = g.FindGroupRecurse(name);
        if (result != null) return result;
      }
      return null;
    }


    public void ForEachAllSets(Action<Node> action) {
      entities.ToList().ForEach(action);
      inherited.ToList().ForEach(action);
    }

    public void TraverseGroups() {
      foreach (Group g in childGroups.Values.ToList()) {
        g.TraverseGroups();
      }
    }

    //dunno about this
    public void RemoveFromChildrenDeep(Node toremove) {
      if (entities.Contains(toremove)) entities.Remove(toremove);
      if (childGroups.Count == 0) return;

      foreach (string s in childGroups.Keys.ToList()) {
        childGroups[s].RemoveFromChildrenDeep(toremove);
      }
    }

    //public void Update(GameTime gametime)
    //{
    //    if (groupState.In(GroupState.on, GroupState.updatingOnly))
    //    {
    //        entities.ToList().ForEach(delegate(Node n) { ((Node)n).Update(gametime); });
    //    }
    //}

    //public void Draw(SpriteBatch spritebatch)
    //{
    //    if (groupState.In(GroupState.on, GroupState.drawingOnly))
    //    {
    //        entities.ToList().ForEach(delegate(Node n) { ((Node)n).Draw(); });
    //    }
    //}

    public override string ToString() {
      return Name;
    }

    /*
        public Group FindGroup(string name)
        {
            if (name.Equals(Name)) return this;

            foreach (string s in childGroups.Keys.ToList())
            {
                if (name.Equals(s)) return childGroups[s];
            }
            if (parentGroup != null) return parentGroup;
            return this;
        }
        */

    public void AddGroup(Group group) {
      if (childGroups.ContainsKey(group.Name)) {
        return;
        //throw new SystemException("Error: One of the childGroups with the same key was already present in this Group.");
      }
      group.room = this.room;
      childGroups.Add(group.Name, group);
      group.ParentGroup = this;
      foreach (Node n in group.fullSet) {
        inherited.Add(n);
      }

      Group g = this;
      while (g != null) {
        group.groupPath = new List<Group>();
        group.groupPath.Add(g);
        g = g.ParentGroup;
      }
    }

    public void DetatchFromParent() {
      if (ParentGroup == null) return;
      foreach (Node n in fullSet) {
        ParentGroup.inherited.Remove(n);
      }
      if (ParentGroup.childGroups.ContainsKey(Name)) {
        ParentGroup.childGroups.Remove(Name);
      }
      ParentGroup = null;
    }

    public void GroupNamesToList(List<object> list, bool addSelf = true) {
      if (addSelf) list.Add(Name);
      foreach (Group g in childGroups.Values) {
        g.GroupNamesToList(list);
      }
    }

    public void DeleteGroup() {
      foreach (Group g in childGroups.Values)
        g.DeleteGroup();

      foreach (Node n in entities.ToList()) {
        n.group = null;
        DeleteEntity(n);
      }
      if (ParentGroup == null) throw new SystemException("Don't delete orphans");
      ParentGroup.childGroups.Remove(Name);
    }

    /*
        public void UpdateComboBox()
        {
            Game1.ui.sidebar.cbListPicker.ItemIndex = 0;
            List<object> list = Game1.ui.sidebar.cbListPicker.Items;
            list.ToList().ForEach((o) => list.Remove(o));

            GroupNamesToList(list);
            list.Add("Other Objects");
        }
        */
    //unfortunately I'm not sure it makes sense to use this awesome method
    public static void ForEachDictionary(Dictionary<string, Group> dict, Action<object> action) {
      //dict.Values.ToList().Select(x => x.entities).Aggregate((x, y) => (ObservableCollection<object>)x.Union(y)).ToList().ForEach(action);
      HashSet<object> hashset = new HashSet<object>();
      dict.Keys.ToList().ForEach(delegate(string key) {
                                   Group g = dict[key];

                                   g.entities.ToList().ForEach(delegate(Node o) {
                                                                 if (!hashset.Contains(o)) {
                                                                   hashset.Add(o);
                                                                   action(o);
                                                                 }
                                                               });
                                 });
    }
  }
}