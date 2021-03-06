﻿using System;
using System.Linq;
using OrbitVR.Framework;
using SharpDX;
using SharpDX.Direct3D11;
using Point = System.Drawing.Point;

namespace OrbitVR.Physics {
  public enum ShapeType {
    Circle,
    Polygon,
  }

  public abstract class Shape {
    public static int TypeCount = 2;

    public Body body; //{ get; set; }
    public Mat22 u; // { get; set; }
    public float radius { get; set; }

    //public Body bodyP { get { return body; } set { body = value; } }
    public Mat22 uP {
      get { return u; }
      set { u = value; }
    }

    public Shape() {}

    public abstract Shape Clone();
    public abstract void Initialize();
    public abstract void ComputeMass(float density);
    public abstract void SetOrient(float radians);
    public abstract void Draw();
    public abstract ShapeType GetShapeType();
  }

  public class Circle : Shape {
    public Circle(float r) {
      //Debug.Assert(r != 0);
      radius = r;
    }

    public override Shape Clone() {
      return new Circle(radius);
    }

    public override void Initialize() {
      ComputeMass(0.001f);
    }

    public override void ComputeMass(float density) {
      body.mass = (float) Math.PI*radius*radius*density;
      body.inertia = body.mass*radius*radius;
    }

    public override ShapeType GetShapeType() {
      return ShapeType.Circle;
    }

    public override void SetOrient(float radians) {}

    public override void Draw() {}
  }

  public class Polygon : Shape {
    public static int MaxPolyVertexCount = 64;
    public Vector2R[] normals = new Vector2R[MaxPolyVertexCount];
    private Vector2R offset;

    public float polyReach = 0;
    public Texture2D testTexture;
    private Vector2R trueOffset;
    public Vector2R[] vertices = new Vector2R[MaxPolyVertexCount];

    public int vertexCount { get; set; }

    public float LineThickness { get; set; }
    public bool RecurseDrawEnabled { get; set; }
    public int RecurseCount { get; set; }
    public float RecurseScaleReduction { get; set; }
    public bool FillEnabled { get; set; }


    public float[,] verticesP {
      get {
        float[,] result = new float[MaxPolyVertexCount, 2];
        for (int i = 0; i < MaxPolyVertexCount; i++) {
          result[i, 0] = vertices[i].X;
          result[i, 1] = vertices[i].Y;
        }
        return result;
      }
      set {
        for (int i = 0; i < MaxPolyVertexCount; i++) {
          vertices[i].X = value[i, 0];
          vertices[i].Y = value[i, 1];
        }
      }
    }

    public float[,] normalsP {
      get {
        float[,] result = new float[MaxPolyVertexCount, 2];
        for (int i = 0; i < MaxPolyVertexCount; i++) {
          result[i, 0] = normals[i].X;
          result[i, 1] = normals[i].Y;
        }
        return result;
      }
      set {
        for (int i = 0; i < MaxPolyVertexCount; i++) {
          normals[i].X = value[i, 0];
          normals[i].Y = value[i, 1];
        }
      }
    }


    public Polygon() {
      if (radius == 0) radius = 1;
      LineThickness = 1f;
      RecurseDrawEnabled = false;
      RecurseCount = 1;
      RecurseScaleReduction = 0.2f;
      FillEnabled = false;
      if (body != null) {
        body.orient = body.orient;
      }
    }

    public override void Initialize() {
      ComputeMass(0.001f);
    }

    public override Shape Clone() {
      Polygon poly = new Polygon();
      poly.u = u;
      poly.RecurseCount = RecurseCount;
      poly.RecurseDrawEnabled = RecurseDrawEnabled;
      poly.RecurseScaleReduction = RecurseScaleReduction;
      poly.LineThickness = LineThickness;
      poly.FillEnabled = FillEnabled;


      for (int i = 0; i < vertexCount; i++) {
        poly.vertices[i] = vertices[i];
        poly.normals[i] = normals[i];
      }
      poly.vertexCount = vertexCount;
      poly.polyReach = polyReach;
      return poly;
    }

