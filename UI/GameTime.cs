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
using SharpDX;

namespace OrbitVR
{
  /// <summary>
  /// Current timing used for variable-step (real time) or fixed-step (game time) games.
  /// </summary>
  public class GameTime
  {
    private TimeSpan totalGameTime;
    private TimerTick timer;

    public TimeSpan maximumElapsedTime;
    public bool IsFixedTimeStep = false;
    public TimeSpan TargetElapsedTime;
    private TimeSpan accumulatedElapsedGameTime;
    private int[] lastUpdateCount;
    private int nextLastUpdateCountIndex;
    private float updateCountAverageSlowLimit;
    private bool drawRunningSlowly;
    private TimeSpan lastFrameElapsedGameTime;
    private Game game;

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="GameTime" /> class.
    /// </summary>
    /// <param name="game"></param>
    public GameTime(Game game) {
      this.game = game;

      maximumElapsedTime = TimeSpan.FromMilliseconds(500.0);
      TargetElapsedTime = TimeSpan.FromTicks(10000000 / 60); // target elapsed time is by default 60Hz
      totalGameTime = new TimeSpan();

      lastUpdateCount = new int[4];
      nextLastUpdateCountIndex = 0;

      // Calculate the updateCountAverageSlowLimit (assuming moving average is >=3 )
      // Example for a moving average of 4:
      // updateCountAverageSlowLimit = (2 * 2 + (4 - 2)) / 4 = 1.5f
      const int BadUpdateCountTime = 2; // number of bad frame (a bad frame is a frame that has at least 2 updates)
      var maxLastCount = 2 * Math.Min(BadUpdateCountTime, lastUpdateCount.Length);
      updateCountAverageSlowLimit = (float)(maxLastCount + (lastUpdateCount.Length - maxLastCount)) / lastUpdateCount.Length;


      timer = new TimerTick();
      timer.Reset();
      Update(totalGameTime, TimeSpan.Zero, false);
      FrameCount = 0;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the elapsed game time since the last update
    /// </summary>
    /// <value>The elapsed game time.</value>
    public TimeSpan ElapsedGameTime { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the game is running slowly than its TargetElapsedTime. This can be used for example to render less details...etc.
    /// </summary>
    /// <value><c>true</c> if this instance is running slowly; otherwise, <c>false</c>.</value>
    public bool IsRunningSlowly { get; private set; }

    /// <summary>
    /// Gets the amount of game time since the start of the game.
    /// </summary>
    /// <value>The total game time.</value>
    public TimeSpan TotalGameTime { get; private set; }

    /// <summary>
    /// Gets the current frame count since the start of the game.
    /// </summary>
    public int FrameCount { get; internal set; }

    public void Update() {
      timer.Tick();
      var elapsedAdjustedTime = timer.ElapsedAdjustedTime;

      if (elapsedAdjustedTime > maximumElapsedTime)
      {
        elapsedAdjustedTime = maximumElapsedTime;
      }

      bool suppressNextDraw = true;
      int updateCount = 1;
      var singleFrameElapsedTime = elapsedAdjustedTime;
      if (IsFixedTimeStep)
      {
        // If the rounded TargetElapsedTime is equivalent to current ElapsedAdjustedTime
        // then make ElapsedAdjustedTime = TargetElapsedTime. We take the same internal rules as XNA 
        if (Math.Abs(elapsedAdjustedTime.Ticks - TargetElapsedTime.Ticks) < (TargetElapsedTime.Ticks >> 6))
        {
          elapsedAdjustedTime = TargetElapsedTime;
        }

        // Update the accumulated time
        accumulatedElapsedGameTime += elapsedAdjustedTime;

        // Calculate the number of update to issue
        updateCount = (int)(accumulatedElapsedGameTime.Ticks / TargetElapsedTime.Ticks);

        // If there is no need for update, then exit
        if (updateCount == 0)
        {
          // check if we can sleep the thread to free CPU resources
          var sleepTime = TargetElapsedTime - accumulatedElapsedGameTime;
          if (sleepTime > TimeSpan.Zero)
            Utilities.Sleep(sleepTime);

          return;
        }

        // Calculate a moving average on updateCount
        lastUpdateCount[nextLastUpdateCountIndex] = updateCount;
        float updateCountMean = 0;
        for (int i = 0; i < lastUpdateCount.Length; i++)
        {
          updateCountMean += lastUpdateCount[i];
        }

        updateCountMean /= lastUpdateCount.Length;
        nextLastUpdateCountIndex = (nextLastUpdateCountIndex + 1) % lastUpdateCount.Length;

        // Test when we are running slowly
        drawRunningSlowly = updateCountMean > updateCountAverageSlowLimit;

        // We are going to call Update updateCount times, so we can subtract this from accumulated elapsed game time
        accumulatedElapsedGameTime = new TimeSpan(accumulatedElapsedGameTime.Ticks - (updateCount * TargetElapsedTime.Ticks));
        singleFrameElapsedTime = TargetElapsedTime;
      }
      else
      {
        Array.Clear(lastUpdateCount, 0, lastUpdateCount.Length);
        nextLastUpdateCountIndex = 0;
        drawRunningSlowly = false;
      }

      // Reset the time of the next frame
      for (lastFrameElapsedGameTime = TimeSpan.Zero; updateCount > 0 /*&& !isExiting*/; updateCount--)
      {
        Update(totalGameTime, singleFrameElapsedTime, drawRunningSlowly);
        try
        {
          game.Update();

          // If there is no exception, then we can draw the frame
          suppressNextDraw &= game.suppressDraw;
          game.suppressDraw = false;
        }
        finally
        {
          lastFrameElapsedGameTime += singleFrameElapsedTime;
          totalGameTime += singleFrameElapsedTime;
        }
      }

      if (!suppressNextDraw)
      {
        DrawFrame();
      }


    }
    private void DrawFrame()
    {
      try
      {
          Update(totalGameTime, lastFrameElapsedGameTime, drawRunningSlowly);
          FrameCount++;

        game.Draw();

        game.EndDraw();
      }
      finally
      {
        lastFrameElapsedGameTime = TimeSpan.Zero;
      }
    }


    private void Update(TimeSpan totalGameTime, TimeSpan elapsedGameTime, bool isRunningSlowly)
    {
      TotalGameTime = totalGameTime;
      ElapsedGameTime = elapsedGameTime;
      IsRunningSlowly = isRunningSlowly;
    }

    #endregion
  }
}