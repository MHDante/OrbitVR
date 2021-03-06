﻿using System;
using System.Collections.Generic;
using System.Linq;
using OrbitVR.Components.Essential;
using OrbitVR.Components.Items;
using OrbitVR.Physics;
using SharpDX;

namespace OrbitVR.Framework {
  public class PlayerData {
    public PlayerData() {}
  }


  public class Player {
    public static Player[] players = new Player[5]; //0th for keyboard, 1st-4th for controllers (for now)

    public static bool EnablePlayers = true;
    public ItemSlots _currentItem = ItemSlots.Y_Yellow;
    public Node _node;
    public string ColorName;

    //public Controller controller;
    public Input input;

    public Dictionary<ItemSlots, Component> itemSlots = new Dictionary<ItemSlots, Component>() {
      {ItemSlots.Y_Yellow, null},
      {ItemSlots.A_Green, null},
      {ItemSlots.B_Red, null},
      {ItemSlots.X_Blue, null},
    };

    public ItemSlots occupiedSlots = ItemSlots.None;

    public Color pColor;

    public Dictionary<Type, PlayerData> playerDatas = new Dictionary<Type, PlayerData>();

    public int playerIndex;
    public Room room;

    public Node node {
      get { return _node; }
      set {
        if (_node != null) _node.player = null;
        _node = value;
        if (value != null) value.player = this;
      }
    }

    public Body body {
      get { return node != null ? node.body : null; }
    }

    public ItemSlots currentItem {
      get { return _currentItem; }
      set {
        foreach (var item in itemSlots.Keys) {
          if (itemSlots[item] != null) {
            if (item != value) itemSlots[item].active = false;
            else itemSlots[item].active = true;
          }
        }
        _currentItem = value;
      }
    }

    /*
        public static Player GetNew(int playerIndex)
        {
            bool success = false;
            Player p = new Player(playerIndex, ref success);
            return success ? p : null;
        }
        private Player(int playerIndex, ref bool sucess)
        {
            controller = FullController.GetNew(playerIndex);
            if (controller == null)
            {
                sucess = false;
                return;
            }
            sucess = true;
            room = OrbIt.game.room;
            this.playerIndex = playerIndex;
            SetPlayerColor();
        }
        */

    public Player(int playerIndex) {
      this.room = OrbIt.Game.Room;
      this.playerIndex = playerIndex;
      if (playerIndex == 0) {
        this.input = new PcFullInput(this);
      }
      else {
        this.input = new ControllerFullInput(this, (PlayerIndex) (playerIndex - 1));
      }

      SetPlayerColor();
    }

    public void AddItem(Component comp) {
      int count = 0;
      foreach (var slot in itemSlots.Keys.ToList()) {
        if (itemSlots[slot] != null) {
          count++;
          if (itemSlots[slot].GetType() == comp.GetType()) {
            itemSlots[slot] = comp;
            if (slot != currentItem) comp.active = false;
            if (count == 0) currentItem = slot;
            return;
          }
        }
      }
      if (count == 4) return;
      foreach (var slot in itemSlots.Keys.ToList()) {
        if (itemSlots[slot] == null) {
          itemSlots[slot] = comp;
          occupiedSlots |= slot;
          if (slot != currentItem) comp.active = false;
          return;
        }
      }
    }

    public void RemoveItem(Component comp) {
      foreach (var slot in itemSlots.Keys.ToList()) {
        if (comp == itemSlots[slot]) {
          occupiedSlots = occupiedSlots ^ slot;
          itemSlots[slot] = null;
          return;
        }
      }
    }

    public T Data<T>() where T : PlayerData {
      Type t = typeof (T);
      if (playerDatas.ContainsKey(t)) return (T) playerDatas[t];
      return null;
    }

    public bool HasData<T>() where T : PlayerData {
      return playerDatas.ContainsKey(typeof (T));
    }

    public void SetPlayerColor() {
      switch (playerIndex) {
        case 1:
          pColor = Color.Blue;
          ColorName = "Blue";
          break;
        case 2:
          pColor = Color.Green;
          ColorName = "Green";
          break;
        case 3:
          pColor = Color.Red;
          ColorName = "Red";
          break;
        case 4:
          pColor = Color.Yellow;
          ColorName = "Yellow";
          break;
      }
      byte min = 40;
      if (pColor.R == 0) pColor.R = min;
      if (pColor.G == 0) pColor.G = min;
      if (pColor.B == 0) pColor.B = min;
    }

    //
    public static void ResetPlayers(Room room) //todo:fix
    {
      room.Groups.Player.EmptyGroup();
      Controller.ResetControllers();
      CreatePlayers(room);
      //OrbIt.ui.sidebar.playerView.InitializePlayers();
    }

    public static void CreatePlayers(Room room) {
      room.Groups.Player.defaultNode = room.MasterGroup.defaultNode.CreateClone(room);
      Shooter.MakeBullet(room);
      if (!EnablePlayers) return;
      //def.addComponent(comp.shooter, true);

      for (int i = 1; i < 5; i++) {
        TryCreatePlayer(room, room.Groups.Player.defaultNode, i, false);
      }
      //OrbIt.ui.sidebar.playerView.InitializePlayers();
    }

    public static void CheckForPlayers(Room room) {
      for (int i = 1; i < 5; i++) {
        if (players[i] == null) // && GamePad.GetState((PlayerIndex)(i-1)).IsConnected)
        {
          TryCreatePlayer(room, room.Groups.Player.defaultNode, i, true);
        }
      }
    }

    public static void TryCreatePcPlayer() {
      if (players[0] == null) {
        TryCreatePlayer(OrbIt.Game.Room, OrbIt.Game.Room.Groups.Player.defaultNode, 0, true);
      }
    }

    private static void TryCreatePlayer(Room room, Node defaultNode, int playerIndex, bool updateUI) {
      if (playerIndex != 0) {
        GamePadState gamePadState = GamePad.GetState((PlayerIndex) (playerIndex - 1));
        if (!gamePadState.IsConnected || gamePadState.Buttons.Back == ButtonState.Released) return;
      }
      //Player p = Player.GetNew(playerIndex);
      Player p = new Player(playerIndex);
      players[playerIndex] = p;
      if (p == null) return;
      double angle = Utils.random.NextDouble()*Math.PI*2;
      angle -= Math.PI;
      float dist = 200;
      float x = dist*(float) Math.Cos(angle);
      float y = dist*(float) Math.Sin(angle);
      Vector2R spawnPos = new Vector2R((room.WorldWidth/4)*playerIndex - (room.WorldWidth/8), room.WorldHeight - 600);
      // -new Vector2(x, y);
      Node node = defaultNode.CreateClone(room);
      p.node = node;

      node.body.pos = spawnPos;
      node.name = "player" + p.ColorName;
      node.SetColor(p.pColor);
      //node.addComponent(comp.shooter, true);
      //node.addComponent(comp.sword, true);
      //node.Comp<Sword>().sword.collision.DrawRing = false;

      //room.groups.player.IncludeEntity(node);
      node.meta.healthBar = Meta.HealthBarMode.Bar;
      //node.OnSpawn();
      node.body.velocity = Vector2R.Zero;
      //node.body.mass = 0.1f;
      node.movement.maxVelocity.value = 6f;
      //node.addComponent<LinkGun>(true);
      room.SpawnNode(node, g: room.Groups.Player);
      //node.OnSpawn();
      node.texture = Textures.Robot1;

      //if (updateUI)
      //{
      //    OrbIt.ui.sidebar.playerView.InitializePlayers();
      //}
    }
  }
}