    public override void ComputeMass(float density) {
      //calculate centroid and moment of inertia
      Vector2R c = new Vector2R(0, 0); // centroid
      double area = 0;
      double I = 0;
      double k_inv3 = 1.0/3.0;

      for (int i1 = 0; i1 < vertexCount; i1++) {
        Vector2R p1 = vertices[i1];
        int i2 = i1 + 1 < vertexCount ? i1 + 1 : 0;
        Vector2R p2 = vertices[i2];

        double D = VMath.Cross(p1, p2);
        double triangleArea = 0.5*D;

        area += triangleArea;

        //use area to weight the centroid average, not just the vertex position
        c += VMath.MultVectDouble(p1 + p2, triangleArea*k_inv3); // triangleArea * k_inv3 * (p1 + p2);

        double intx2 = p1.X*p1.X + p2.X*p1.X + p2.X*p2.X;
        double inty2 = p1.Y*p1.Y + p2.Y*p1.Y + p2.Y*p2.Y;
        I += (0.25*k_inv3*D)*(intx2 + inty2);
      }
      c = VMath.MultVectDouble(c, 1.0/area);

      //translate verticies to centroid (make centroid (0,0)
      //for the polygon in model space)
      //Not really necessary but I like doing this anyway
      for (int i = 0; i < vertexCount; i++) {
        vertices[i] -= c;
      }

      body.mass = density*(float) area;
      body.inertia = (float) I*density;
    }

    public override void SetOrient(float radians) {
      u.Set(radians);
    }

    public override void Draw() {
      DrawPolygon(body.pos, body.color);
      if (testTexture != null)
        throw new NotImplementedException();
        //body.room.Camera.Draw(testTexture, body.pos + (trueOffset.Rotate(body.orient)) + (offset.Rotate(body.orient)),
        //                      body.color, 1f, body.orient, Layers.Over1);
    }

    public void DrawPolygon(Vector2R position, Color color) {
      //Vector2[] vertIncrements = new Vector2[vertexCount];
      //for (int i = 0; i < vertexCount; i++)
      //{
      //    vertIncrements[i] = vertices[i];
      //    vertIncrements[i].Normalize();
      //    vertIncrements[i] *= -LineThickness;
      //
      //}

      //could optimize to use the last vertex on the next iteration
      for (int i = 0; i < vertexCount; i++) {
        Vector2R a1 = (u /** (RecurseCount)*/)*vertices[i];
        Vector2R a2 = (u /** (RecurseCount)*/)*vertices[(i + 1)%vertexCount];

        Vector2R v1 = position + a1;
        Vector2R v2 = position + a2;
        body.room.Camera.DrawLine(v1, v2, LineThickness, color, (int)Layers.Over2);

        if (RecurseDrawEnabled) {
          DrawRecurse(body.pos + a1, RecurseCount, 1f);
        }
      }


      //DrawFill(body.pos, 1f);
    }

    public void DrawFill(Vector2R pos, float scale) {
      //could optimize to use the last vertex on the next iteration
      scale -= RecurseScaleReduction;
      if (scale < 0.3f) return;

      for (int i = 0; i < vertexCount; i++) {
        Vector2R a1 = u*vertices[i]*scale;
        Vector2R a2 = u*vertices[(i + 1)%vertexCount]*scale;

        Vector2R v1 = pos + a1;
        Vector2R v2 = pos + a2;
        body.room.Camera.DrawLine(v1, v2, LineThickness, body.color, (int)Layers.Under5);

        //Draw(pos, count, scale, scalediff);
      }
      DrawFill(pos, scale);
    }

    public void DrawRecurse(Vector2R pos, int count, float scale) {
      //could optimize to use the last vertex on the next iteration
      scale -= RecurseScaleReduction;
      count--;
      if (scale < 0 || count < 0) return;

      for (int i = 0; i < vertexCount; i++) {
        Vector2R a1 = (u*(i + 1))*vertices[i]*scale;
        Vector2R a2 = (u*(i + 1))*vertices[(i + 1)%vertexCount]*scale;

        Vector2R v1 = pos + a1;
        Vector2R v2 = pos + a2;
        body.room.Camera.DrawLine(v1, v2, 1f, body.color, (int)Layers.Under5);

        DrawRecurse(pos + a1, count, scale);
      }
    }

