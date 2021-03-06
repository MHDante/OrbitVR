﻿using System;
using System.Collections.Generic;
using System.Linq;
using OrbitVR.Components.Drawers;
using OrbitVR.Components.Essential;
using OrbitVR.Components.Items;
using OrbitVR.Components.Meta;
using OrbitVR.Framework;
using OrbitVR.Physics;
using OrbitVR.Processes;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR {
  public enum RenderShape {
    Plane,
    Sphere,
    Cylinder
  }

  public class Room : Object3D {
    private readonly RenderShape _renderShape = DebugFlags.renderShape;
    private Action _pendingRoomResize;
    public Node ActiveDefaultNode => GetActiveGroup()?.defaultNode;

    public Group GetActiveGroup() => string.IsNullOrEmpty(ActiveGroupName) ? MasterGroup : MasterGroup.FindGroup(ActiveGroupName);

    public string ActiveGroupName;
    public Node TargetNodeGraphic { get; }
    public static long TotalElapsedMilliseconds { get; private set; }
    //Components
    public GridSystem GridsystemAffect { get; private set; }
    public Level Level { get; private set; }
    public MeshCamera Camera { get; }
    public Scheduler Scheduler { get; }
    public CollisionManager CollisionManager { get; }
    //Entities
    public Group MasterGroup { get; }
    public RoomGroups Groups { get; }
    public Node DefaultNode { get; }
    public ObservableHashSet<Link> AllActiveLinks { get; }
    public ObservableHashSet<Link> AllInactiveLinks { get; }
    //Values
    public int WorldWidth { get; private set; }
    public int WorldHeight { get; private set; }
    private bool DrawLinks { get; }
    public Node TargetNode { get; set; }
    private Color BorderColor { get; }

    public Room(int worldWidth, int worldHeight, bool groups = true){
      
      
      Transform = new Transform();
      Transform.rotation = Quaternion.RotationAxis(Vector3.Up, (float)Math.PI);
      Groups = new RoomGroups(this);
      AllActiveLinks = new ObservableHashSet<Link>();
      AllInactiveLinks = new ObservableHashSet<Link>();

      WorldWidth = worldWidth;
      WorldHeight = worldHeight;

      Scheduler = new Scheduler();
      BorderColor = Color.DarkGray;

      // grid System
      GridsystemAffect = new GridSystem(this, 40, new Vector2R(0, 0), worldWidth, OrbIt.ScreenHeight);
      CollisionManager = new CollisionManager(this);
      Level = new Level(this, 40, 40, GridsystemAffect.cellWidth, GridsystemAffect.cellHeight);
      Camera = new MeshCamera(this,1f, Vector2R.Zero);
      DrawLinks = true;
      Scheduler = new Scheduler();

      Dictionary<dynamic, dynamic> userPr = new Dictionary<dynamic, dynamic>() {
        {nodeE.position, new Vector2R(0, 0)},
        {nodeE.texture, Textures.Blackorb},
      };

      DefaultNode = new Node(this, userPr) {name = "master"};
      //defaultNode.IsDefault = true;

      foreach (Component c in DefaultNode.comps.Values) {
        c.AfterCloning();
      }

      Node firstdefault = new Node(this, ShapeType.Circle);
      //firstdefault.addComponent(comp.itempayload, true);
      Node.cloneNode(DefaultNode, firstdefault);
      firstdefault.name = "[G0]0";
      //firstdefault.IsDefault = true;

      MasterGroup = new Group(this, DefaultNode, DefaultNode.name, false);
      if (groups) {
        MasterGroup.AddGroup(new Group(this, DefaultNode, "General Groups", false));
        MasterGroup.AddGroup(new Group(this, DefaultNode, "Preset Groups", false));
        MasterGroup.AddGroup(new Group(this, DefaultNode.CreateClone(this), "Player Group"));
        MasterGroup.AddGroup(new Group(this, DefaultNode, "Item Group", false));
        MasterGroup.AddGroup(new Group(this, DefaultNode, "Link Groups", false));
        MasterGroup.AddGroup(new Group(this, DefaultNode.CreateClone(this), "Bullet Group"));
        MasterGroup.AddGroup(new Group(this, DefaultNode, "Wall Group"));
        Groups.General.AddGroup(new Group(this, firstdefault, "Group1"));
      }
      Dictionary<dynamic, dynamic> userPropsTarget = new Dictionary<dynamic, dynamic>() {
        {typeof (ColorChanger), true},
        {nodeE.texture, Textures.Ring}
      };

      TargetNodeGraphic = new Node(this, userPropsTarget) {name = "TargetNodeGraphic"};
      if (DebugFlags.addRoomWalls) MakeWalls(10);

      MakePresetGroups();
      MakeItemGroups();
    }

    //Events
    public event EventHandler AfterIteration;

    private void MakePresetGroups() {
      var infos = Component.compInfos;
      int runenum = 0;
      foreach (Type t in infos.Keys) {
        Info info = infos[t];
        if ((info.compType & mtypes.essential) == mtypes.essential) continue;
        if ((info.compType & mtypes.exclusiveLinker) == mtypes.exclusiveLinker) continue;
        if ((info.compType & mtypes.item) == mtypes.item) continue;
        if (info.userLevel == UserLevel.Developer || info.userLevel == UserLevel.Advanced) continue;
        if (t == typeof (Lifetime)) continue;
        if (t == typeof (Rune)) continue;
        Node nodeDef = DefaultNode.CreateClone(this);
        nodeDef.SetColor(Utils.randomColor());
        nodeDef.addComponent(t, true);
        nodeDef.addComponent(typeof (Rune), true);
        nodeDef.Comp<Rune>().runeTexture = (Textures) runenum++;
        Groups.Preset.AddGroup(new Group(this, nodeDef, t.ToString().LastWord('.') + " Group"));
      }
    }

    private void MakeItemGroups() {
      Node itemDef = DefaultNode.CreateClone(this);
      itemDef.addComponent(typeof (ItemPayload), true);
      itemDef.movement.active = false;

      var infos = Component.compInfos;
      foreach (Type t in infos.Keys) {
        Info info = infos[t];
        if ((info.compType & mtypes.item) != mtypes.item) continue;
        if (t == typeof (ItemPayload)) continue;
        //if (info.userLevel == UserLevel.Developer || info.userLevel == UserLevel.Advanced) continue;
        Node nodeDef = itemDef.CreateClone(this);
        //nodeDef.addComponent(t, true);
        Component c = Node.MakeComponent(t, true, nodeDef);
        nodeDef.Comp<ItemPayload>().AddComponentItem(c);
        Groups.Items.AddGroup(new Group(this, nodeDef, t.ToString().LastWord('.') + " Item"));
      }
    }

    public void Update() {
      Player.CheckForPlayers(this);

      Camera.Update();
      long elapsed = 0;
      if (OrbIt.Game.Time != null) elapsed = (long) Math.Round(OrbIt.Game.Time.ElapsedGameTime.TotalMilliseconds);
      TotalElapsedMilliseconds += elapsed;


      //AffectAlgorithm #2 See Souce History in this file for AffectAlgorithm 1
      GridsystemAffect.clearBuckets();
      foreach (var n in MasterGroup.fullSet) {
        if (MasterGroup.childGroups["Wall Group"].fullSet.Contains(n)) continue;
        GridsystemAffect.insertToBuckets(n.body);
      }
      if (DebugFlags.drawAffectGrid) GridsystemAffect.addGridSystemLines();

      CollisionManager.Update();

      foreach (Node n in MasterGroup.fullSet.ToList()) {
        if (n.active) {
          n.Update();
        }
      }
      AfterIteration?.Invoke(this, null);

      Level.addLevelLines();

      UpdateTargetNodeGraphic();

      Scheduler.AffectSelf();

      if (_pendingRoomResize != null) {
        _pendingRoomResize();
        _pendingRoomResize = null;
      }
    }


    public void Draw() {
      //spritebatch.Draw(game.textureDict[textures.whitepixel], new Vector2(300, 300), null, Color.Black, 0f, Vector2.Zero, 100f, SpriteEffects.None, 0);
      if (TargetNode != null) {
        UpdateTargetNodeGraphic();
        TargetNodeGraphic.Draw();
      }
      foreach (var n in MasterGroup.fullSet.ToList()) {
        //Node n = (Node)o;
        n.Draw();
      }
      if (DebugFlags.drawRoomBorder) {
        Camera.DrawRect(Vector2R.Zero, new Vector2R(WorldWidth, WorldHeight), BorderColor);
      }
      if (DebugFlags.drawAffectGrid) GridsystemAffect.DrawGrid(this, Color.LightBlue);
      if (DebugFlags.drawCollisionGrid) CollisionManager.Draw();

      if (DrawLinks) {
        foreach (Link link in AllActiveLinks) {
          link.GenericDraw();
        }
      }
      OrbIt.GlobalGameMode.Draw();
      GraphData.DrawGraph();
    }

    public void Draw3D(Matrix world) {
        Camera.Draw(world);
        
    }

    private void MakeWalls(float wallWidth) {
      Dictionary<dynamic, dynamic> props = new Dictionary<dynamic, dynamic>() {
        {nodeE.position, new Vector2R(0, 0)},
      };
      var v = wallWidth/2f;
      var v1 = WorldHeight/2f;
      Node left = ConstructWallPoly(props, v, v1, new Vector2R(v, v1));
      Node right = ConstructWallPoly(props, v, v1, new Vector2R(WorldWidth - v, v1));
      Node top = ConstructWallPoly(props, (WorldWidth + wallWidth*2)/2f, v, new Vector2R(WorldWidth/2f, v));
      Node bottom = ConstructWallPoly(props, (WorldWidth + wallWidth*2)/2f, v,
                                      new Vector2R(WorldWidth/2f, WorldHeight - v));
      left.name = "left wall";
      right.name = "right wall";
      top.name = "top wall";
      bottom.name = "bottom wall";
    }

    private Node ConstructWallPoly(Dictionary<dynamic, dynamic> props, float hw, float hh, Vector2R pos) {
      Node n = new Node(this, props);
      n.Comp<BasicDraw>().active = false;
      Polygon poly = new Polygon {body = n.body};
      poly.body.pos = pos;
      poly.SetBox(hw, hh);
      //poly.SetOrient(0f);

      n.body.shape = poly;
      n.body.SetStatic();
      n.body.orient = (0);
      //n.body.restitution = 1f;

      //n.movement.pushable = false;

      MasterGroup.childGroups["Wall Group"].entities.Add(n);
      return n;
    }


    private void UpdateTargetNodeGraphic() {
      if (TargetNode != null) {
        TargetNodeGraphic.Comp<ColorChanger>().AffectSelf();
        TargetNodeGraphic.body.pos = TargetNode.body.pos;
        TargetNodeGraphic.body.radius = TargetNode.body.radius*1.5f;
      }
    }


    public Node SelectNodeAt(Vector2R pos) {
      Node found = null;
      float shortedDistance = Int32.MaxValue;
      for (int i = MasterGroup.fullSet.Count - 1; i >= 0; i--) {
        Node n = MasterGroup.fullSet.ElementAt(i);
        // find node that has been clicked, starting from the most recently placed nodes
        float distsquared = Vector2R.DistanceSquared(n.body.pos, pos);
        if (distsquared < n.body.radius*n.body.radius) {
          if (distsquared < shortedDistance) {
            found = n;
            shortedDistance = distsquared;
          }
        }
      }
      return found;
    }

    public Node SpawnNode(int worldMouseX, int worldMouseY) {
      Dictionary<dynamic, dynamic> userP = new Dictionary<dynamic, dynamic>() {
        {nodeE.position, new Vector2R(worldMouseX, worldMouseY)},
      };
      return SpawnNode(userP);
    }

    public Node SpawnNode(Node newNode, Action<Node> afterSpawnAction = null, int lifetime = -1, Group g = null) {
      Group spawngroup = g ?? GetActiveGroup();
      if (spawngroup == null || !spawngroup.Spawnable) return null;
      //newNode.name = "bullet" + Node.nodeCounter;
      return SpawnNodeHelper(newNode, afterSpawnAction, spawngroup, lifetime);
    }

    public Node SpawnNode(Dictionary<dynamic, dynamic> userProperties, Action<Node> afterSpawnAction = null,
                          bool blank = false, int lifetime = -1) {
      Group activegroup = GetActiveGroup();
      if (activegroup == null || !activegroup.Spawnable) return null;

      Node newNode = new Node(this, ShapeType.Circle);
      if (!blank) {
        Node.cloneNode(ActiveDefaultNode, newNode);
      }
      newNode.group = activegroup;
      newNode.name = activegroup.Name + Node.nodeCounter;
      newNode.acceptUserProps(userProperties);

      return SpawnNodeHelper(newNode, afterSpawnAction, activegroup, lifetime);
    }

    public Node SpawnNode(Group group, Dictionary<dynamic, dynamic> userProperties = null) {
      if (group == null) return null;
      Node newNode = group.defaultNode.CreateClone(this);
      newNode.group = group;
      newNode.name = group.Name + Node.nodeCounter;
      if (userProperties != null) newNode.acceptUserProps(userProperties);
      return SpawnNodeHelper(newNode, null, group);
    }

    private Node SpawnNodeHelper(Node newNode, Action<Node> afterSpawnAction = null, Group g = null, int lifetime = -1) {
      //newNode.addComponent(comp.itempayload, true);
      newNode.OnSpawn();
      afterSpawnAction?.Invoke(newNode);
      if (lifetime != -1) {
        newNode.addComponent<Lifetime>(true);
        newNode.Comp<Lifetime>().timeUntilDeath.value = lifetime;
        newNode.Comp<Lifetime>().timeUntilDeath.enabled = true;
      }

      g?.IncludeEntity(newNode);
      newNode.spawned = true;
      return newNode;
    }

    internal void Resize(Vector2R resizeVect, bool fillWithGrid = false) {
      _pendingRoomResize = () => {
                             WorldWidth = (int) resizeVect.X;
                             WorldHeight = (int) resizeVect.Y;
                             int newCellsX = WorldWidth/GridsystemAffect.cellWidth;
                             int gridHeight = fillWithGrid ? WorldHeight : OrbIt.ScreenHeight;
                             GridsystemAffect = new GridSystem(this, newCellsX, new Vector2R(0, WorldHeight - gridHeight),
                                                               WorldWidth,
                                                               gridHeight);
                             Level = new Level(this, newCellsX, newCellsX, GridsystemAffect.cellWidth,
                                               GridsystemAffect.cellHeight);
                             //roomRenderTarget = new RenderTarget2D(game.GraphicsDevice, worldWidth, worldHeight);
                             CollisionManager.gridsystemCollision = new GridSystem(this, newCellsX,
                                                                                   new Vector2R(0,
                                                                                               WorldHeight - gridHeight),
                                                                                   WorldWidth, gridHeight);
                             fillWithGrid = false;

                             Camera.pos = new Vector2R(WorldWidth/2f, WorldHeight/2f);
                           };
    }

    public void EmptyCurrentGroup()
    {
      Group g = GetActiveGroup();
      if (g == null) return;
      if (TargetNode != null) if (g.fullSet.Contains(TargetNode)) TargetNode = null;
      g.EmptyGroup();
    }

    
  }
}