﻿using System;
using System.Collections;
using System.Diagnostics;
using OrbItProcs;
using SharpDX;
using SharpOVR;

public class PSMoveUtility 
{
    // Conversion between centimeters and meters (Unity Units)
    public static float CMToMeters = 1.0f / 100.0f;
    public static float MetersToCM = 100.0f;

    public static Quaternion PSMoveQuatToUnityQuat(Quaternion q)
    {
        return new Quaternion(-q.X, -q.Y, q.Z, q.W);
    }

    public static Vector3 PSMoveCSToUnityCSPosition(Vector3 p)
    {
        // Convert from OpenGL/PSMove Right Handed coordinate system to Unity Left Handed coordinate system.
        // PSMove Coordinate System -> Unity Coordinate system ==> (x, y, z) -> (-x, y, z)
        // PSMove also specifies points in centimenters, while Unity specifies them in meters
        return new Vector3(-p.X * CMToMeters, p.Y * CMToMeters, p.Z * CMToMeters);
    }

    public static void ComputeTrackingToWorldTransforms(
        Transform parentGameObjectTransform,
        ref Matrix TrackingSpaceToWorldSpacePosition,
        ref Quaternion OrientationTransform)
    {
        float TrackingCameraNearPlane = 0.0f;
        float TrackingCameraFarPlane = 0.0f;
        float TrackingCameraHHalfRadians = 0.0f;
        float TrackingCameraVHalfRadians = 0.0f;

        ComputeTrackingToWorldTransformsAndFrustum(
            parentGameObjectTransform,
            ref TrackingSpaceToWorldSpacePosition, 
            ref OrientationTransform, 
            ref TrackingCameraNearPlane,
            ref TrackingCameraFarPlane,
            ref TrackingCameraHHalfRadians,
            ref TrackingCameraVHalfRadians);
    }