    public override ShapeType GetShapeType() {
      return ShapeType.Polygon;
    }

    //Highlight something and then use [Shift *] to put it in a comment block!---------------------------------

    public void SetCenterOfMass(Vector2R[] verts) {
      int len = verts.Length;
      if (len < 3) return;

      Set(verts, len);

      Vector2R centroid = FindCentroid(vertices, vertexCount);

      for (int i = 0; i < vertexCount; i++) {
        vertices[i] = new Vector2R(vertices[i].X - centroid.X, vertices[i].Y - centroid.Y);
      }
      //body.pos = new Vector2(x, y);
      Set(vertices, vertexCount);

      CalibrateTexture();
      //Vector2 newCentroid = FindCentroid(vertices, vertexCount);
      body.pos = centroid; // +newCentroid;
    }

    public Vector2R FindCentroid(Vector2R[] verts, int? length = null) {
      int len;
      if (length == null) {
        len = verts.Length;
      }
      else {
        len = (int) length;
      }

      float x = 0, y = 0, area = 0;
      for (int i = 0; i < len; i++) {
        Vector2R next = verts[(i + 1)%len];
        Vector2R current = verts[i];
        float factor = current.X*next.Y - next.X*current.Y;
        x += (current.X + next.X)*factor;
        y += (current.Y + next.Y)*factor;

        area += factor;
      }
      area /= 2;
      x /= 6*area;
      y /= 6*area;
      //if (x < 0 || y < 0) System.Diagnostics.Debugger.Break();

      return new Vector2R(x, y);
    }

    // half width and half height
    public void SetBox(float hw, float hh, bool fill = false) {
      vertexCount = 4;
      VMath.Set(ref vertices[0], -hw, -hh); //vertices[0].Set(-hw, -hh);
      VMath.Set(ref vertices[1], hw, -hh); //vertices[1].Set(hw, -hh);
      VMath.Set(ref vertices[2], hw, hh); //vertices[2].Set(hw, hh);
      VMath.Set(ref vertices[3], -hw, hh); //vertices[3].Set(-hw, hh);
      VMath.Set(ref normals[0], 0, -1); //normals[0].Set(0, -1);
      VMath.Set(ref normals[1], 1, 0); //normals[1].Set(1, 0);
      VMath.Set(ref normals[2], 0, 1); //normals[2].Set(0, 1);
      VMath.Set(ref normals[3], -1, 0); //normals[3].Set(-1, 0);
      polyReach = Vector2R.Distance(Vector2R.Zero, new Vector2R(hw, hh))*2;
      if (fill) {
        testTexture = CreateClippedTexture(body.texture, vertices, vertexCount, out this.offset);
        this.trueOffset = this.offset*-1f;
      }
    }

    public void CalibrateTexture() {
      float minX = vertices.Min(x => x.X);
      float maxX = vertices.Max(x => x.X);
      float minY = vertices.Min(x => x.Y);
      float maxY = vertices.Max(x => x.Y);
      this.trueOffset = new Vector2R((maxX - minX)/2, (maxY - minY)/2);
      this.testTexture = CreateClippedTexture(body.texture, vertices, vertexCount, out offset);
      this.offset = new Vector2R(offset.X, offset.Y);
    }

