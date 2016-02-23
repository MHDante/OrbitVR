using System;
using System.Collections.Generic;
using System.Linq;
using OrbitVR.Components.Drawers;
using OrbitVR.Components.Essential;
using OrbitVR.Components.Items;
using OrbitVR.Components.Meta;
using OrbitVR.Framework;
using OrbitVR.Interface;
using OrbitVR.Physics;
using OrbitVR.Processes;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace OrbitVR {
  public enum RenderShape
{
    Plane,
    Sphere,
    Cylinder
}
  public class Room : Object3D {
    private Action _pendingRoomResize;
    public readonly Node TargetNodeGraphic;
    private readonly GeometricPrimitive _renderQuad;
    private readonly RenderShape _renderShape = DebugFlags.renderShape;
    public static long TotalElapsedMilliseconds { get; private set; }
    //Components
    public GridSystem GridsystemAffect { get; private set; }
    public Level Level { get; private set; }
    public RenderTarget2D RoomRenderTarget { get; }
    public CameraBase Camera { get; }
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
    private bool DrawAffectGrid = true;
    //Events
    public event EventHandler AfterIteration;
    
    public Room(int worldWidth, int worldHeight, bool groups = true)
    {
      Transform = new Transform();
      switch (_renderShape)
      {
        case RenderShape.Plane:
          _renderQuad = GeometricPrimitive.Plane.New(OrbIt.Game.GraphicsDevice, 2, 2, 32, true);
          break;
        case RenderShape.Sphere:
          _renderQuad = GeometricPrimitive.Sphere.New(OrbIt.Game.GraphicsDevice, 5, 32, true);
          break;
        case RenderShape.Cylinder:
          _renderQuad = GeometricPrimitive.Cylinder.New(OrbIt.Game.GraphicsDevice, 2, 2, 32, true);
          break;
      }
      Groups = new RoomGroups(this);
      AllActiveLinks = new ObservableHashSet<Link>();
      AllInactiveLinks = new ObservableHashSet<Link>();

      WorldWidth = worldWidth;
      WorldHeight = worldHeight;


      Scheduler = new Scheduler();
      BorderColor = Color.DarkGray;

      // grid System
      GridsystemAffect = new GridSystem(this, 40, new Vector2(0, 0), worldWidth, OrbIt.ScreenHeight);
      CollisionManager = new CollisionManager(this);
      Level = new Level(this, 40, 40, GridsystemAffect.cellWidth, GridsystemAffect.cellHeight);
      RoomRenderTarget = RenderTarget2D.New(OrbIt.Game.Graphics.GraphicsDevice, OrbIt.ScreenWidth, OrbIt.ScreenHeight,
                                            OrbIt.Game.pixelFormat.Format);
      Camera = new ThreadedCamera(this, 1f);
      DrawLinks = true;
      Scheduler = new Scheduler();
      
      Dictionary<dynamic, dynamic> userPr = new Dictionary<dynamic, dynamic>() {
        {nodeE.position, new Vector2(0, 0)},
        {nodeE.texture, textures.blackorb},
      };

      DefaultNode = new Node(this, userPr) {name = "master"};
      //defaultNode.IsDefault = true;

      foreach (Component c in DefaultNode.comps.Values)
      {
        c.AfterCloning();
      }

      Node firstdefault = new Node(this, ShapeType.Circle);
      //firstdefault.addComponent(comp.itempayload, true);
      Node.cloneNode(DefaultNode, firstdefault);
      firstdefault.name = "[G0]0";
      //firstdefault.IsDefault = true;

      MasterGroup = new Group(this, DefaultNode, DefaultNode.name, false);
      if (groups)
      {
        MasterGroup.AddGroup(new Group(this, DefaultNode,  "General Groups", false));
        MasterGroup.AddGroup(new Group(this, DefaultNode,  "Preset Groups", false));
        MasterGroup.AddGroup(new Group(this, DefaultNode.CreateClone(this),  "Player Group"));
        MasterGroup.AddGroup(new Group(this, DefaultNode,  "Item Group", false));
        MasterGroup.AddGroup(new Group(this, DefaultNode,  "Link Groups", false));
        MasterGroup.AddGroup(new Group(this, DefaultNode.CreateClone(this),  "Bullet Group"));
        MasterGroup.AddGroup(new Group(this, DefaultNode,  "Wall Group"));
        Groups.General.AddGroup(new Group(this, firstdefault, "Group1"));
      }
      Dictionary<dynamic, dynamic> userPropsTarget = new Dictionary<dynamic, dynamic>() {
        {typeof (ColorChanger), true},
        {nodeE.texture, textures.ring}
      };

      TargetNodeGraphic = new Node(this, userPropsTarget);

      TargetNodeGraphic.name = "TargetNodeGraphic";

      //MakeWalls(WallWidth);

      MakePresetGroups();
      MakeItemGroups();
    }
    public void MakePresetGroups() {
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
        nodeDef.Comp<Rune>().runeTexture = (textures) runenum++;
        Groups.Preset.AddGroup(new Group(this, nodeDef, t.ToString().LastWord('.') + " Group"));
      }
    }

    public void MakeItemGroups() {
      Node itemDef = DefaultNode.CreateClone(this);
      itemDef.addComponent(typeof (ItemPayload), true);
      itemDef.movement.active = false;

      var infos = Component.compInfos;
      foreach (Type t in infos.Keys) {
        Info info = infos[t];
        if ((info.compType & mtypes.item) != mtypes.item) continue;
        if (t == typeof (ItemPayload)) continue;
        //if (info.userLevel == UserLevel.Developer || info.userLevel == UserLevel.Advanced) continue;
        Node nodeDef = itemDef.CreateClone(this); ///
        //nodeDef.addComponent(t, true);
        Component c = Node.MakeComponent(t, true, nodeDef);
        nodeDef.Comp<ItemPayload>().AddComponentItem(c);
        Groups.Items.AddGroup(new Group(this, nodeDef, t.ToString().LastWord('.') + " Item"));
      }
    }

    public void Update(GameTime gametime) {
      Player.CheckForPlayers(this);

      Camera.Update();
      long elapsed = 0;
      if (gametime != null) elapsed = (long) Math.Round(gametime.ElapsedGameTime.TotalMilliseconds);
      TotalElapsedMilliseconds += elapsed;

      HashSet<Node> toDelete = new HashSet<Node>();

      //AffectAlgorithm #2 See Souce History in this file for AffectAlgorithm 1
      GridsystemAffect.clearBuckets();
      foreach (var n in MasterGroup.fullSet) {
        if (MasterGroup.childGroups["Wall Group"].fullSet.Contains(n)) continue;
        GridsystemAffect.insertToBuckets(n.body);
      }

      CollisionManager.Update();

      foreach (Node n in MasterGroup.fullSet.ToList()) {
        if (n.active) {
          n.Update(gametime);
        }
      }
      if (AfterIteration != null) AfterIteration(this, null);

      GridsystemAffect.addGridSystemLines();
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
      foreach (var n in MasterGroup.fullSet.ToList()) //todo:wtfuck threading?
      {
        //Node n = (Node)o;
        n.Draw();
      }
      
      Camera.DrawLine(Vector2.Zero, new Vector2(WorldWidth, 0), 2, BorderColor, Layers.Under5);
      Camera.DrawLine(Vector2.Zero, new Vector2(0, WorldHeight), 2, BorderColor, Layers.Under5);
      Camera.DrawLine(new Vector2(0, WorldHeight), new Vector2(WorldWidth, WorldHeight), 2, BorderColor, Layers.Under5);
      Camera.DrawLine(new Vector2(WorldWidth, 0), new Vector2(WorldWidth, WorldHeight), 2, BorderColor, Layers.Under5);

      if (DrawAffectGrid) GridsystemAffect.DrawGrid(this, Color.LightBlue);

      if (DrawLinks) {
        foreach (Link link in AllActiveLinks) {
          link.GenericDraw();
        }
      }
      OrbIt.GlobalGameMode.Draw();
      GraphData.DrawGraph();
    }

    public void Draw3D() {
      var quadEffect = new BasicEffect(OrbIt.Game.GraphicsDevice) {
        World = Transform.getMatrix(),
        View = OrbIt.Game.view,
        Projection = OrbIt.Game.projection,
        TextureEnabled = true,
        Texture = RoomRenderTarget
      };
      foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes) {
        pass.Apply();

        _renderQuad.Draw();
      }
      if (Camera.TakeScreenshot) {
        Camera.Screenshot();
        Camera.TakeScreenshot = false;
      }
    }

    public void MakeWalls(float wallWidth) {
      Dictionary<dynamic, dynamic> props = new Dictionary<dynamic, dynamic>() {
        {nodeE.position, new Vector2(0, 0)},
      };
      Node left = ConstructWallPoly(props, (int) wallWidth/2, WorldHeight/2, new Vector2(wallWidth/2, WorldHeight/2));
      left.name = "left wall";
      Node right = ConstructWallPoly(props, (int) wallWidth/2, WorldHeight/2,
                                     new Vector2(WorldWidth - wallWidth/2, WorldHeight/2));
      right.name = "right wall";
      Node top = ConstructWallPoly(props, (WorldWidth + (int) wallWidth*2)/2, (int) wallWidth/2,
                                   new Vector2(WorldWidth/2, (int) wallWidth/2));
      top.name = "top wall";
      Node bottom = ConstructWallPoly(props, (WorldWidth + (int) wallWidth*2)/2, (int) wallWidth/2,
                                      new Vector2(WorldWidth/2, WorldHeight - wallWidth/2));
      bottom.name = "bottom wall";
    }

    public Node ConstructWallPoly(Dictionary<dynamic, dynamic> props, int hw, int hh, Vector2 pos) {
      Node n = new Node(this, props);
      n.Comp<BasicDraw>().active = false;
      Polygon poly = new Polygon();
      poly.body = n.body;
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


    public void UpdateTargetNodeGraphic() {
      if (TargetNode != null) {
        TargetNodeGraphic.Comp<ColorChanger>().AffectSelf();
        TargetNodeGraphic.body.pos = TargetNode.body.pos;
        TargetNodeGraphic.body.radius = TargetNode.body.radius*1.5f;
      }
    }

    public void AddRectangleLines(float x, float y, float width, float height) {
      AddRectangleLines((int) x, (int) y, (int) width, (int) height);
    }

    public Node SelectNodeAt(Vector2 pos) {
      Node found = null;
      float shortedDistance = Int32.MaxValue;
      for (int i = MasterGroup.fullSet.Count - 1; i >= 0; i--) {
        Node n = (Node) MasterGroup.fullSet.ElementAt(i);
        // find node that has been clicked, starting from the most recently placed nodes
        float distsquared = Vector2.DistanceSquared(n.body.pos, pos);
        if (distsquared < n.body.radius*n.body.radius) {
          if (distsquared < shortedDistance) {
            found = n;
            shortedDistance = distsquared;
          }
        }
      }
      return found;
    }

    public Node spawnNode(int worldMouseX, int worldMouseY) {
      Dictionary<dynamic, dynamic> userP = new Dictionary<dynamic, dynamic>() {
        {nodeE.position, new Vector2(worldMouseX, worldMouseY)},
      };
      return spawnNode(userP);
    }

    public Node spawnNode(Node newNode, Action<Node> afterSpawnAction = null, int lifetime = -1, Group g = null) {
      Group spawngroup = g ?? OrbIt.UI.sidebar.GetActiveGroup();
      if (spawngroup == null || !spawngroup.Spawnable) return null;
      //newNode.name = "bullet" + Node.nodeCounter;
      return SpawnNodeHelper(newNode, afterSpawnAction, spawngroup, lifetime);
    }

    public Node spawnNode(Dictionary<dynamic, dynamic> userProperties, Action<Node> afterSpawnAction = null,
                          bool blank = false, int lifetime = -1) {
      Group activegroup = OrbIt.UI.sidebar.GetActiveGroup();
      if (activegroup == null || !activegroup.Spawnable) return null;

      Node newNode = new Node(this, ShapeType.Circle);
      if (!blank) {
        Node.cloneNode(OrbIt.UI.sidebar.ActiveDefaultNode, newNode);
      }
      newNode.group = activegroup;
      newNode.name = activegroup.Name + Node.nodeCounter;
      newNode.acceptUserProps(userProperties);

      return SpawnNodeHelper(newNode, afterSpawnAction, activegroup, lifetime);
    }

    public Node spawnNode(Group group, Dictionary<dynamic, dynamic> userProperties = null) {
      if (group == null) return null;
      Node newNode = group.defaultNode.CreateClone(this);
      newNode.group = group;
      newNode.name = group.Name + Node.nodeCounter;
      if (userProperties != null) newNode.acceptUserProps(userProperties);
      return SpawnNodeHelper(newNode, null, group, -1);
    }

    private Node SpawnNodeHelper(Node newNode, Action<Node> afterSpawnAction = null, Group g = null, int lifetime = -1) {
      //newNode.addComponent(comp.itempayload, true);
      newNode.OnSpawn();
      if (afterSpawnAction != null) afterSpawnAction(newNode);
      if (lifetime != -1) {
        newNode.addComponent<Lifetime>(true);
        newNode.Comp<Lifetime>().timeUntilDeath.value = lifetime;
        newNode.Comp<Lifetime>().timeUntilDeath.enabled = true;
      }

      g.IncludeEntity(newNode);
      newNode.spawned = true;
      return newNode;
    }

    internal void Resize(Vector2 resizeVect, bool fillWithGrid = false) {
      _pendingRoomResize = delegate {
                            WorldWidth = (int) resizeVect.X;
                            WorldHeight = (int) resizeVect.Y;
                            int newCellsX = WorldWidth/GridsystemAffect.cellWidth;
                            int gridHeight = fillWithGrid ? WorldHeight : OrbIt.ScreenHeight;
                            GridsystemAffect = new GridSystem(this, newCellsX, new Vector2(0, WorldHeight - gridHeight),
                                                              WorldWidth,
                                                              gridHeight);
                            Level = new Level(this, newCellsX, newCellsX, GridsystemAffect.cellWidth,
                                              GridsystemAffect.cellHeight);
                            //roomRenderTarget = new RenderTarget2D(game.GraphicsDevice, worldWidth, worldHeight);
                            CollisionManager.gridsystemCollision = new GridSystem(this, newCellsX,
                                                                                  new Vector2(0,
                                                                                              WorldHeight - gridHeight),
                                                                                  WorldWidth, gridHeight);
                            fillWithGrid = false;

                            Camera.pos = new Vector2(WorldWidth/2, WorldHeight/2);
                          };
    }


    public class RoomGroups {
      private Room _room;

      public RoomGroups(Room room) {
        _room = room;
      }

      public Group General {
        get {
          if (_room.MasterGroup == null) return null;
          return _room.MasterGroup.childGroups["General Groups"];
        }
      }

      public Group Preset {
        get {
          if (_room.MasterGroup == null) return null;
          return _room.MasterGroup.childGroups["Preset Groups"];
        }
      }

      public Group Player {
        get {
          if (_room.MasterGroup == null) return null;
          return _room.MasterGroup.childGroups["Player Group"];
        }
      }

      public Group Items {
        get {
          if (_room.MasterGroup == null) return null;
          return _room.MasterGroup.childGroups["Item Group"];
        }
      }

      public Group Bullets {
        get {
          if (_room.MasterGroup == null) return null;
          return _room.MasterGroup.childGroups["Bullet Group"];
        }
      }

      public Group Walls {
        get {
          if (_room.MasterGroup == null) return null;
          return _room.MasterGroup.childGroups["Wall Group"];
        }
      }
    }
  }
}