    public static void ComputeTrackingToWorldTransformsAndFrustum(
        Transform parentGameObjectTransform,
        ref Matrix TrackingSpaceToWorldSpacePosition, 
        ref Quaternion OrientationTransform,
        ref float TrackingCameraNearPlane, 
        ref float TrackingCameraFarPlane, 
        ref float TrackingCameraHHalfRadians,
        ref float TrackingCameraVHalfRadians)
    {
        // Get the world game camera transform for the player
        Quaternion ParentGameObjectOrientation = 
            (parentGameObjectTransform != null) ? parentGameObjectTransform.rotation : Quaternion.Identity;
        Vector3 ParentGameObjectLocation = 
            (parentGameObjectTransform != null) ? parentGameObjectTransform.position : Vector3.Zero;

      TrackingState trackingState = OrbIt.game.hmd.GetTrackingState(0.0);
        if ((OrbIt.game.hmd.TrackingCaps | TrackingCapabilities.None) != 0)//TODO: trackingState.RawSensorD != null && OVRManager.tracker.isPresent && OVRManager.tracker.isEnabled)
        {
            Vector3 TrackingCameraOrigin = Vector3.Zero;
            Quaternion TrackingCameraOrientation = Quaternion.Identity;
            float TrackingCameraHFOVDegrees = 0.0f;
            float TrackingCameraVFOVDegrees = 0.0f;

            // Get the camera pose in player reference frame, UE4 CS (LHS), Unreal Units
            GetPositionalTrackingCameraProperties(
                    ref TrackingCameraOrigin, ref TrackingCameraOrientation,
                    ref TrackingCameraHFOVDegrees, ref TrackingCameraVFOVDegrees,
                    ref TrackingCameraNearPlane, ref TrackingCameraFarPlane);

            TrackingCameraHHalfRadians = MathHelper.ToRadians(TrackingCameraHFOVDegrees / 2.0f);
            TrackingCameraVHalfRadians = MathHelper.ToRadians(TrackingCameraVFOVDegrees / 2.0f);

            // Apply the parent game object orientation THEN apply tracking camera orientation
            Quaternion TrackingCameraToGameRotation = ParentGameObjectOrientation * TrackingCameraOrientation;

            // Compute the tracking camera location in world space
            Vector3 TrackingCameraWorldSpaceOrigin = Vector3.Transform(TrackingCameraOrigin,
                ParentGameObjectOrientation) + ParentGameObjectLocation;

            // Compute the Transform to go from Tracking Camera Space to World Space
          TrackingSpaceToWorldSpacePosition = Matrix.RotationQuaternion(TrackingCameraToGameRotation)*
                                              Matrix.Translation(TrackingCameraWorldSpaceOrigin); //TODO: Test!
        }
        else
        {
            // DK2 Camera Frustum properties
            const float k_default_tracking_hfov_degrees = 74.0f; // degrees
            const float k_default_tracking_vfov_degrees = 54.0f;  // degrees
            const float k_default_tracking_distance = 1.5f; // meters
            const float k_default_tracking_near_plane_distance = 0.4f; // meters
            const float k_default_tracking_far_plane_distance = 2.5f; // meters

            // Pretend that the tracking camera is directly in front of the game camera
            const float FakeTrackingCameraOffset = k_default_tracking_distance;
            Vector3 FakeTrackingCameraWorldSpaceOrigin =
                ParentGameObjectLocation + (Vector3.Transform(Vector3.ForwardLH, 
                ParentGameObjectOrientation)) * FakeTrackingCameraOffset;

            // Get tracking frustum properties from defaults
            TrackingCameraHHalfRadians = MathHelper.ToRadians(k_default_tracking_hfov_degrees / 2.0f);
            TrackingCameraVHalfRadians = MathHelper.ToRadians(k_default_tracking_vfov_degrees / 2.0f);
            TrackingCameraNearPlane = k_default_tracking_near_plane_distance;
            TrackingCameraFarPlane = k_default_tracking_far_plane_distance;

      // Compute the Transform to go from faux tracking camera Space to World Space
      TrackingSpaceToWorldSpacePosition = Matrix.RotationQuaternion(ParentGameObjectOrientation) *
                              Matrix.Translation(FakeTrackingCameraWorldSpaceOrigin); //TODO: Test!
      TrackingSpaceToWorldSpacePosition = TrackingSpaceToWorldSpacePosition = Matrix.RotationQuaternion(ParentGameObjectOrientation) *
                              Matrix.Translation(FakeTrackingCameraWorldSpaceOrigin); //TODO: Test!
    }

        // Transform the orientation of the controller from world space to camera space
        OrientationTransform = ParentGameObjectOrientation;
    }

    public static void GetPositionalTrackingCameraProperties(
        ref Vector3 position,
        ref Quaternion rotation,
        ref float cameraHFov,
        ref float cameraVFov,
        ref float cameraNearZ,
        ref float cameraFarZ)
    {
        TrackingState ss = OrbIt.game.hmd.GetTrackingState(0.0);

    rotation = new Quaternion(ss.HeadPose.ThePose.Orientation.X,
                              ss.HeadPose.ThePose.Orientation.Y,
                              ss.HeadPose.ThePose.Orientation.Z,
                              ss.HeadPose.ThePose.Orientation.W);

        position = new Vector3(ss.HeadPose.ThePose.Position.X, // meters
                               ss.HeadPose.ThePose.Position.Y,
                               ss.HeadPose.ThePose.Position.Z);
    
        cameraHFov = 74; // degrees
        cameraVFov = 54; // degrees
        cameraNearZ = 0.4f; // meters
        cameraFarZ = 2.5f; // meters
    }

