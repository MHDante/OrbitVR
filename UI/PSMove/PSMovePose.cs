/**
* PSMove API - A Unity5 plugin for the PSMove motion controller.
*              Derived from the psmove-ue4 plugin by Chadwick Boulay
*              and the UniMove plugin by the Copenhagen Game Collective
* Copyright (C) 2015, PolyarcGames (http://www.polyarcgames.com)
*                   Brendan Walker (brendan@polyarcgames.com)
* 
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*
*    1. Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*
*    2. Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
* IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
* ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
* LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
* CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
* SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
* CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
* ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
* POSSIBILITY OF SUCH DAMAGE.
**/

using System;
using SharpDX;

namespace OrbitVR.PSMove {
  public class PSMovePose {
    public Quaternion UncorrectedWorldOrientation;
    public Vector3 UncorrectedWorldPosition;
    public Quaternion WorldOrientation;
    public Vector3 WorldPosition;
    public Vector3 ZeroPosition;
    public Quaternion ZeroYaw;

    public PSMovePose() {
      Clear();
    }

    public void Clear() {
      WorldPosition = Vector3.Zero;
      ZeroPosition = Vector3.Zero;
      UncorrectedWorldPosition = Vector3.Zero;
      WorldOrientation = Quaternion.Identity;
      ZeroYaw = Quaternion.Identity;
      UncorrectedWorldOrientation = Quaternion.Identity;
    }

    public void PoseUpdate(PSMoveDataContext DataContext, Transform ParentGameObjectTransform) {
      Matrix TrackingSpaceToWorldSpacePosition = Matrix.Identity;
      Quaternion OrientationTransform = Quaternion.Identity;

      PSMoveUtility.ComputeTrackingToWorldTransforms(
                                                     ParentGameObjectTransform,
                                                     ref TrackingSpaceToWorldSpacePosition,
                                                     ref OrientationTransform);

      if (DataContext.GetIsSeenByTracker()) {
        // The PSMove position is given in the space of the rift camera in centimeters
        Vector3 PSMPosTrackingSpace = DataContext.GetTrackingSpacePosition();
        // Transform to world space
        Vector3 PSMPosWorldSpace =
          Vector3.TransformCoordinate(PSMoveUtility.PSMoveCSToUnityCSPosition(PSMPosTrackingSpace),
                                      TrackingSpaceToWorldSpacePosition);

        // Save the resulting position, updating for internal offset
        UncorrectedWorldPosition = PSMPosWorldSpace;
        WorldPosition = PSMPosWorldSpace - ZeroPosition;
      }

      // The PSMove orientation is given in its native coordinate system
      Quaternion PSMOriNative = DataContext.GetTrackingSpaceOrientation();
      // Apply controller orientation first, then apply orientation transform
      Quaternion PSMOriWorld =
        PSMoveUtility.PSMoveQuatToUnityQuat(PSMOriNative)* OrientationTransform;

      // Save the resulting pose, updating for internal zero yaw
      UncorrectedWorldOrientation = PSMOriWorld;
      WorldOrientation = ZeroYaw*PSMOriWorld;
    }

    public void ResetYawSnapshot() {
      ZeroYaw = Quaternion.Identity;
    }

    public void SnapshotOrientationYaw() {
      float Magnitude =
        (float) Math.Sqrt(UncorrectedWorldOrientation.Y*UncorrectedWorldOrientation.Y +
                          UncorrectedWorldOrientation.W*UncorrectedWorldOrientation.W);

      // Strip out the x and z (pitch and roll) components of rotation and negate the yaw-axis
      // Then normalize the resulting quaternion
      ZeroYaw.X = 0;
      ZeroYaw.Y = -UncorrectedWorldOrientation.Y/Magnitude;
      ZeroYaw.Z = 0;
      ZeroYaw.W = UncorrectedWorldOrientation.W/Magnitude;
    }

    public void ResetPositionSnapshot() {
      ZeroPosition = Vector3.Zero;
    }

    public void SnapshotPosition() {
      ZeroPosition = UncorrectedWorldPosition;
    }
  };
}