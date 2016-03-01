// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Drawing;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Device = SharpDX.Direct3D11.Device;


namespace OrbitVR {

  public abstract class Game : DisposeBase {
    public RenderForm Window;
    public int Width = 1280;
    public int Height = 720;
    private List<IDisposable> disposables;
    public Device GraphicsDevice;
    public GameTime Time;
    public DeviceContext d3dDeviceContext;
    public SwapChain swapChain;
    public RenderTargetView renderTargetView;
    private Viewport viewport;
    public bool suppressDraw;
    private string name;
    public DeviceCreationFlags DeviceCreationFlags;
    public ModeDescription backBufferDesc;
    public Texture2D backBuffer;

    /// <summary>
    /// Create and initialize a new game.
    /// </summary>
    protected Game(string name) {
      // Set window properties
      this.name = name;
      disposables = new List<IDisposable>();
      Time = new GameTime(this);
      
    }

    /// <summary>
    /// Start the game.
    /// </summary>
    public void Run() {
      InitializeGraphics();
      Initialize();
      // Start the render loop
      RenderLoop.Run(Window, () => { Time.Update(); }
      

  );
    }

    public virtual void Update() {}

    protected virtual void Initialize() {}

    protected virtual void InitializeGraphics() {

      Window = new RenderForm(name)
      {
        ClientSize = new Size(Width, Height),
        AllowUserResizing = false
      };
      ToDispose(Window);
      backBufferDesc = new ModeDescription(Width, Height, new Rational(60, 1), Format.R8G8B8A8_UNorm);

      // Descriptor for the swap chain
      SwapChainDescription swapChainDesc = new SwapChainDescription()
      {
        ModeDescription = backBufferDesc,
        SampleDescription = new SampleDescription(1, 0),
        Usage = Usage.RenderTargetOutput,
        BufferCount = 1,
        OutputHandle = Window.Handle,
        IsWindowed = true
      };

      // Create device and swap chain
      Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDesc,
                                 out GraphicsDevice, out swapChain);
      ToDispose(swapChain);
      ToDispose(GraphicsDevice);
      d3dDeviceContext = GraphicsDevice.ImmediateContext;
      ToDispose(d3dDeviceContext);
      viewport = new Viewport(0, 0, Width, Height);
      d3dDeviceContext.Rasterizer.SetViewport(viewport);
      // Create render target view for back buffer
      backBuffer = swapChain.GetBackBuffer<Texture2D>(0);
        renderTargetView = new RenderTargetView(GraphicsDevice, backBuffer);
        ToDispose(renderTargetView);
        
      // Set back buffer as current render target view
      d3dDeviceContext.OutputMerger.SetRenderTargets(renderTargetView);
    }



    /// <summary>
    /// Draw the game.
    /// </summary>
    protected internal virtual void Draw() {
      // Clear the screen
      //d3dDeviceContext.ClearRenderTargetView(renderTargetView, new SharpDX.Color(32, 103, 178));

      // Set vertex buffer
      //d3dDeviceContext.InputAssembler.SetVertexBuffers(0,new VertexBufferBinding(triangleVertexBuffer, Utilities.SizeOf<Vector3>(), 0));

      // Draw the triangle
      //d3dDeviceContext.Draw(vertices.Count(), 0);

      // Swap front and back buffer
    }


    protected void ToDispose(IDisposable disposable)
    {
      disposables.Add(disposable);
    }
    protected override void Dispose(bool disposing) {
      foreach (IDisposable disposable in disposables) {
        disposable.Dispose();
      }
    }

    public virtual void EndDraw() {}
  }
}
