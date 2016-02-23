using SharpDX;

namespace OrbitVR {
  public class Object3D {

    public Transform Transform;
  }
  public class Transform
  {
    public Transform parent;
    public Vector3 position = Vector3.Zero;
    public Quaternion rotation = Quaternion.Identity;
    public float Scale = 1;

    public Matrix getMatrix()
    {
      var ret = Matrix.AffineTransformation(Scale, rotation, -position);
      if (parent != null) ret *= parent.getMatrix();
      return ret;
    }
  }
}