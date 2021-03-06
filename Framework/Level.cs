﻿using System.Collections.Generic;
using SharpDX;

namespace OrbitVR.Framework {
  public class Level {
    public List<Rectangle> linesToDraw = new List<Rectangle>();
    public Room room;

    public int cellsX { get; set; }
    public int cellsY { get; set; }

    public int cellWidth { get; set; }
    public int cellHeight { get; set; }

    public int gridWidth {
      get { return cellsX*cellWidth; }
    }

    public int gridHeight {
      get { return cellsY*cellWidth; }
    }

    public Level() {}

    public Level(Room room, int cellsX, int cellsY, int cellWidth, int? cellHeight = null) {
      this.room = room;
      this.cellsX = cellsX;
      this.cellsY = cellsY;
      this.cellWidth = cellWidth;
      this.cellHeight = cellHeight ?? cellWidth;
    }

    public void Update() {}

    public void Draw() {}


    public void addLevelLines() {
      for (int i = 0; i <= cellsX; i++) {
        int x = i*cellWidth;
        //linesToDraw.Add(new Rectangle(x, 0, x, cellHeight*cellsY));
      }
      for (int i = 0; i <= cellsY; i++) {
        int y = i*cellHeight;
        //linesToDraw.Add(new Rectangle(0, y, cellWidth*cellsX, y));
      }
    }
  }
}