using System;
using System.Collections.Generic;
using OrbitVR.Components.Drawers;
using OrbitVR.Framework;
using OrbitVR.UI;
using SharpDX;

namespace OrbitVR.Components.Items {
  /// <summary>
  /// LineSpinner is an item that deploys lines that rotate randomly from the center of the node, upon pressing the trigger. (Bumpers to remove lines.)
  /// </summary>
  [Info(UserLevel.User,
    "LineSpinner is an item that deploys lines that rotate randomly from the center of the node, upon pressing the trigger. (Bumpers to remove lines.)",
    CompType)]
  public class LineSpinner : Component {
    public const mtypes CompType = mtypes.item | mtypes.playercontrol;
    Queue<SpinningLine> spinningLines = new Queue<SpinningLine>();

    public override mtypes compType {
      get { return CompType; }
      set { }
    }

    public float minDist { get; set; }
    public float distRange { get; set; }
    public float minSpeed { get; set; }
    public float speedRange { get; set; }
    public float randRotationSpeed { get; set; }
    public float totalRotationSpeedRange { get; set; }
    public int minNumberOfLines { get; set; }
    public int numberOfLinesRange { get; set; }
    public int maxCopyCount { get; set; }
    public int maxDistCopyCount { get; set; }
    public int maxPermanentDraw { get; set; }
    public LineSpinner() : this(null) {}

    public LineSpinner(Node parent) {
      this.parent = parent;
      minDist = 50f;
      distRange = 100;
      minSpeed = 0f;
      speedRange = 2f;
      randRotationSpeed = 10f;
      totalRotationSpeedRange = 20f;
      minNumberOfLines = 2;
      numberOfLinesRange = 10;
      maxCopyCount = 4;
      maxDistCopyCount = 4;
      maxPermanentDraw = 200;
    }

    public override void PlayerControl(Input input) {
      if (input.BtnClicked(InputButtons.RightTrigger_Mouse1)) {
        RandomizeSpinningLines();
      }
      else if (input.BtnClicked(InputButtons.RightBumper_E)) {
        if (spinningLines.Count != 0) {
          spinningLines.Dequeue();
        }
      }
      else if (input.BtnClicked(InputButtons.LeftBumper_Q)) {
        int count2 = spinningLines.Count;
        for (int i = 0; i < count2; i++) {
          spinningLines.Dequeue();
        }
      }

      if (spinningLines.Count > 0) {
        Color c = ColorChanger.getColorFromHSV(input.newInputState.RightStick_Mouse.AsDegrees);

        foreach (var line in spinningLines) {
          line.UpdateLines();
          line.DrawLines(this, c);
        }
      }
    }

    public void RandomizeSpinningLines() {
      Color color = Utils.randomColor();
      Vector2 center = parent.body.pos;
      float maxDist = (float) Utils.random.NextDouble()*distRange;
      //float angle = (float)Utils.random.NextDouble() * GMath.TwoPI;
      float speed = (float) Utils.random.NextDouble()*speedRange;
      float rotationSpeed = (float) Utils.random.NextDouble()*randRotationSpeed/500f;
      float totalRotationSpeed = (float) Utils.random.NextDouble()*totalRotationSpeedRange/500f;
      int numberOfLines = Utils.random.Next(numberOfLinesRange) + minNumberOfLines;
      int randmaxPermanentDraw = Utils.random.Next(maxPermanentDraw) + 1;
      //float angleIncrement = GMath.TwoPI / numberOfNodes;
      int copyCount = Utils.random.Next(maxCopyCount) + 1;
      float? copyOffset = Utils.random.Next(100)%2 == 0
                            ? (float) Utils.random.NextDouble()*GMath.PIbyTwo
                            : (null as float?); //as if

      int distCopyCount = Utils.random.Next(maxDistCopyCount) + 1;
      for (int i = 0; i < distCopyCount; i++) {
        SpinningLine spin = new SpinningLine(center, rotationSpeed, totalRotationSpeed, minDist, maxDist + minDist,
                                             speed, numberOfLines, copyCount, randmaxPermanentDraw, copyOffset,
                                             parent.body.color);
        spin.dist = maxDist/distCopyCount*i;
        spinningLines.Enqueue(spin);
      }
    }
  }

  public class SpinningLine {
    public Vector2 center;
    public Color? color = null;
    private float distLengthRatio, angleIncrement, copyOffset;
    public int lineCount, copyCount, permDraw;
    public float rotation, rotationSpeed, totalRotation, totalRotationSpeed, dist, minDist, maxDist, speed;

    public SpinningLine(Vector2 center, float rotationSpeed, float totalRotationSpeed, float minDist, float maxDist,
                        float speed, int lineCount, int copyCount, int permDraw, float? copyOffset, Color? color = null) {
      this.center = center;
      this.rotation = 0;
      this.totalRotation = 0;
      this.rotationSpeed = rotationSpeed;
      this.totalRotationSpeed = totalRotationSpeed;
      this.maxDist = maxDist;
      this.speed = speed;
      this.minDist = minDist;
      this.dist = minDist;
      this.lineCount = lineCount;
      this.permDraw = permDraw;
      this.color = color;
      this.angleIncrement = GMath.TwoPI/lineCount;
      this.distLengthRatio = (float) Math.Tan(angleIncrement/2)*2;
      this.copyCount = copyCount;
      this.copyOffset = copyOffset ?? GMath.TwoPI/copyCount;
    }

    public void UpdateLines() {
      rotation = (rotation + rotationSpeed)%GMath.TwoPI;
      totalRotation = (totalRotation + totalRotationSpeed)%GMath.TwoPI;
      dist += speed;
      if (dist > maxDist) {
        dist = maxDist;
        speed *= -1;
      }
      else if (dist < minDist) {
        dist = minDist;
        speed *= -1;
      }
    }

    public void DrawLines(LineSpinner lineSpinner, Color newColor) {
      for (int o = 0; o < copyCount; o++) {
        for (int i = 0; i < lineCount; i++) {
          float dirAngle = angleIncrement*i;
          dirAngle = (dirAngle + totalRotation)%GMath.TwoPI;
          Vector2 dir = VMath.AngleToVector(dirAngle);
          dir *= dist;
          float rotationAngle = dirAngle + GMath.PIbyTwo; //) % GMath.TwoPI;
          rotationAngle = (rotationAngle + rotation + copyOffset*o)%GMath.TwoPI;
          float length = dist*distLengthRatio;

          //symmetry.room.camera.Draw(textures.whitepixel, dir + symmetry.parent.body.pos, color ?? symmetry.parent.body.color, new Vector2(1f, length), rotationAngle, Layers.Under1);
          //lineSpinner.room.Camera.AddPermanentDraw(Textures.Whitepixel, dir + lineSpinner.parent.body.pos, newColor,
          //                                         new Vector2(1f, length), rotationAngle, permDraw);

          //Todo:drawLines
        }
      }
    }
  }
}