    // Debug Rendering
    public static void DebugDrawHMDFrustum(Transform parentGameObjectTransform)
    {
        Matrix TrackingToWorldTransform = Matrix.Identity;
        Quaternion OrientationTransform= Quaternion.Identity;
        float TrackingCameraNearPlane= 0.0f;
        float TrackingCameraFarPlane= 0.0f;
        float TrackingCameraHHalfRadians= 0.0f;
        float TrackingCameraVHalfRadians= 0.0f;

        ComputeTrackingToWorldTransformsAndFrustum(
            parentGameObjectTransform,
            ref TrackingToWorldTransform,
            ref OrientationTransform,
            ref TrackingCameraNearPlane,
            ref TrackingCameraFarPlane,
            ref TrackingCameraHHalfRadians,
            ref TrackingCameraVHalfRadians);

        float HorizontalRatio = (float)Math.Tan(TrackingCameraHHalfRadians);
        float VerticalRatio = (float)Math.Tan(TrackingCameraVHalfRadians);

        float HalfNearWidth = TrackingCameraNearPlane * HorizontalRatio;
        float HalfNearHeight = TrackingCameraNearPlane * VerticalRatio;

        float HalfFarWidth = TrackingCameraFarPlane * HorizontalRatio;
        float HalfFarHeight = TrackingCameraFarPlane * VerticalRatio;

        Func<Vector4,Vector3> v3 = (v) => new Vector3(v.X,v.Y,v.Z);
        Vector3 Origin = v3(TrackingToWorldTransform.Column4);
        Vector3 XAxis = v3(TrackingToWorldTransform.Column1);
        Vector3 YAxis = v3(TrackingToWorldTransform.Column2);
        Vector3 ZAxis = v3(TrackingToWorldTransform.Column3);

        

        Vector3 NearV0 = TrackingToWorldTransform.MultiplyPoint3x4(new Vector3(HalfNearWidth, HalfNearHeight, TrackingCameraNearPlane));
        Vector3 NearV1 = TrackingToWorldTransform.MultiplyPoint3x4(new Vector3(-HalfNearWidth, HalfNearHeight, TrackingCameraNearPlane));
        Vector3 NearV2 = TrackingToWorldTransform.MultiplyPoint3x4(new Vector3(-HalfNearWidth, -HalfNearHeight, TrackingCameraNearPlane));
        Vector3 NearV3 = TrackingToWorldTransform.MultiplyPoint3x4(new Vector3(HalfNearWidth, -HalfNearHeight, TrackingCameraNearPlane));

        Vector3 FarV0 = TrackingToWorldTransform.MultiplyPoint3x4(new Vector3(HalfFarWidth, HalfFarHeight, TrackingCameraFarPlane));
        Vector3 FarV1 = TrackingToWorldTransform.MultiplyPoint3x4(new Vector3(-HalfFarWidth, HalfFarHeight, TrackingCameraFarPlane));
        Vector3 FarV2 = TrackingToWorldTransform.MultiplyPoint3x4(new Vector3(-HalfFarWidth, -HalfFarHeight, TrackingCameraFarPlane));
        Vector3 FarV3 = TrackingToWorldTransform.MultiplyPoint3x4(new Vector3(HalfFarWidth, -HalfFarHeight, TrackingCameraFarPlane));

        //Debug.DrawLine(Origin, FarV0, Color.yellow);
        //Debug.DrawLine(Origin, FarV1, Color.yellow);
        //Debug.DrawLine(Origin, FarV2, Color.yellow);
        //Debug.DrawLine(Origin, FarV3, Color.yellow);
        //
        //Debug.DrawLine(NearV0, NearV1, Color.yellow);
        //Debug.DrawLine(NearV1, NearV2, Color.yellow);
        //Debug.DrawLine(NearV2, NearV3, Color.yellow);
        //Debug.DrawLine(NearV3, NearV0, Color.yellow);
        //
        //Debug.DrawLine(FarV0, FarV1, Color.yellow);
        //Debug.DrawLine(FarV1, FarV2, Color.yellow);
        //Debug.DrawLine(FarV2, FarV3, Color.yellow);
        //Debug.DrawLine(FarV3, FarV0, Color.yellow);
        //
        //Debug.DrawLine(Origin, Origin + XAxis, Color.red);
        //Debug.DrawLine(Origin, Origin + YAxis, Color.green);
        //Debug.DrawLine(Origin, Origin + ZAxis, Color.blue);
    }
}