    public void Set(Vector2R[] verts, int count) {
      //no hulls with less than 3 verticies (ensure actual polygon)
      //Debug.Assert(count > 2 && count < MaxPolyVertexCount);
      count = Math.Min(count, MaxPolyVertexCount);

      //find the right most point in the hull
      int rightMost = 0;
      double highestXCoord = verts[0].X;
      for (int i = 1; i < count; i++) {
        double x = verts[0].X;
        if (x > highestXCoord) {
          highestXCoord = x;
          rightMost = i;
        }
        //if matching x then take farthest negative y
        else if (x == highestXCoord && verts[i].Y < verts[rightMost].Y) {
          rightMost = i;
        }
      }

      int[] hull = new int[MaxPolyVertexCount];
      int outCount = 0;
      int indexHull = rightMost;

      for (;;) {
        hull[outCount] = indexHull;
        // search for next index that wraps around the hull
        // by computing cross products to find the most counter-clockwise
        // vertex in the set, given the previous hull index
        int nextHullIndex = 0;
        for (int i = 1; i < count; i++) {
          //skip if same coordinate as we need three unique
          //points in the set to perform a cross product
          if (nextHullIndex == indexHull) {
            nextHullIndex = i;
            continue;
          }
          // cross every set of three unquie verticies
          // record each counter clockwise third vertex and add
          // to the output hull
          Vector2R e1 = verts[nextHullIndex] - verts[hull[outCount]];
          Vector2R e2 = verts[i] - verts[hull[outCount]];
          double c = VMath.Cross(e1, e2);
          if (c < 0.0f)
            nextHullIndex = i;

          // Cross product is zero then e vectors are on same line
          // therefor want to record vertex farthest along that line
          if (c == 0.0f && e2.LengthSquared() > e1.LengthSquared())
            nextHullIndex = i;
        }
        outCount++;
        indexHull = nextHullIndex;
        //conclude algorithm upon wraparound
        if (nextHullIndex == rightMost) {
          vertexCount = outCount;
          break;
        }
      }
      float maxDist = 0;

      // Copy vertices into shape's vertices
      for (int i = 0; i < vertexCount; ++i) {
        vertices[i] = verts[hull[i]];
        float dist = Vector2R.Distance(Vector2R.Zero, vertices[i]);
        if (dist > maxDist) maxDist = dist;
      }
      polyReach = maxDist*2;

      ComputeNormals();
    }

    public Texture2D CreateClippedTexture(Textures tex, Vector2R[] verts, int count, out Vector2R offset) {
      Point offsetP = new Point();
      Point[] points = new Point[count];
      for (int i = 0; i < count; i++) {
        points[i] = new Point((int) verts[i].X, (int) verts[i].Y);
      }

      Texture2D ret = Assets.ClippedBitmap(tex, points, out offsetP);
      offset = Vector2R.One;

      offset.X = offsetP.X;
      offset.Y = offsetP.Y;
      return ret;
    }

    public void ComputeNormals() {
      // Compute face normals
      for (int i1 = 0; i1 < vertexCount; ++i1) {
        int i2 = i1 + 1 < vertexCount ? i1 + 1 : 0;
        Vector2R face = vertices[i2] - vertices[i1];

        // Ensure no zero-length edges, because that's bad
        //Debug.Assert(face.LengthSquared() > GMath.EPSILON*GMath.EPSILON);

        // Calculate normal with 2D cross product between vector and scalar
        normals[i1] = new Vector2R(face.Y, -face.X);
        VMath.NormalizeSafe(ref normals[i1]);
      }
    }

    // The extreme point along a direction within a polygon
    public Vector2R GetSupport(Vector2R dir) {
      double bestProjection = -float.MaxValue; //-FLT_MAX;
      Vector2R bestVertex = new Vector2R(0, 0);

      for (int i = 0; i < vertexCount; ++i) {
        Vector2R v = vertices[i];
        double projection = Vector2R.Dot(v, dir);

        if (projection > bestProjection) {
          bestVertex = v;
          bestProjection = projection;
        }
      }
      return bestVertex;
    }
  }


  public struct Mat22 {
    //public Vector2 xCol;
    //public Vector2 yCol;
    public Vector2R Col1, Col2;

    /// <summary>
    /// Construct this matrix using columns.
    /// </summary>
    public Mat22(Vector2R c1, Vector2R c2) {
      Col1 = c1;
      Col2 = c2;
    }

    /// <summary>
    /// Construct this matrix using scalars.
    /// </summary>
    public Mat22(float a11, float a12, float a21, float a22) {
      Col1 = new Vector2R(a11, a21);
      Col2 = new Vector2R(a12,a22);
    }

