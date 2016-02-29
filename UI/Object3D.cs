using SharpDX;
using SharpDX.Direct3D11;

namespace OrbitVR {
  public class Object3D {
    public Transform Transform;
    private Buffer triangleVertexBuffer;

    Vector3[] vertices = { new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(0.5f, 0.5f, 0.0f), new Vector3(0.0f, -0.5f, 0.0f) };

    public Object3D()
    {
      triangleVertexBuffer = Buffer.Create(OrbIt.Game.GraphicsDevice, BindFlags.VertexBuffer, vertices);
    }
  }

  public class Transform {
    public Transform parent;
    public Vector3 position = Vector3.Zero;
    public Quaternion rotation = Quaternion.Identity;
    public float Scale = 1;


    public Matrix getMatrix() {
      var ret = Matrix.AffineTransformation(Scale, rotation, -position);
      if (parent != null) ret *= parent.getMatrix();
      return ret;
    }
  }
}