    /// <summary>
    /// Construct this matrix using an angle. 
    /// This matrix becomes an orthonormal rotation matrix.
    /// </summary>
    public Mat22(float angle) {
      float c = (float) System.Math.Cos(angle), s = (float) System.Math.Sin(angle);
      Col1 = new Vector2R(c, s);
      Col2 = new Vector2R(-s, c);
    }

    /// <summary>
    /// Initialize this matrix using columns.
    /// </summary>
    public void Set(Vector2R c1, Vector2R c2) {
      Col1 = c1;
      Col2 = c2;
    }

    /// <summary>
    /// Initialize this matrix using an angle.
    /// This matrix becomes an orthonormal rotation matrix.
    /// </summary>
    public void Set(float angle) {
      float c = (float) System.Math.Cos(angle), s = (float) System.Math.Sin(angle);
      Col1.X = c;
      Col2.X = -s;
      Col1.Y = s;
      Col2.Y = c;
    }

    /// <summary>
    /// Set this to the identity matrix.
    /// </summary>
    public void SetIdentity() {
      Col1.X = 1.0f;
      Col2.X = 0.0f;
      Col1.Y = 0.0f;
      Col2.Y = 1.0f;
    }

    /// <summary>
    /// Set this matrix to all zeros.
    /// </summary>
    public void SetZero() {
      Col1.X = 0.0f;
      Col2.X = 0.0f;
      Col1.Y = 0.0f;
      Col2.Y = 0.0f;
    }

    /// <summary>
    /// Extract the angle from this matrix (assumed to be a rotation matrix).
    /// </summary>
    public float GetAngle() {
      return (float) System.Math.Atan2(Col1.Y, Col1.X);
    }

    /// <summary>
    /// Compute the inverse of this matrix, such that inv(A) * A = identity.
    /// </summary>
    public Mat22 GetInverse() {
      float a = Col1.X, b = Col2.X, c = Col1.Y, d = Col2.Y;
      Mat22 B = new Mat22();
      float det = a*d - b*c;
      //Box2DXDebug.Assert(det != 0.0f);
      //Debug.Assert(det != 0.0f);
      det = 1.0f/det;
      B.Col1.X = det*d;
      B.Col2.X = -det*b;
      B.Col1.Y = -det*c;
      B.Col2.Y = det*a;
      return B;
    }

    /// <summary>
    /// Solve A * x = b, where b is a column vector. This is more efficient
    /// than computing the inverse in one-shot cases.
    /// </summary>
    public Vector2R Solve(Vector2R b) {
      float a11 = Col1.X, a12 = Col2.X, a21 = Col1.Y, a22 = Col2.Y;
      float det = a11*a22 - a12*a21;
      //Box2DXDebug.Assert(det != 0.0f);
      //Debug.Assert(det != 0.0f);
      det = 1.0f/det;
      Vector2R x = new Vector2R();
      x.X = det*(a22*b.X - a12*b.Y);
      x.Y = det*(a11*b.Y - a21*b.X);
      return x;
    }

    public static Mat22 Identity {
      get { return new Mat22(1, 0, 0, 1); }
    }

    public static Mat22 operator +(Mat22 A, Mat22 B) {
      Mat22 C = new Mat22();
      C.Set(A.Col1 + B.Col1, A.Col2 + B.Col2);
      return C;
    }

    // switched them and collision is working properly
    public static Vector2R operator *(Mat22 m, Vector2R v) {
      //return new Vector2(m.Col1.X * v.X + m.Col1.Y * v.Y, m.Col2.X * v.X + m.Col2.Y * v.Y);
      return new Vector2R(m.Col1.X*v.X + m.Col2.X*v.Y, m.Col1.Y*v.X + m.Col2.Y*v.Y);
    }

    public static Mat22 operator *(Mat22 m, int i) {
      //return new Vector2(m.Col1.X * v.X + m.Col1.Y * v.Y, m.Col2.X * v.X + m.Col2.Y * v.Y);
      return new Mat22(m.GetAngle()*i);
    }

    public Mat22 Transpose() {
      return new Mat22(Col1.X, Col1.Y, Col2.X, Col2.Y);
    }
  }
}