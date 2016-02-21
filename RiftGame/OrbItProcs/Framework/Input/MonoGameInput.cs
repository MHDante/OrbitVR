//shamelessly copied from monogame. no questions
#region License
// /*
// Microsoft Public License (Ms-PL)
// MonoGame - Copyright © 2009 The MonoGame Team
// 
// All rights reserved.
// 
// This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
// accept the license, do not use the software.
// 
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under 
// U.S. copyright law.
// 
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
// 
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
// each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
// each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
// 
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
// your patent license from such contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution 
// notices that are present in the software.
// (D) If you distribute any portion of the software in source code form, you may do so only under this license by including 
// a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object 
// code form, you may only do so under a license that complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
// or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
// permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
// purpose and non-infringement.
// */
#endregion License



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpOVR;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.DXGI;
using SharpDX.Toolkit.Content;
using GBF = SharpDX.XInput.GamepadButtonFlags;



#if MONOMAC || WINDOWS
using System.Runtime.InteropServices;
using System.Drawing;
#endif

#if OPENGL
#if DESKTOPGL
using MouseInfo = OpenTK.Input.Mouse;
#elif MONOMAC
#if PLATFORM_MACOS_LEGACY
using MonoMac.Foundation;
using MonoMac.AppKit;
#else
using Foundation;
using AppKit;
using PointF = CoreGraphics.CGPoint;
#endif
#endif
#endif

namespace OrbItProcs
{
    /// <summary>
    /// Defines a button state for buttons of mouse, gamepad or joystick.
    /// </summary>
    public enum ButtonState
    {
        /// <summary>
        /// The button is released.
        /// </summary>
        Released,

        /// <summary>
        /// The button is pressed.
        /// </summary>
        Pressed
    }
    /// <summary>
    /// Defines the index of player for various MonoGame components.
    /// </summary>
    public enum PlayerIndex
    {
        /// <summary>
        /// The first player index.
        /// </summary>
        One = 0,
        /// <summary>
        /// The second player index.
        /// </summary>
        Two = 1,
        /// <summary>
        /// The third player index.
        /// </summary>
        Three = 2,
        /// <summary>
        /// The fourth player index.
        /// </summary>
        Four = 3
    }
    // Summary:
    //     Specifies a type of dead zone processing to apply to Xbox 360 Controller
    //     analog sticks when calling GetState.
    //
    // Parameters:
    //   Circular:
    //     The combined X and Y position of each stick is compared to the dead zone.
    //     This provides better control than IndependentAxes when the stick is used
    //     as a two-dimensional control surface, such as when controlling a character's
    //     view in a first-person game.
    //
    //   IndependentAxes:
    //     The X and Y positions of each stick are compared against the dead zone independently.
    //     This setting is the default when calling GetState.
    //
    //   None:
    //     The values of each stick are not processed and are returned by GetState as
    //     "raw" values. This is best if you intend to implement your own dead zone
    //     processing.
    public enum GamePadDeadZone
    {
        None,
        IndependentAxes,
        Circular
    };
    public static partial class GamePad
    {
        /// <summary>
        /// Returns the capabilites of the connected controller.
        /// </summary>
        /// <param name="playerIndex">Player index for the controller you want to query.</param>
        /// <returns>The capabilites of the controller.</returns>
        public static GamePadCapabilities GetCapabilities(PlayerIndex playerIndex)
        {
            return GetCapabilities((int)playerIndex);
        }

        /// <summary>
        /// Returns the capabilites of the connected controller.
        /// </summary>
        /// <param name="index">Index for the controller you want to query.</param>
        /// <returns>The capabilites of the controller.</returns>
        public static GamePadCapabilities GetCapabilities(int index)
        {
            if (index < 0 || index >= PlatformGetMaxNumberOfGamePads())
                throw new InvalidOperationException();

            return PlatformGetCapabilities(index);
        }

        /// <summary>
        /// Gets the current state of a game pad controller with an independent axes dead zone.
        /// </summary>
        /// <param name="playerIndex">Player index for the controller you want to query.</param>
        /// <returns>The state of the controller.</returns>
        public static GamePadState GetState(PlayerIndex playerIndex)
        {
            return GetState((int)playerIndex, GamePadDeadZone.IndependentAxes);
        }

        /// <summary>
        /// Gets the current state of a game pad controller with an independent axes dead zone.
        /// </summary>
        /// <param name="index">Index for the controller you want to query.</param>
        /// <returns>The state of the controller.</returns>
        public static GamePadState GetState(int index)
        {
            return GetState(index, GamePadDeadZone.IndependentAxes);
        }

        /// <summary>
        /// Gets the current state of a game pad controller, using a specified dead zone
        /// on analog stick positions.
        /// </summary>
        /// <param name="playerIndex">Player index for the controller you want to query.</param>
        /// <param name="deadZoneMode">Enumerated value that specifies what dead zone type to use.</param>
        /// <returns>The state of the controller.</returns>
        public static GamePadState GetState(PlayerIndex playerIndex, GamePadDeadZone deadZoneMode)
        {
            return GetState((int)playerIndex, deadZoneMode);
        }

        /// <summary>
        /// Gets the current state of a game pad controller, using a specified dead zone
        /// on analog stick positions.
        /// </summary>
        /// <param name="index">Index for the controller you want to query.</param>
        /// <param name="deadZoneMode">Enumerated value that specifies what dead zone type to use.</param>
        /// <returns>The state of the controller.</returns>
        public static GamePadState GetState(int index, GamePadDeadZone deadZoneMode)
        {
            if (index < 0 || index >= PlatformGetMaxNumberOfGamePads())
                throw new InvalidOperationException();

            return PlatformGetState(index, deadZoneMode);
        }

        /// <summary>
        /// Sets the vibration motor speeds on the controller device if supported.
        /// </summary>
        /// <param name="playerIndex">Player index that identifies the controller to set.</param>
        /// <param name="leftMotor">The speed of the left motor, between 0.0 and 1.0. This motor is a low-frequency motor.</param>
        /// <param name="rightMotor">The speed of the right motor, between 0.0 and 1.0. This motor is a high-frequency motor.</param>
        /// <returns>Returns true if the vibration motors were set.</returns>
        public static bool SetVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor)
        {
            return SetVibration((int)playerIndex, leftMotor, rightMotor);
        }

        /// <summary>
        /// Sets the vibration motor speeds on the controller device if supported.
        /// </summary>
        /// <param name="index">Index for the controller you want to query.</param>
        /// <param name="leftMotor">The speed of the left motor, between 0.0 and 1.0. This motor is a low-frequency motor.</param>
        /// <param name="rightMotor">The speed of the right motor, between 0.0 and 1.0. This motor is a high-frequency motor.</param>
        /// <returns>Returns true if the vibration motors were set.</returns>
        public static bool SetVibration(int index, float leftMotor, float rightMotor)
        {
            if (index < 0 || index >= PlatformGetMaxNumberOfGamePads())
                throw new InvalidOperationException();

            return PlatformSetVibration(index, MathHelper.Clamp(leftMotor, 0.0f, 1.0f), MathHelper.Clamp(rightMotor, 0.0f, 1.0f));
        }

        /// <summary>
        /// The maximum number of game pads supported on this system.  Attempting to
        /// access a gamepad index higher than this number will result in an <see cref="InvalidOperationException"/>
        /// being thrown by the API.
        /// </summary>
        public static int MaximumGamePadCount
        {
            get { return PlatformGetMaxNumberOfGamePads(); }
        }
    }

    static partial class GamePad
    {
        internal static bool Back;

        private static readonly SharpDX.XInput.Controller[] _controllers = new[]
        {
            new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.One),
            new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.Two),
            new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.Three),
            new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.Four),
        };

        private static readonly bool[] _connected = new bool[4];
        private static readonly long[] _timeout = new long[4];
        private static readonly long TimeoutTicks = TimeSpan.FromSeconds(1).Ticks;

        private static int PlatformGetMaxNumberOfGamePads()
        {
            return 4;
        }

        private static GamePadCapabilities PlatformGetCapabilities(int index)
        {
            // If the device was disconneced then wait for 
            // the timeout to elapsed before we test it again.
            if (!_connected[index] && _timeout[index] > DateTime.UtcNow.Ticks)
                return new GamePadCapabilities();

            // Check to see if the device is connected.
            var controller = _controllers[index];
            _connected[index] = controller.IsConnected;

            // If the device is disconnected retry it after the
            // timeout period has elapsed to avoid the overhead.
            if (!_connected[index])
            {
                _timeout[index] = DateTime.UtcNow.Ticks + TimeoutTicks;
                return new GamePadCapabilities();
            }

            var capabilities = controller.GetCapabilities(SharpDX.XInput.DeviceQueryType.Any);
            var ret = new GamePadCapabilities();
            switch (capabilities.SubType)
            {
#if DIRECTX11_1
                case SharpDX.XInput.DeviceSubType.ArcadePad:
                    Debug.WriteLine("XInput's DeviceSubType.ArcadePad is not supported in XNA");
                    ret.GamePadType = Input.GamePadType.Unknown; // TODO: Should this be BigButtonPad?
                    break;
                case SharpDX.XInput.DeviceSubType.FlightStick:
                    ret.GamePadType = Input.GamePadType.FlightStick;
                    break;
                case SharpDX.XInput.DeviceSubType.GuitarAlternate:
                    ret.GamePadType = Input.GamePadType.AlternateGuitar;
                    break;
                case SharpDX.XInput.DeviceSubType.GuitarBass:
                    // Note: XNA doesn't distinguish between Guitar and GuitarBass, but 
                    // GuitarBass is identical to Guitar in XInput, distinguished only
                    // to help setup for those controllers. 
                    ret.GamePadType = Input.GamePadType.Guitar;
                    break;
                case SharpDX.XInput.DeviceSubType.Unknown:
                    ret.GamePadType = Input.GamePadType.Unknown;
                    break;
#endif
                case SharpDX.XInput.DeviceSubType.ArcadeStick:
                    ret.GamePadType = GamePadType.ArcadeStick;
                    break;
                case SharpDX.XInput.DeviceSubType.DancePad:
                    ret.GamePadType = GamePadType.DancePad;
                    break;
                case SharpDX.XInput.DeviceSubType.DrumKit:
                    ret.GamePadType = GamePadType.DrumKit;
                    break;

                case SharpDX.XInput.DeviceSubType.Gamepad:
                    ret.GamePadType = GamePadType.GamePad;
                    break;
                case SharpDX.XInput.DeviceSubType.Guitar:
                    ret.GamePadType = GamePadType.Guitar;
                    break;
                case SharpDX.XInput.DeviceSubType.Wheel:
                    ret.GamePadType = GamePadType.Wheel;
                    break;
                default:
                    Console.WriteLine("unexpected XInput DeviceSubType: {0}", capabilities.SubType.ToString());
                    ret.GamePadType = GamePadType.Unknown;
                    break;
            }

            var gamepad = capabilities.Gamepad;

            // digital buttons
            var buttons = gamepad.Buttons;
            ret.HasAButton = (buttons & GBF.A) == GBF.A;
            ret.HasBackButton = (buttons & GBF.Back) == GBF.Back;
            ret.HasBButton = (buttons & GBF.B) == GBF.B;
            ret.HasBigButton = false; // TODO: what IS this? Is it related to GamePadType.BigGamePad?
            ret.HasDPadDownButton = (buttons & GBF.DPadDown) == GBF.DPadDown;
            ret.HasDPadLeftButton = (buttons & GBF.DPadLeft) == GBF.DPadLeft;
            ret.HasDPadRightButton = (buttons & GBF.DPadRight) == GBF.DPadRight;
            ret.HasDPadUpButton = (buttons & GBF.DPadUp) == GBF.DPadUp;
            ret.HasLeftShoulderButton = (buttons & GBF.LeftShoulder) == GBF.LeftShoulder;
            ret.HasLeftStickButton = (buttons & GBF.LeftThumb) == GBF.LeftThumb;
            ret.HasRightShoulderButton = (buttons & GBF.RightShoulder) == GBF.RightShoulder;
            ret.HasRightStickButton = (buttons & GBF.RightThumb) == GBF.RightThumb;
            ret.HasStartButton = (buttons & GBF.Start) == GBF.Start;
            ret.HasXButton = (buttons & GBF.X) == GBF.X;
            ret.HasYButton = (buttons & GBF.Y) == GBF.Y;

            // analog controls
            ret.HasRightTrigger = gamepad.LeftTrigger > 0;
            ret.HasRightXThumbStick = gamepad.RightThumbX != 0;
            ret.HasRightYThumbStick = gamepad.RightThumbY != 0;
            ret.HasLeftTrigger = gamepad.LeftTrigger > 0;
            ret.HasLeftXThumbStick = gamepad.LeftThumbX != 0;
            ret.HasLeftYThumbStick = gamepad.LeftThumbY != 0;

            // vibration
#if DIRECTX11_1
            bool hasForceFeedback = (capabilities.Flags & SharpDX.XInput.CapabilityFlags.FfbSupported) == SharpDX.XInput.CapabilityFlags.FfbSupported;
            ret.HasLeftVibrationMotor = hasForceFeedback && capabilities.Vibration.LeftMotorSpeed > 0;
            ret.HasRightVibrationMotor = hasForceFeedback && capabilities.Vibration.RightMotorSpeed > 0;
#else
            ret.HasLeftVibrationMotor = false;
            ret.HasRightVibrationMotor = false;
#endif

            // other
            ret.IsConnected = controller.IsConnected;
            ret.HasVoiceSupport = (capabilities.Flags & SharpDX.XInput.CapabilityFlags.VoiceSupported) == SharpDX.XInput.CapabilityFlags.VoiceSupported;

            return ret;
        }

        private static GamePadState GetDefaultState()
        {
            var state = new GamePadState();
            state.Buttons = new GamePadButtons(Back ? Buttons.Back : 0);
            return state;
        }

        private static GamePadState PlatformGetState(int index, GamePadDeadZone deadZoneMode)
        {
            // If the device was disconneced then wait for 
            // the timeout to elapsed before we test it again.
            if (!_connected[index] && _timeout[index] > DateTime.UtcNow.Ticks)
                return GetDefaultState();

            int packetNumber = 0;

            // Try to get the controller state.
            var gamepad = new SharpDX.XInput.Gamepad();
            try
            {
                SharpDX.XInput.State xistate;
                var controller = _controllers[index];
                _connected[index] = controller.GetState(out xistate);
                packetNumber = xistate.PacketNumber;
                gamepad = xistate.Gamepad;
            }
            catch (Exception ex)
            {
            }

            // If the device is disconnected retry it after the
            // timeout period has elapsed to avoid the overhead.
            if (!_connected[index])
            {
                _timeout[index] = DateTime.UtcNow.Ticks + TimeoutTicks;
                return GetDefaultState();
            }

            var thumbSticks = new GamePadThumbSticks(
                leftPosition: new Vector2(gamepad.LeftThumbX, gamepad.LeftThumbY) / (float)short.MaxValue,
                rightPosition: new Vector2(gamepad.RightThumbX, gamepad.RightThumbY) / (float)short.MaxValue,
                    deadZoneMode: deadZoneMode);

            var triggers = new GamePadTriggers(
                    leftTrigger: gamepad.LeftTrigger / (float)byte.MaxValue,
                    rightTrigger: gamepad.RightTrigger / (float)byte.MaxValue);

            var dpadState = new GamePadDPad(
                upValue: ConvertToButtonState(gamepad.Buttons, SharpDX.XInput.GamepadButtonFlags.DPadUp),
                downValue: ConvertToButtonState(gamepad.Buttons, SharpDX.XInput.GamepadButtonFlags.DPadDown),
                leftValue: ConvertToButtonState(gamepad.Buttons, SharpDX.XInput.GamepadButtonFlags.DPadLeft),
                rightValue: ConvertToButtonState(gamepad.Buttons, SharpDX.XInput.GamepadButtonFlags.DPadRight));

            var buttons = ConvertToButtons(
                buttonFlags: gamepad.Buttons,
                leftThumbX: gamepad.LeftThumbX,
                leftThumbY: gamepad.LeftThumbY,
                rightThumbX: gamepad.RightThumbX,
                rightThumbY: gamepad.RightThumbY,
                leftTrigger: gamepad.LeftTrigger,
                rightTrigger: gamepad.RightTrigger);

            var state = new GamePadState(
                thumbSticks: thumbSticks,
                triggers: triggers,
                buttons: buttons,
                dPad: dpadState);

            state.PacketNumber = packetNumber;

            return state;
        }

        private static ButtonState ConvertToButtonState(
            SharpDX.XInput.GamepadButtonFlags buttonFlags,
            SharpDX.XInput.GamepadButtonFlags desiredButton)
        {
            return ((buttonFlags & desiredButton) == desiredButton) ? ButtonState.Pressed : ButtonState.Released;
        }

        private static Buttons AddButtonIfPressed(
            SharpDX.XInput.GamepadButtonFlags buttonFlags,
            SharpDX.XInput.GamepadButtonFlags xInputButton,
            Buttons xnaButton)
        {
            var buttonState = ((buttonFlags & xInputButton) == xInputButton) ? ButtonState.Pressed : ButtonState.Released;
            return buttonState == ButtonState.Pressed ? xnaButton : 0;
        }

        private static Buttons AddThumbstickButtons(
            short thumbX, short thumbY, short deadZone,
            Buttons thumbstickLeft,
            Buttons thumbStickRight,
            Buttons thumbStickUp,
            Buttons thumbStickDown)
        {
            // TODO: this needs adjustment. Very naive implementation. Doesn't match XNA yet
            var result = (Buttons)0;
            if (thumbX < -deadZone)
                result |= thumbstickLeft;
            if (thumbX > deadZone)
                result |= thumbStickRight;
            if (thumbY < -deadZone)
                result |= thumbStickDown;
            else if (thumbY > deadZone)
                result |= thumbStickUp;
            return result;
        }

        private static GamePadButtons ConvertToButtons(SharpDX.XInput.GamepadButtonFlags buttonFlags,
            short leftThumbX, short leftThumbY,
            short rightThumbX, short rightThumbY,
            byte leftTrigger,
            byte rightTrigger)
        {
            var ret = (Buttons)0;
            ret |= AddButtonIfPressed(buttonFlags, GBF.A, Buttons.A);
            ret |= AddButtonIfPressed(buttonFlags, GBF.B, Buttons.B);
            ret |= AddButtonIfPressed(buttonFlags, GBF.Back, Buttons.Back);
            ret |= AddButtonIfPressed(buttonFlags, GBF.DPadDown, Buttons.DPadDown);
            ret |= AddButtonIfPressed(buttonFlags, GBF.DPadLeft, Buttons.DPadLeft);
            ret |= AddButtonIfPressed(buttonFlags, GBF.DPadRight, Buttons.DPadRight);
            ret |= AddButtonIfPressed(buttonFlags, GBF.DPadUp, Buttons.DPadUp);
            ret |= AddButtonIfPressed(buttonFlags, GBF.LeftShoulder, Buttons.LeftShoulder);
            ret |= AddButtonIfPressed(buttonFlags, GBF.RightShoulder, Buttons.RightShoulder);
            ret |= AddButtonIfPressed(buttonFlags, GBF.LeftThumb, Buttons.LeftStick);
            ret |= AddButtonIfPressed(buttonFlags, GBF.RightThumb, Buttons.RightStick);
            ret |= AddButtonIfPressed(buttonFlags, GBF.Start, Buttons.Start);
            ret |= AddButtonIfPressed(buttonFlags, GBF.X, Buttons.X);
            ret |= AddButtonIfPressed(buttonFlags, GBF.Y, Buttons.Y);

            ret |= AddThumbstickButtons(leftThumbX, leftThumbY,
                SharpDX.XInput.Gamepad.LeftThumbDeadZone,
                Buttons.LeftThumbstickLeft,
                Buttons.LeftThumbstickRight,
                Buttons.LeftThumbstickUp,
                Buttons.LeftThumbstickDown);

            ret |= AddThumbstickButtons(rightThumbX, rightThumbY,
                SharpDX.XInput.Gamepad.RightThumbDeadZone,
                Buttons.RightThumbstickLeft,
                Buttons.RightThumbstickRight,
                Buttons.RightThumbstickUp,
                Buttons.RightThumbstickDown);

            if (leftTrigger >= SharpDX.XInput.Gamepad.TriggerThreshold)
                ret |= Buttons.LeftTrigger;

            if (rightTrigger >= SharpDX.XInput.Gamepad.TriggerThreshold)
                ret |= Buttons.RightTrigger;

            // Check for the hardware back button.
            if (Back)
                ret |= Buttons.Back;

            return new GamePadButtons(ret);
        }

        private static bool PlatformSetVibration(int index, float leftMotor, float rightMotor)
        {
            if (!_connected[index])
                return false;

            var controller = _controllers[index];
            var result = controller.SetVibration(new SharpDX.XInput.Vibration
            {
                LeftMotorSpeed = (ushort)(leftMotor * ushort.MaxValue),
                RightMotorSpeed = (ushort)(rightMotor * ushort.MaxValue),
            });

            return result == SharpDX.Result.Ok;
        }
    }
    public struct GamePadCapabilities
    {
        public bool IsConnected { get; internal set; }

        public bool HasAButton { get; internal set; }

        public bool HasBackButton { get; internal set; }

        public bool HasBButton { get; internal set; }

        public bool HasDPadDownButton { get; internal set; }

        public bool HasDPadLeftButton { get; internal set; }

        public bool HasDPadRightButton { get; internal set; }

        public bool HasDPadUpButton { get; internal set; }

        public bool HasLeftShoulderButton { get; internal set; }

        public bool HasLeftStickButton { get; internal set; }

        public bool HasRightShoulderButton { get; internal set; }

        public bool HasRightStickButton { get; internal set; }

        public bool HasStartButton { get; internal set; }

        public bool HasXButton { get; internal set; }

        public bool HasYButton { get; internal set; }

        public bool HasBigButton { get; internal set; }

        public bool HasLeftXThumbStick { get; internal set; }

        public bool HasLeftYThumbStick { get; internal set; }

        public bool HasRightXThumbStick { get; internal set; }

        public bool HasRightYThumbStick { get; internal set; }

        public bool HasLeftTrigger { get; internal set; }

        public bool HasRightTrigger { get; internal set; }

        public bool HasLeftVibrationMotor { get; internal set; }

        public bool HasRightVibrationMotor { get; internal set; }

        public bool HasVoiceSupport { get; internal set; }

        public GamePadType GamePadType { get; internal set; }
    }
    /// <summary>
    /// Defines a type of gamepad.
    /// </summary>
	public enum GamePadType
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown,
        /// <summary>
        /// GamePad is the XBOX controller.
        /// </summary>
        GamePad,
        /// <summary>
        /// GamePad is a wheel.
        /// </summary>
		Wheel,
        /// <summary>
        /// GamePad is an arcade stick.
        /// </summary>
        ArcadeStick,
        /// <summary>
        /// GamePad is a flight stick.
        /// </summary>
		FlightStick,
        /// <summary>
        /// GamePad is a dance pad.
        /// </summary>
		DancePad,
        /// <summary>
        /// GamePad is a guitar.
        /// </summary>
		Guitar,
        /// <summary>
        /// GamePad is an alternate guitar.
        /// </summary>
        AlternateGuitar,
        /// <summary>
        /// GamePad is a drum kit.
        /// </summary>
        DrumKit,
        /// <summary>
        /// GamePad is a big button pad.
        /// </summary>
        BigButtonPad = 768
    }
    //
    // Summary:
    //     Represents specific information about the state of an Xbox 360 Controller,
    //     including the current state of buttons and sticks. Reference page contains
    //     links to related code samples.
    //     This is implemented as a partial struct to allow for individual platforms
    //     to offer additional data without separate state queries to GamePad.
    public partial struct GamePadState
    {
        /// <summary>
        /// The default initialized gamepad state.
        /// </summary>
        public static readonly GamePadState Default = new GamePadState();

        //
        // Summary:
        //     Indicates whether the Xbox 360 Controller is connected. Reference page contains
        //     links to related code samples.
        public bool IsConnected
        {
            get;
            internal set;
        }
        //
        // Summary:
        //     Gets the packet number associated with this state. Reference page contains
        //     links to related code samples.
        public int PacketNumber
        {
            get;
            internal set;
        }

        //
        // Summary:
        //     Returns a structure that identifies what buttons on the Xbox 360 controller
        //     are pressed. Reference page contains links to related code samples.
        public GamePadButtons Buttons
        {
            get;
            internal set;
        }
        //
        // Summary:
        //     Returns a structure that identifies what directions of the directional pad
        //     on the Xbox 360 Controller are pressed.
        public GamePadDPad DPad
        {
            get;
            internal set;
        }
        //
        // Summary:
        //     Returns a structure that indicates the position of the Xbox 360 Controller
        //     sticks (thumbsticks).
        public GamePadThumbSticks ThumbSticks
        {
            get;
            internal set;
        }
        //
        // Summary:
        //     Returns a structure that identifies the position of triggers on the Xbox
        //     360 controller.
        public GamePadTriggers Triggers
        {
            get;
            internal set;
        }

        //
        // Summary:
        //     Initializes a new instance of the GamePadState class using the specified
        //     GamePadThumbSticks, GamePadTriggers, GamePadButtons, and GamePadDPad.
        //
        // Parameters:
        //   thumbSticks:
        //     Initial thumbstick state.
        //
        //   triggers:
        //     Initial trigger state.
        //
        //   buttons:
        //     Initial button state.
        //
        //   dPad:
        //     Initial directional pad state.
        public GamePadState(GamePadThumbSticks thumbSticks, GamePadTriggers triggers, GamePadButtons buttons, GamePadDPad dPad)
            : this()
        {
            ThumbSticks = thumbSticks;
            Triggers = triggers;
            Buttons = buttons;
            DPad = dPad;
            IsConnected = true;

            PlatformConstruct();
        }
        //
        // Summary:
        //     Initializes a new instance of the GamePadState class with the specified stick,
        //     trigger, and button values.
        //
        // Parameters:
        //   leftThumbStick:
        //     Left stick value. Each axis is clamped between −1.0 and 1.0.
        //
        //   rightThumbStick:
        //     Right stick value. Each axis is clamped between −1.0 and 1.0.
        //
        //   leftTrigger:
        //     Left trigger value. This value is clamped between 0.0 and 1.0.
        //
        //   rightTrigger:
        //     Right trigger value. This value is clamped between 0.0 and 1.0.
        //
        //   buttons:
        //     Array or parameter list of Buttons to initialize as pressed.
        public GamePadState(Vector2 leftThumbStick, Vector2 rightThumbStick, float leftTrigger, float rightTrigger, params Buttons[] buttons)
            : this(new GamePadThumbSticks(leftThumbStick, rightThumbStick), new GamePadTriggers(leftTrigger, rightTrigger), new GamePadButtons(buttons), new GamePadDPad(buttons))
        {
        }

        /// <summary>
        /// Define this method in platform partial classes to initialize default
        /// values for platform-specific fields.
        /// </summary>
        partial void PlatformConstruct();

        /// <summary>
        /// Gets the button mask along with 'virtual buttons' like LeftThumbstickLeft.
        /// </summary>
        private Buttons GetVirtualButtons()
        {
            var result = Buttons.buttons;
            var sticks = ThumbSticks;

            if (sticks.Left.X < 0)
                result |= OrbItProcs.Buttons.LeftThumbstickLeft;
            else if (sticks.Left.X > 0)
                result |= OrbItProcs.Buttons.LeftThumbstickRight;

            if (sticks.Left.Y < 0)
                result |= OrbItProcs.Buttons.LeftThumbstickDown;
            else if (sticks.Left.Y > 0)
                result |= OrbItProcs.Buttons.LeftThumbstickUp;

            if (sticks.Right.X < 0)
                result |= OrbItProcs.Buttons.RightThumbstickLeft;
            else if (sticks.Right.X > 0)
                result |= OrbItProcs.Buttons.RightThumbstickRight;

            if (sticks.Right.Y < 0)
                result |= OrbItProcs.Buttons.RightThumbstickDown;
            else if (sticks.Right.Y > 0)
                result |= OrbItProcs.Buttons.RightThumbstickUp;

            if (DPad.Down == ButtonState.Pressed)
                result |= OrbItProcs.Buttons.DPadDown;
            if (DPad.Up == ButtonState.Pressed)
                result |= OrbItProcs.Buttons.DPadUp;
            if (DPad.Left == ButtonState.Pressed)
                result |= OrbItProcs.Buttons.DPadLeft;
            if (DPad.Right == ButtonState.Pressed)
                result |= OrbItProcs.Buttons.DPadRight;

            return result;
        }

        //
        // Summary:
        //     Determines whether specified input device buttons are pressed in this GamePadState.
        //
        // Parameters:
        //   button:
        //     Buttons to query. Specify a single button, or combine multiple buttons using
        //     a bitwise OR operation.
        public bool IsButtonDown(Buttons button)
        {
            return (GetVirtualButtons() & button) == button;
        }
        //
        // Summary:
        //     Determines whether specified input device buttons are up (not pressed) in
        //     this GamePadState.
        //
        // Parameters:
        //   button:
        //     Buttons to query. Specify a single button, or combine multiple buttons using
        //     a bitwise OR operation.
        public bool IsButtonUp(Buttons button)
        {
            return (GetVirtualButtons() & button) != button;
        }

        //
        // Summary:
        //     Determines whether two GamePadState instances are not equal.
        //
        // Parameters:
        //   left:
        //     Object on the left of the equal sign.
        //
        //   right:
        //     Object on the right of the equal sign.
        public static bool operator !=(GamePadState left, GamePadState right)
        {
            return !left.Equals(right);
        }
        //
        // Summary:
        //     Determines whether two GamePadState instances are equal.
        //
        // Parameters:
        //   left:
        //     Object on the left of the equal sign.
        //
        //   right:
        //     Object on the right of the equal sign.
        public static bool operator ==(GamePadState left, GamePadState right)
        {
            return left.Equals(right);
        }
        //
        // Summary:
        //     Returns a value that indicates whether the current instance is equal to a
        //     specified object.
        //
        // Parameters:
        //   obj:
        //     Object with which to make the comparison.
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        //
        // Summary:
        //     Gets the hash code for this instance.
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        //
        // Summary:
        //     Retrieves a string representation of this object.
        public override string ToString()
        {
            return base.ToString();
        }
    }
    /// <summary>
    /// Contains commonly used precalculated values and mathematical operations.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Represents the mathematical constant e(2.71828175).
        /// </summary>
        public const float E = (float)Math.E;

        /// <summary>
        /// Represents the log base ten of e(0.4342945).
        /// </summary>
        public const float Log10E = 0.4342945f;

        /// <summary>
        /// Represents the log base two of e(1.442695).
        /// </summary>
        public const float Log2E = 1.442695f;

        /// <summary>
        /// Represents the value of pi(3.14159274).
        /// </summary>
        public const float Pi = (float)Math.PI;

        /// <summary>
        /// Represents the value of pi divided by two(1.57079637).
        /// </summary>
        public const float PiOver2 = (float)(Math.PI / 2.0);

        /// <summary>
        /// Represents the value of pi divided by four(0.7853982).
        /// </summary>
        public const float PiOver4 = (float)(Math.PI / 4.0);

        /// <summary>
        /// Represents the value of pi times two(6.28318548).
        /// </summary>
        public const float TwoPi = (float)(Math.PI * 2.0);

        /// <summary>
        /// Returns the Cartesian coordinate for one axis of a point that is defined by a given triangle and two normalized barycentric (areal) coordinates.
        /// </summary>
        /// <param name="value1">The coordinate on one axis of vertex 1 of the defining triangle.</param>
        /// <param name="value2">The coordinate on the same axis of vertex 2 of the defining triangle.</param>
        /// <param name="value3">The coordinate on the same axis of vertex 3 of the defining triangle.</param>
        /// <param name="amount1">The normalized barycentric (areal) coordinate b2, equal to the weighting factor for vertex 2, the coordinate of which is specified in value2.</param>
        /// <param name="amount2">The normalized barycentric (areal) coordinate b3, equal to the weighting factor for vertex 3, the coordinate of which is specified in value3.</param>
        /// <returns>Cartesian coordinate of the specified point with respect to the axis being used.</returns>
        public static float Barycentric(float value1, float value2, float value3, float amount1, float amount2)
        {
            return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param>
        /// <param name="value2">The second position in the interpolation.</param>
        /// <param name="value3">The third position in the interpolation.</param>
        /// <param name="value4">The fourth position in the interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>A position that is the result of the Catmull-Rom interpolation.</returns>
        public static float CatmullRom(float value1, float value2, float value3, float value4, float amount)
        {
            // Using formula from http://www.mvps.org/directx/articles/catmull/
            // Internally using doubles not to lose precission
            double amountSquared = amount * amount;
            double amountCubed = amountSquared * amount;
            return (float)(0.5 * (2.0 * value2 +
                (value3 - value1) * amount +
                (2.0 * value1 - 5.0 * value2 + 4.0 * value3 - value4) * amountSquared +
                (3.0 * value2 - value1 - 3.0 * value3 + value4) * amountCubed));
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static float Clamp(float value, float min, float max)
        {
            // First we check to see if we're greater than the max
            value = (value > max) ? max : value;

            // Then we check to see if we're less than the min.
            value = (value < min) ? min : value;

            // There's no check to see if min > max.
            return value;
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static int Clamp(int value, int min, int max)
        {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }

        /// <summary>
        /// Calculates the absolute value of the difference of two values.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <returns>Distance between the two values.</returns>
        public static float Distance(float value1, float value2)
        {
            return Math.Abs(value1 - value2);
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">Source position.</param>
        /// <param name="tangent1">Source tangent.</param>
        /// <param name="value2">Source position.</param>
        /// <param name="tangent2">Source tangent.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of the Hermite spline interpolation.</returns>
        public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            // All transformed to double not to lose precission
            // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
            double v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
            double sCubed = s * s * s;
            double sSquared = s * s;

            if (amount == 0f)
                result = value1;
            else if (amount == 1f)
                result = value2;
            else
                result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                    (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                    t1 * s +
                    v1;
            return (float)result;
        }


        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Destination value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns>Interpolated value.</returns> 
        /// <remarks>This method performs the linear interpolation based on the following formula:
        /// <code>value1 + (value2 - value1) * amount</code>.
        /// Passing amount a value of 0 will cause value1 to be returned, a value of 1 will cause value2 to be returned.
        /// See <see cref="MathHelper.LerpPrecise"/> for a less efficient version with more precision around edge cases.
        /// </remarks>
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }


        /// <summary>
        /// Linearly interpolates between two values.
        /// This method is a less efficient, more precise version of <see cref="MathHelper.Lerp"/>.
        /// See remarks for more info.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Destination value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns>Interpolated value.</returns>
        /// <remarks>This method performs the linear interpolation based on the following formula:
        /// <code>((1 - amount) * value1) + (value2 * amount)</code>.
        /// Passing amount a value of 0 will cause value1 to be returned, a value of 1 will cause value2 to be returned.
        /// This method does not have the floating point precision issue that <see cref="MathHelper.Lerp"/> has.
        /// i.e. If there is a big gap between value1 and value2 in magnitude (e.g. value1=10000000000000000, value2=1),
        /// right at the edge of the interpolation range (amount=1), <see cref="MathHelper.Lerp"/> will return 0 (whereas it should return 1).
        /// This also holds for value1=10^17, value2=10; value1=10^18,value2=10^2... so on.
        /// For an in depth explanation of the issue, see below references:
        /// Relevant Wikipedia Article: https://en.wikipedia.org/wiki/Linear_interpolation#Programming_language_support
        /// Relevant StackOverflow Answer: http://stackoverflow.com/questions/4353525/floating-point-linear-interpolation#answer-23716956
        /// </remarks>
        public static float LerpPrecise(float value1, float value2, float amount)
        {
            return ((1 - amount) * value1) + (value2 * amount);
        }

        /// <summary>
        /// Returns the greater of two values.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <returns>The greater value.</returns>
        public static float Max(float value1, float value2)
        {
            return value1 > value2 ? value1 : value2;
        }

        /// <summary>
        /// Returns the greater of two values.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <returns>The greater value.</returns>
        public static int Max(int value1, int value2)
        {
            return value1 > value2 ? value1 : value2;
        }

        /// <summary>
        /// Returns the lesser of two values.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <returns>The lesser value.</returns>
        public static float Min(float value1, float value2)
        {
            return value1 < value2 ? value1 : value2;
        }

        /// <summary>
        /// Returns the lesser of two values.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <returns>The lesser value.</returns>
        public static int Min(int value1, int value2)
        {
            return value1 < value2 ? value1 : value2;
        }

        /// <summary>
        /// Interpolates between two values using a cubic equation.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <param name="amount">Weighting value.</param>
        /// <returns>Interpolated value.</returns>
        public static float SmoothStep(float value1, float value2, float amount)
        {
            // It is expected that 0 < amount < 1
            // If amount < 0, return value1
            // If amount > 1, return value2
            float result = MathHelper.Clamp(amount, 0f, 1f);
            result = MathHelper.Hermite(value1, 0f, value2, 0f, result);

            return result;
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The angle in degrees.</returns>
        /// <remarks>
        /// This method uses double precission internally,
        /// though it returns single float
        /// Factor = 180 / pi
        /// </remarks>
        public static float ToDegrees(float radians)
        {
            return (float)(radians * 57.295779513082320876798154814105);
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        /// <remarks>
        /// This method uses double precission internally,
        /// though it returns single float
        /// Factor = pi / 180
        /// </remarks>
        public static float ToRadians(float degrees)
        {
            return (float)(degrees * 0.017453292519943295769236907684886);
        }

        /// <summary>
        /// Reduces a given angle to a value between π and -π.
        /// </summary>
        /// <param name="angle">The angle to reduce, in radians.</param>
        /// <returns>The new angle, in radians.</returns>
        public static float WrapAngle(float angle)
        {
            angle = (float)Math.IEEERemainder((double)angle, 6.2831854820251465);
            if (angle <= -3.14159274f)
            {
                angle += 6.28318548f;
            }
            else
            {
                if (angle > 3.14159274f)
                {
                    angle -= 6.28318548f;
                }
            }
            return angle;
        }

        /// <summary>
        /// Determines if value is powered by two.
        /// </summary>
        /// <param name="value">A value.</param>
        /// <returns><c>true</c> if <c>value</c> is powered by two; otherwise <c>false</c>.</returns>
        public static bool IsPowerOfTwo(int value)
        {
            return (value > 0) && ((value & (value - 1)) == 0);
        }
    }
    public struct GamePadButtons
    {
        internal Buttons buttons;

        public ButtonState A
        {
            get
            {
                return ((buttons & Buttons.A) == Buttons.A) ? ButtonState.Pressed : ButtonState.Released;
            }
        }
        public ButtonState B
        {
            get
            {
                return ((buttons & Buttons.B) == Buttons.B) ? ButtonState.Pressed : ButtonState.Released;
            }
        }
        public ButtonState Back
        {
            get
            {
                return ((buttons & Buttons.Back) == Buttons.Back) ? ButtonState.Pressed : ButtonState.Released;
            }
        }
        public ButtonState X
        {
            get
            {
                return ((buttons & Buttons.X) == Buttons.X) ? ButtonState.Pressed : ButtonState.Released;
            }
        }
        public ButtonState Y
        {
            get
            {
                return ((buttons & Buttons.Y) == Buttons.Y) ? ButtonState.Pressed : ButtonState.Released;
            }
        }
        public ButtonState Start
        {
            get
            {
                return ((buttons & Buttons.Start) == Buttons.Start) ? ButtonState.Pressed : ButtonState.Released;
            }
        }
        public ButtonState LeftShoulder
        {
            get
            {
                return ((buttons & Buttons.LeftShoulder) == Buttons.LeftShoulder) ? ButtonState.Pressed : ButtonState.Released;
            }
        }
        public ButtonState LeftStick
        {
            get
            {
                return ((buttons & Buttons.LeftStick) == Buttons.LeftStick) ? ButtonState.Pressed : ButtonState.Released;
            }
        }
        public ButtonState RightShoulder
        {
            get
            {
                return ((buttons & Buttons.RightShoulder) == Buttons.RightShoulder) ? ButtonState.Pressed : ButtonState.Released;
            }
        }
        public ButtonState RightStick
        {
            get
            {
                return ((buttons & Buttons.RightStick) == Buttons.RightStick) ? ButtonState.Pressed : ButtonState.Released;
            }
        }
        public ButtonState BigButton
        {
            get
            {
                return ((buttons & Buttons.BigButton) == Buttons.BigButton) ? ButtonState.Pressed : ButtonState.Released;
            }
        }

        public GamePadButtons(Buttons buttons)
        {
            this.buttons = buttons;
        }
        internal GamePadButtons(params Buttons[] buttons)
            : this()
        {
            foreach (Buttons b in buttons)
                this.buttons |= b;
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadButtons"/> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(GamePadButtons left, GamePadButtons right)
        {
            return left.buttons == right.buttons;
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadButtons"/> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(GamePadButtons left, GamePadButtons right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns>true if <paramref name="obj"/> is a <see cref="GamePadButtons"/> and has the same value as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is GamePadButtons) && (this == (GamePadButtons)obj);
        }

        public override int GetHashCode()
        {
            return (int)this.buttons;
        }
    }
    /// <summary>
    /// Defines the buttons on gamepad.
    /// </summary>
    [Flags]
    public enum Buttons
    {
        /// <summary>
        /// Directional pad up.
        /// </summary>
        DPadUp = 1,

        /// <summary>
        /// Directional pad down.
        /// </summary>
        DPadDown = 2,

        /// <summary>
        /// Directional pad left.
        /// </summary>
        DPadLeft = 4,

        /// <summary>
        /// Directional pad right.
        /// </summary>
        DPadRight = 8,

        /// <summary>
        /// START button.
        /// </summary>
        Start = 16,

        /// <summary>
        /// BACK button.
        /// </summary>
        Back = 32,

        /// <summary>
        /// Left stick button (pressing the left stick).
        /// </summary>
        LeftStick = 64,

        /// <summary>
        /// Right stick button (pressing the right stick).
        /// </summary>
        RightStick = 128,

        /// <summary>
        /// Left bumper (shoulder) button.
        /// </summary>
        LeftShoulder = 256,

        /// <summary>
        /// Right bumper (shoulder) button.
        /// </summary>
        RightShoulder = 512,

        /// <summary>
        /// Big button.
        /// </summary>    
        BigButton = 2048,

        /// <summary>
        /// A button.
        /// </summary>
        A = 4096,

        /// <summary>
        /// B button.
        /// </summary>
        B = 8192,

        /// <summary>
        /// X button.
        /// </summary>
        X = 16384,

        /// <summary>
        /// Y button.
        /// </summary>
        Y = 32768,

        /// <summary>
        /// Left stick is towards the left.
        /// </summary>
        LeftThumbstickLeft = 2097152,

        /// <summary>
        /// Right trigger.
        /// </summary>
        RightTrigger = 4194304,

        /// <summary>
        /// Left trigger.
        /// </summary>
        LeftTrigger = 8388608,

        /// <summary>
        /// Right stick is towards up.
        /// </summary>   
        RightThumbstickUp = 16777216,

        /// <summary>
        /// Right stick is towards down.
        /// </summary>   
        RightThumbstickDown = 33554432,

        /// <summary>
        /// Right stick is towards the right.
        /// </summary>
        RightThumbstickRight = 67108864,

        /// <summary>
        /// Right stick is towards the left.
        /// </summary>
        RightThumbstickLeft = 134217728,

        /// <summary>
        /// Left stick is towards up.
        /// </summary>  
        LeftThumbstickUp = 268435456,

        /// <summary>
        /// Left stick is towards down.
        /// </summary>  
        LeftThumbstickDown = 536870912,

        /// <summary>
        /// Left stick is towards the right.
        /// </summary>
        LeftThumbstickRight = 1073741824
    }
    public struct GamePadThumbSticks
    {
        Vector2 left;
        Vector2 right;

        public Vector2 Left
        {
            get
            {
                return left;
            }
        }
        public Vector2 Right
        {
            get
            {
                return right;
            }
        }

        public GamePadThumbSticks(Vector2 leftPosition, Vector2 rightPosition) : this()
        {
            left = leftPosition;
            right = rightPosition;
            ApplySquareClamp();
        }

        internal GamePadThumbSticks(Vector2 leftPosition, Vector2 rightPosition, GamePadDeadZone deadZoneMode) : this()
        {
            // XNA applies dead zones before rounding/clamping values. The public ctor does not allow this because the dead zone must be known before
            left = leftPosition;
            right = rightPosition;
            ApplyDeadZone(deadZoneMode);
            if (deadZoneMode == GamePadDeadZone.Circular)
                ApplyCircularClamp();
            else
                ApplySquareClamp();
        }

        private void ApplySquareClamp()
        {
            left = new Vector2(MathHelper.Clamp(left.X, -1f, 1f), MathHelper.Clamp(left.Y, -1f, 1f));
            right = new Vector2(MathHelper.Clamp(right.X, -1f, 1f), MathHelper.Clamp(right.Y, -1f, 1f));
        }

        private void ApplyCircularClamp()
        {
            if (left.LengthSquared() > 1f)
                left.Normalize();
            if (right.LengthSquared() > 1f)
                right.Normalize();
        }

        private void ApplyDeadZone(GamePadDeadZone dz)
        {
#if DIRECTX && !WINDOWS_PHONE && !WINDOWS_PHONE81 && !WINDOWS_UAP
            // XInput Xbox 360 Controller dead zones
            // Dead zones are slighty different between left and right sticks, this may come from Microsoft usability tests
            const float leftThumbDeadZone = SharpDX.XInput.Gamepad.LeftThumbDeadZone / (float)short.MaxValue;
            const float rightThumbDeadZone = SharpDX.XInput.Gamepad.RightThumbDeadZone / (float)short.MaxValue;
#elif OUYA
            // OUYA dead zones should
            // They are a bit larger to accomodate OUYA Gamepad (but will also affect Xbox 360 controllers plugged to an OUYA)
            const float leftThumbDeadZone = 0.3f;
            const float rightThumbDeadZone = 0.3f;
#else
            // Default & SDL Xbox 360 Controller dead zones
            // Based on the XInput constants
            const float leftThumbDeadZone = 0.24f;
            const float rightThumbDeadZone = 0.265f;
#endif
            switch (dz)
            {
                case GamePadDeadZone.None:
                    break;
                case GamePadDeadZone.IndependentAxes:
                    left = ExcludeIndependentAxesDeadZone(left, leftThumbDeadZone);
                    right = ExcludeIndependentAxesDeadZone(right, rightThumbDeadZone);
                    break;
                case GamePadDeadZone.Circular:
                    left = ExcludeCircularDeadZone(left, leftThumbDeadZone);
                    right = ExcludeCircularDeadZone(right, rightThumbDeadZone);
                    break;
            }
        }

        private Vector2 ExcludeIndependentAxesDeadZone(Vector2 value, float deadZone)
        {
            return new Vector2(ExcludeAxisDeadZone(value.X, deadZone), ExcludeAxisDeadZone(value.Y, deadZone));
        }

        private float ExcludeAxisDeadZone(float value, float deadZone)
        {
            if (value < -deadZone)
                value += deadZone;
            else if (value > deadZone)
                value -= deadZone;
            else
                return 0f;
            return value / (1f - deadZone);
        }

        private Vector2 ExcludeCircularDeadZone(Vector2 value, float deadZone)
        {
            var originalLength = value.Length();
            if (originalLength <= deadZone)
                return Vector2.Zero;
            var newLength = (originalLength - deadZone) / (1f - deadZone);
            return value * (newLength / originalLength);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadThumbSticks"/> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(GamePadThumbSticks left, GamePadThumbSticks right)
        {
            return (left.left == right.left)
                && (left.right == right.right);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadThumbSticks"/> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(GamePadThumbSticks left, GamePadThumbSticks right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns>true if <paramref name="obj"/> is a <see cref="GamePadThumbSticks"/> and has the same value as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is GamePadThumbSticks) && (this == (GamePadThumbSticks)obj);
        }

        public override int GetHashCode()
        {
            return this.Left.GetHashCode() + 37 * this.Right.GetHashCode();
        }
    }
    public struct GamePadTriggers
    {
        float left, right;

        public float Left
        {
            get { return left; }
            internal set { left = MathHelper.Clamp(value, 0f, 1f); }
        }
        public float Right
        {
            get { return right; }
            internal set { right = MathHelper.Clamp(value, 0f, 1f); }
        }

        public GamePadTriggers(float leftTrigger, float rightTrigger) : this()
        {
            Left = leftTrigger;
            Right = rightTrigger;
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadTriggers"/> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(GamePadTriggers left, GamePadTriggers right)
        {
            return (left.left == right.left)
                && (left.right == right.right);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadTriggers"/> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(GamePadTriggers left, GamePadTriggers right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns>true if <paramref name="obj"/> is a <see cref="GamePadTriggers"/> and has the same value as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is GamePadTriggers) && (this == (GamePadTriggers)obj);
        }

        public override int GetHashCode()
        {
            return this.Left.GetHashCode() + this.Right.GetHashCode();
        }
    }
    public struct GamePadDPad
    {
        public ButtonState Down
        {
            get;
            internal set;
        }
        public ButtonState Left
        {
            get;
            internal set;
        }
        public ButtonState Right
        {
            get;
            internal set;
        }
        public ButtonState Up
        {
            get;
            internal set;
        }

        public GamePadDPad(ButtonState upValue, ButtonState downValue, ButtonState leftValue, ButtonState rightValue)
            : this()
        {
            Up = upValue;
            Down = downValue;
            Left = leftValue;
            Right = rightValue;
        }

        internal GamePadDPad(params Buttons[] buttons)
            : this()
        {
            foreach (var b in buttons)
            {
                if ((b & Buttons.DPadDown) == Buttons.DPadDown)
                    Down = ButtonState.Pressed;
                if ((b & Buttons.DPadLeft) == Buttons.DPadLeft)
                    Left = ButtonState.Pressed;
                if ((b & Buttons.DPadRight) == Buttons.DPadRight)
                    Right = ButtonState.Pressed;
                if ((b & Buttons.DPadUp) == Buttons.DPadUp)
                    Up = ButtonState.Pressed;
            }
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadDPad"/> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(GamePadDPad left, GamePadDPad right)
        {
            return (left.Down == right.Down)
                && (left.Left == right.Left)
                && (left.Right == right.Right)
                && (left.Up == right.Up);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GamePadDPad"/> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(GamePadDPad left, GamePadDPad right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns>true if <paramref name="obj"/> is a <see cref="GamePadDPad"/> and has the same value as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is GamePadDPad) && (this == (GamePadDPad)obj);
        }

        public override int GetHashCode()
        {
            return
                (this.Down == ButtonState.Pressed ? 1 : 0) +
                (this.Left == ButtonState.Pressed ? 2 : 0) +
                (this.Right == ButtonState.Pressed ? 4 : 0) +
                (this.Up == ButtonState.Pressed ? 8 : 0);
        }
    }
    /// <summary>
    /// Holds the state of keystrokes by a keyboard.
    /// </summary>
	public struct KeyboardState
    {
        // Used for the common situation where GetPressedKeys will return an empty array
        static Keys[] empty = new Keys[0];

        #region Key Data

        // Array of 256 bits:
        uint keys0, keys1, keys2, keys3, keys4, keys5, keys6, keys7;

        bool InternalGetKey(Keys key)
        {
            uint mask = (uint)1 << (((int)key) & 0x1f);

            uint element;
            switch (((int)key) >> 5)
            {
                case 0: element = keys0; break;
                case 1: element = keys1; break;
                case 2: element = keys2; break;
                case 3: element = keys3; break;
                case 4: element = keys4; break;
                case 5: element = keys5; break;
                case 6: element = keys6; break;
                case 7: element = keys7; break;
                default: element = 0; break;
            }

            return (element & mask) != 0;
        }

        void InternalSetKey(Keys key)
        {
            uint mask = (uint)1 << (((int)key) & 0x1f);
            switch (((int)key) >> 5)
            {
                case 0: keys0 |= mask; break;
                case 1: keys1 |= mask; break;
                case 2: keys2 |= mask; break;
                case 3: keys3 |= mask; break;
                case 4: keys4 |= mask; break;
                case 5: keys5 |= mask; break;
                case 6: keys6 |= mask; break;
                case 7: keys7 |= mask; break;
            }
        }

        void InternalClearKey(Keys key)
        {
            uint mask = (uint)1 << (((int)key) & 0x1f);
            switch (((int)key) >> 5)
            {
                case 0: keys0 &= ~mask; break;
                case 1: keys1 &= ~mask; break;
                case 2: keys2 &= ~mask; break;
                case 3: keys3 &= ~mask; break;
                case 4: keys4 &= ~mask; break;
                case 5: keys5 &= ~mask; break;
                case 6: keys6 &= ~mask; break;
                case 7: keys7 &= ~mask; break;
            }
        }

        void InternalClearAllKeys()
        {
            keys0 = 0;
            keys1 = 0;
            keys2 = 0;
            keys3 = 0;
            keys4 = 0;
            keys5 = 0;
            keys6 = 0;
            keys7 = 0;
        }

        #endregion


        #region XNA Interface

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardState"/> class.
        /// </summary>
        /// <param name="keys">List of keys to be flagged as pressed on initialization.</param>
        internal KeyboardState(List<Keys> keys)
        {
            keys0 = 0;
            keys1 = 0;
            keys2 = 0;
            keys3 = 0;
            keys4 = 0;
            keys5 = 0;
            keys6 = 0;
            keys7 = 0;

            if (keys != null)
                foreach (Keys k in keys)
                    InternalSetKey(k);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardState"/> class.
        /// </summary>
        /// <param name="keys">List of keys to be flagged as pressed on initialization.</param>
        public KeyboardState(params Keys[] keys)
        {
            keys0 = 0;
            keys1 = 0;
            keys2 = 0;
            keys3 = 0;
            keys4 = 0;
            keys5 = 0;
            keys6 = 0;
            keys7 = 0;

            if (keys != null)
                foreach (Keys k in keys)
                    InternalSetKey(k);
        }

        /// <summary>
        /// Returns the state of a specified key.
        /// </summary>
        /// <param name="key">The key to query.</param>
        /// <returns>The state of the key.</returns>
        public KeyState this[Keys key]
        {
            get { return InternalGetKey(key) ? KeyState.Down : KeyState.Up; }
        }

        /// <summary>
        /// Gets whether given key is currently being pressed.
        /// </summary>
        /// <param name="key">The key to query.</param>
        /// <returns>true if the key is pressed; false otherwise.</returns>
        public bool IsKeyDown(Keys key)
        {
            return InternalGetKey(key);
        }

        /// <summary>
        /// Gets whether given key is currently being not pressed.
        /// </summary>
        /// <param name="key">The key to query.</param>
        /// <returns>true if the key is not pressed; false otherwise.</returns>
        public bool IsKeyUp(Keys key)
        {
            return !InternalGetKey(key);
        }

        #endregion


        #region GetPressedKeys()

        private static uint CountBits(uint v)
        {
            // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
            v = v - ((v >> 1) & 0x55555555);                    // reuse input as temporary
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333);     // temp
            return ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24; // count
        }

        private static int AddKeysToArray(uint keys, int offset, Keys[] pressedKeys, int index)
        {
            for (int i = 0; i < 32; i++)
            {
                if ((keys & (1 << i)) != 0)
                    pressedKeys[index++] = (Keys)(offset + i);
            }
            return index;
        }

        /// <summary>
        /// Returns an array of values holding keys that are currently being pressed.
        /// </summary>
        /// <returns>The keys that are currently being pressed.</returns>
        public Keys[] GetPressedKeys()
        {
            uint count = CountBits(keys0) + CountBits(keys1) + CountBits(keys2) + CountBits(keys3)
                    + CountBits(keys4) + CountBits(keys5) + CountBits(keys6) + CountBits(keys7);
            if (count == 0)
                return empty;
            Keys[] keys = new Keys[count];

            int index = 0;
            if (keys0 != 0) index = AddKeysToArray(keys0, 0 * 32, keys, index);
            if (keys1 != 0) index = AddKeysToArray(keys1, 1 * 32, keys, index);
            if (keys2 != 0) index = AddKeysToArray(keys2, 2 * 32, keys, index);
            if (keys3 != 0) index = AddKeysToArray(keys3, 3 * 32, keys, index);
            if (keys4 != 0) index = AddKeysToArray(keys4, 4 * 32, keys, index);
            if (keys5 != 0) index = AddKeysToArray(keys5, 5 * 32, keys, index);
            if (keys6 != 0) index = AddKeysToArray(keys6, 6 * 32, keys, index);
            if (keys7 != 0) index = AddKeysToArray(keys7, 7 * 32, keys, index);

            return keys;
        }

        #endregion


        #region Objet and Equality

        /// <summary>
        /// Gets the hash code for <see cref="KeyboardState"/> instance.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return (int)(keys0 ^ keys1 ^ keys2 ^ keys3 ^ keys4 ^ keys5 ^ keys6 ^ keys7);
        }

        /// <summary>
        /// Compares whether two <see cref="KeyboardState"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="KeyboardState"/> instance to the left of the equality operator.</param>
        /// <param name="b"><see cref="KeyboardState"/> instance to the right of the equality operator.</param>
        /// <returns>true if the instances are equal; false otherwise.</returns>
        public static bool operator ==(KeyboardState a, KeyboardState b)
        {
            return a.keys0 == b.keys0
                && a.keys1 == b.keys1
                && a.keys2 == b.keys2
                && a.keys3 == b.keys3
                && a.keys4 == b.keys4
                && a.keys5 == b.keys5
                && a.keys6 == b.keys6
                && a.keys7 == b.keys7;
        }

        /// <summary>
        /// Compares whether two <see cref="KeyboardState"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="KeyboardState"/> instance to the left of the inequality operator.</param>
        /// <param name="b"><see cref="KeyboardState"/> instance to the right of the inequality operator.</param>
        /// <returns>true if the instances are different; false otherwise.</returns>
        public static bool operator !=(KeyboardState a, KeyboardState b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Compares whether current instance is equal to specified object.
        /// </summary>
        /// <param name="obj">The <see cref="KeyboardState"/> to compare.</param>
        /// <returns>true if the provided <see cref="KeyboardState"/> instance is same with current; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is KeyboardState && this == (KeyboardState)obj;
        }

        #endregion

    }
    /// <summary>
    /// Defines the keys on a keyboard.
    /// </summary>	
	public enum Keys
    {
        /// <summary>
        /// Reserved.
        /// </summary>
		None = 0,
        /// <summary>
        /// BACKSPACE key.
        /// </summary>
		Back = 8,
        /// <summary>
        /// TAB key.
        /// </summary>
		Tab = 9,
        /// <summary>
        /// ENTER key.
        /// </summary>
		Enter = 13,
        /// <summary>
        /// CAPS LOCK key.
        /// </summary>
		CapsLock = 20,
        /// <summary>
        /// ESC key.
        /// </summary>
		Escape = 27,
        /// <summary>
        /// SPACEBAR key.
        /// </summary>
		Space = 32,
        /// <summary>
        /// PAGE UP key.
        /// </summary>
		PageUp = 33,
        /// <summary>
        /// PAGE DOWN key.
        /// </summary>
		PageDown = 34,
        /// <summary>
        /// END key.
        /// </summary>
		End = 35,
        /// <summary>
        /// HOME key.
        /// </summary>
		Home = 36,
        /// <summary>
        /// LEFT ARROW key.
        /// </summary>
		Left = 37,
        /// <summary>
        /// UP ARROW key.
        /// </summary>
		Up = 38,
        /// <summary>
        /// RIGHT ARROW key.
        /// </summary>
		Right = 39,
        /// <summary>
        /// DOWN ARROW key.
        /// </summary>
		Down = 40,
        /// <summary>
        /// SELECT key.
        /// </summary>
		Select = 41,
        /// <summary>
        /// PRINT key.
        /// </summary>
		Print = 42,
        /// <summary>
        /// EXECUTE key.
        /// </summary>
		Execute = 43,
        /// <summary>
        /// PRINT SCREEN key.
        /// </summary>
		PrintScreen = 44,
        /// <summary>
        /// INS key.
        /// </summary>
		Insert = 45,
        /// <summary>
        /// DEL key.
        /// </summary>
		Delete = 46,
        /// <summary>
        /// HELP key.
        /// </summary>
		Help = 47,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
		D0 = 48,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
		D1 = 49,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
		D2 = 50,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
		D3 = 51,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
		D4 = 52,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
		D5 = 53,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
		D6 = 54,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
		D7 = 55,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
		D8 = 56,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
		D9 = 57,
        /// <summary>
        /// A key.
        /// </summary>
		A = 65,
        /// <summary>
        /// B key.
        /// </summary>
		B = 66,
        /// <summary>
        /// C key.
        /// </summary>
		C = 67,
        /// <summary>
        /// D key.
        /// </summary>
		D = 68,
        /// <summary>
        /// E key.
        /// </summary>
		E = 69,
        /// <summary>
        /// F key.
        /// </summary>
		F = 70,
        /// <summary>
        /// G key.
        /// </summary>
		G = 71,
        /// <summary>
        /// H key.
        /// </summary>
		H = 72,
        /// <summary>
        /// I key.
        /// </summary>
		I = 73,
        /// <summary>
        /// J key.
        /// </summary>
		J = 74,
        /// <summary>
        /// K key.
        /// </summary>
		K = 75,
        /// <summary>
        /// L key.
        /// </summary>
		L = 76,
        /// <summary>
        /// M key.
        /// </summary>
		M = 77,
        /// <summary>
        /// N key.
        /// </summary>
		N = 78,
        /// <summary>
        /// O key.
        /// </summary>
		O = 79,
        /// <summary>
        /// P key.
        /// </summary>
		P = 80,
        /// <summary>
        /// Q key.
        /// </summary>
		Q = 81,
        /// <summary>
        /// R key.
        /// </summary>
		R = 82,
        /// <summary>
        /// S key.
        /// </summary>
		S = 83,
        /// <summary>
        /// T key.
        /// </summary>
		T = 84,
        /// <summary>
        /// U key.
        /// </summary>
		U = 85,
        /// <summary>
        /// V key.
        /// </summary>
		V = 86,
        /// <summary>
        /// W key.
        /// </summary>
		W = 87,
        /// <summary>
        /// X key.
        /// </summary>
		X = 88,
        /// <summary>
        /// Y key.
        /// </summary>
		Y = 89,
        /// <summary>
        /// Z key.
        /// </summary>
		Z = 90,
        /// <summary>
        /// Left Windows key.
        /// </summary>
		LeftWindows = 91,
        /// <summary>
        /// Right Windows key.
        /// </summary>
		RightWindows = 92,
        /// <summary>
        /// Applications key.
        /// </summary>
		Apps = 93,
        /// <summary>
        /// Computer Sleep key.
        /// </summary>
		Sleep = 95,
        /// <summary>
        /// Numeric keypad 0 key.
        /// </summary>
		NumPad0 = 96,
        /// <summary>
        /// Numeric keypad 1 key.
        /// </summary>
		NumPad1 = 97,
        /// <summary>
        /// Numeric keypad 2 key.
        /// </summary>
		NumPad2 = 98,
        /// <summary>
        /// Numeric keypad 3 key.
        /// </summary>
		NumPad3 = 99,
        /// <summary>
        /// Numeric keypad 4 key.
        /// </summary>
		NumPad4 = 100,
        /// <summary>
        /// Numeric keypad 5 key.
        /// </summary>
		NumPad5 = 101,
        /// <summary>
        /// Numeric keypad 6 key.
        /// </summary>
		NumPad6 = 102,
        /// <summary>
        /// Numeric keypad 7 key.
        /// </summary>
		NumPad7 = 103,
        /// <summary>
        /// Numeric keypad 8 key.
        /// </summary>
		NumPad8 = 104,
        /// <summary>
        /// Numeric keypad 9 key.
        /// </summary>
		NumPad9 = 105,
        /// <summary>
        /// Multiply key.
        /// </summary>
		Multiply = 106,
        /// <summary>
        /// Add key.
        /// </summary>
		Add = 107,
        /// <summary>
        /// Separator key.
        /// </summary>
		Separator = 108,
        /// <summary>
        /// Subtract key.
        /// </summary>
		Subtract = 109,
        /// <summary>
        /// Decimal key.
        /// </summary>
		Decimal = 110,
        /// <summary>
        /// Divide key.
        /// </summary>
		Divide = 111,
        /// <summary>
        /// F1 key.
        /// </summary>
		F1 = 112,
        /// <summary>
        /// F2 key.
        /// </summary>
		F2 = 113,
        /// <summary>
        /// F3 key.
        /// </summary>
		F3 = 114,
        /// <summary>
        /// F4 key.
        /// </summary>
		F4 = 115,
        /// <summary>
        /// F5 key.
        /// </summary>
		F5 = 116,
        /// <summary>
        /// F6 key.
        /// </summary>
		F6 = 117,
        /// <summary>
        /// F7 key.
        /// </summary>
		F7 = 118,
        /// <summary>
        /// F8 key.
        /// </summary>
		F8 = 119,
        /// <summary>
        /// F9 key.
        /// </summary>
		F9 = 120,
        /// <summary>
        /// F10 key.
        /// </summary>
		F10 = 121,
        /// <summary>
        /// F11 key.
        /// </summary>
		F11 = 122,
        /// <summary>
        /// F12 key.
        /// </summary>
		F12 = 123,
        /// <summary>
        /// F13 key.
        /// </summary>
		F13 = 124,
        /// <summary>
        /// F14 key.
        /// </summary>
		F14 = 125,
        /// <summary>
        /// F15 key.
        /// </summary>
		F15 = 126,
        /// <summary>
        /// F16 key.
        /// </summary>
		F16 = 127,
        /// <summary>
        /// F17 key.
        /// </summary>
		F17 = 128,
        /// <summary>
        /// F18 key.
        /// </summary>
		F18 = 129,
        /// <summary>
        /// F19 key.
        /// </summary>
		F19 = 130,
        /// <summary>
        /// F20 key.
        /// </summary>
		F20 = 131,
        /// <summary>
        /// F21 key.
        /// </summary>
		F21 = 132,
        /// <summary>
        /// F22 key.
        /// </summary>
		F22 = 133,
        /// <summary>
        /// F23 key.
        /// </summary>
		F23 = 134,
        /// <summary>
        /// F24 key.
        /// </summary>
		F24 = 135,
        /// <summary>
        /// NUM LOCK key.
        /// </summary>
		NumLock = 144,
        /// <summary>
        /// SCROLL LOCK key.
        /// </summary>
		Scroll = 145,
        /// <summary>
        /// Left SHIFT key.
        /// </summary>
		LeftShift = 160,
        /// <summary>
        /// Right SHIFT key.
        /// </summary>
		RightShift = 161,
        /// <summary>
        /// Left CONTROL key.
        /// </summary>
		LeftControl = 162,
        /// <summary>
        /// Right CONTROL key.
        /// </summary>
		RightControl = 163,
        /// <summary>
        /// Left ALT key.
        /// </summary>
		LeftAlt = 164,
        /// <summary>
        /// Right ALT key.
        /// </summary>
		RightAlt = 165,
        /// <summary>
        /// Browser Back key.
        /// </summary>
		BrowserBack = 166,
        /// <summary>
        /// Browser Forward key.
        /// </summary>
		BrowserForward = 167,
        /// <summary>
        /// Browser Refresh key.
        /// </summary>
		BrowserRefresh = 168,
        /// <summary>
        /// Browser Stop key.
        /// </summary>
		BrowserStop = 169,
        /// <summary>
        /// Browser Search key.
        /// </summary>
		BrowserSearch = 170,
        /// <summary>
        /// Browser Favorites key.
        /// </summary>
		BrowserFavorites = 171,
        /// <summary>
        /// Browser Start and Home key.
        /// </summary>
		BrowserHome = 172,
        /// <summary>
        /// Volume Mute key.
        /// </summary>
        VolumeMute = 173,
        /// <summary>
        /// Volume Down key.
        /// </summary>
		VolumeDown = 174,
        /// <summary>
        /// Volume Up key.
        /// </summary>
		VolumeUp = 175,
        /// <summary>
        /// Next Track key.
        /// </summary>
		MediaNextTrack = 176,
        /// <summary>
        /// Previous Track key.
        /// </summary>
		MediaPreviousTrack = 177,
        /// <summary>
        /// Stop Media key.
        /// </summary>
		MediaStop = 178,
        /// <summary>
        /// Play/Pause Media key.
        /// </summary>
		MediaPlayPause = 179,
        /// <summary>
        /// Start Mail key.
        /// </summary>
		LaunchMail = 180,
        /// <summary>
        /// Select Media key.
        /// </summary>
		SelectMedia = 181,
        /// <summary>
        /// Start Application 1 key.
        /// </summary>
		LaunchApplication1 = 182,
        /// <summary>
        /// Start Application 2 key.
        /// </summary>
		LaunchApplication2 = 183,
        /// <summary>
        /// The OEM Semicolon key on a US standard keyboard.
        /// </summary>
		OemSemicolon = 186,
        /// <summary>
        /// For any country/region, the '+' key.
        /// </summary>
		OemPlus = 187,
        /// <summary>
        /// For any country/region, the ',' key.
        /// </summary>
		OemComma = 188,
        /// <summary>
        /// For any country/region, the '-' key.
        /// </summary>
		OemMinus = 189,
        /// <summary>
        /// For any country/region, the '.' key.
        /// </summary>
		OemPeriod = 190,
        /// <summary>
        /// The OEM question mark key on a US standard keyboard.
        /// </summary>
		OemQuestion = 191,
        /// <summary>
        /// The OEM tilde key on a US standard keyboard.
        /// </summary>
		OemTilde = 192,
        /// <summary>
        /// The OEM open bracket key on a US standard keyboard.
        /// </summary>
		OemOpenBrackets = 219,
        /// <summary>
        /// The OEM pipe key on a US standard keyboard.
        /// </summary>
		OemPipe = 220,
        /// <summary>
        /// The OEM close bracket key on a US standard keyboard.
        /// </summary>
		OemCloseBrackets = 221,
        /// <summary>
        /// The OEM singled/double quote key on a US standard keyboard.
        /// </summary>
		OemQuotes = 222,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard.
        /// </summary>
		Oem8 = 223,
        /// <summary>
        /// The OEM angle bracket or backslash key on the RT 102 key keyboard.
        /// </summary>
		OemBackslash = 226,
        /// <summary>
        /// IME PROCESS key.
        /// </summary>
		ProcessKey = 229,
        /// <summary>
        /// Attn key.
        /// </summary>
		Attn = 246,
        /// <summary>
        /// CrSel key.
        /// </summary>
		Crsel = 247,
        /// <summary>
        /// ExSel key.
        /// </summary>
		Exsel = 248,
        /// <summary>
        /// Erase EOF key.
        /// </summary>
		EraseEof = 249,
        /// <summary>
        /// Play key.
        /// </summary>
		Play = 250,
        /// <summary>
        /// Zoom key.
        /// </summary>
		Zoom = 251,
        /// <summary>
        /// PA1 key.
        /// </summary>
		Pa1 = 253,
        /// <summary>
        /// CLEAR key.
        /// </summary>
		OemClear = 254,
        /// <summary>
        /// Green ChatPad key.
        /// </summary>
		ChatPadGreen = 0xCA,
        /// <summary>
        /// Orange ChatPad key.
        /// </summary>
		ChatPadOrange = 0xCB,
        /// <summary>
        /// PAUSE key.
        /// </summary>
		Pause = 0x13,
        /// <summary>
        /// IME Convert key.
        /// </summary>
		ImeConvert = 0x1c,
        /// <summary>
        /// IME NoConvert key.
        /// </summary>
		ImeNoConvert = 0x1d,
        /// <summary>
        /// Kana key on Japanese keyboards.
        /// </summary>
		Kana = 0x15,
        /// <summary>
        /// Kanji key on Japanese keyboards.
        /// </summary>
		Kanji = 0x19,
        /// <summary>
        /// OEM Auto key.
        /// </summary>
		OemAuto = 0xf3,
        /// <summary>
        /// OEM Copy key.
        /// </summary>
		OemCopy = 0xf2,
        /// <summary>
        /// OEM Enlarge Window key.
        /// </summary>
		OemEnlW = 0xf4
    }
    /// <summary>
	/// Identifies the state of a keyboard key.
	/// </summary>
	public enum KeyState
    {
        /// <summary>
        /// Key is released.
        /// </summary>
        Up,

        /// <summary>
        /// Key is pressed.
        /// </summary>
        Down,
    }
    /// <summary>
    /// Allows getting keystrokes from keyboard.
    /// </summary>
	public static class Keyboard
    {
        static List<Keys> _keys;

        /// <summary>
        /// Returns the current keyboard state.
        /// </summary>
        /// <returns>Current keyboard state.</returns>
		public static KeyboardState GetState()
        {
            return new KeyboardState(_keys);
        }

        /// <summary>
        /// Returns the current keyboard state for a given player.
        /// </summary>
        /// <param name="playerIndex">Player index of the keyboard.</param>
        /// <returns>Current keyboard state.</returns>
        [Obsolete("Use GetState() instead. In future versions this method can be removed.")]
        public static KeyboardState GetState(PlayerIndex playerIndex)
        {
            return new KeyboardState(_keys);
        }

        internal static void SetKeys(List<Keys> keys)
        {
            _keys = keys;
        }
    }

    /// <summary>
    /// Allows reading position and button click information from mouse.
    /// </summary>
    public static class Mouse
    {
        internal static GameWindow PrimaryWindow = null;

        private static readonly MouseState _defaultState = new MouseState();

#if DESKTOPGL || ANGLE

        static OpenTK.INativeWindow Window;

        internal static void setWindows(GameWindow window)
        {
            PrimaryWindow = window;
            if (window is OpenTKGameWindow)
            {
                Window = (window as OpenTKGameWindow).Window;
            }
        }

#elif (WINDOWS && DIRECTX)

        static System.Windows.Forms.Form Window;

        internal static void SetWindows(System.Windows.Forms.Form window)
        {
            Window = window;
        }

#elif MONOMAC
        internal static GameWindow Window;
        internal static float ScrollWheelValue;
#endif

        /// <summary>
        /// Gets or sets the window handle for current mouse processing.
        /// </summary> 
        public static IntPtr WindowHandle 
        { 
            get
            { 
#if DESKTOPGL || ANGLE
                return Window.WindowInfo.Handle;
#elif WINRT
                return IntPtr.Zero; // WinRT platform does not create traditionally window, so returns IntPtr.Zero.
#elif(WINDOWS && DIRECTX)
                return Window.Handle; 
#elif MONOMAC
                return IntPtr.Zero;
#else
                return IntPtr.Zero;
#endif
            }
            set
            {
                // only for XNA compatibility, yet
            }
        }

        #region Public methods

        /// <summary>
        /// This API is an extension to XNA.
        /// Gets mouse state information that includes position and button
        /// presses for the provided window
        /// </summary>
        /// <returns>Current state of the mouse.</returns>
        public static MouseState GetState(GameWindow window)
        {
#if MONOMAC
            //We need to maintain precision...
            window.MouseState.ScrollWheelValue = (int)ScrollWheelValue;

#elif DESKTOPGL || ANGLE

            var state = OpenTK.Input.Mouse.GetCursorState();
            var pc = ((OpenTKGameWindow)window).Window.PointToClient(new System.Drawing.Point(state.X, state.Y));
            window.MouseState.X = pc.X;
            window.MouseState.Y = pc.Y;

            window.MouseState.LeftButton = (ButtonState)state.LeftButton;
            window.MouseState.RightButton = (ButtonState)state.RightButton;
            window.MouseState.MiddleButton = (ButtonState)state.MiddleButton;
            window.MouseState.XButton1 = (ButtonState)state.XButton1;
            window.MouseState.XButton2 = (ButtonState)state.XButton2;

            // XNA uses the winapi convention of 1 click = 120 delta
            // OpenTK scales 1 click = 1.0 delta, so make that match
            window.MouseState.ScrollWheelValue = (int)(state.Scroll.Y * 120);
#endif



            return new MouseState();
            //TODO: SAVE ME
            //return window.MouseState;
        }

        /// <summary>
        /// Gets mouse state information that includes position and button presses
        /// for the primary window
        /// </summary>
        /// <returns>Current state of the mouse.</returns>
        public static MouseState GetState()
        {
#if ANDROID

            // Before MouseState was changed to take in a 
            // gamewindow, Android seemed to never update 
            // the previous static MouseState that existed.
            // This implies that the default behavior is to return
            // default(MouseState); A static one is used to prevent
            // constant reallocations
            // This will need to change when MonoGame supports desktop Android.
            // Related discussion: https://github.com/mono/MonoGame/pull/1749

            return _defaultState;
#else
            if (PrimaryWindow != null)
                return GetState(PrimaryWindow);

            return _defaultState;
#endif
        }

        /// <summary>
        /// Sets mouse cursor's relative position to game-window.
        /// </summary>
        /// <param name="x">Relative horizontal position of the cursor.</param>
        /// <param name="y">Relative vertical position of the cursor.</param>
        public static void SetPosition(int x, int y)
        {
            UpdateStatePosition(x, y);

#if (WINDOWS && DIRECTX) || DESKTOPGL || ANGLE
            // correcting the coordinate system
            // Only way to set the mouse position !!!
            var pt = Window.PointToScreen(new System.Drawing.Point(x, y));
#elif WINDOWS
            var pt = new System.Drawing.Point(0, 0);
#endif

#if DESKTOPGL || ANGLE
            OpenTK.Input.Mouse.SetPosition(pt.X, pt.Y);
#elif WINDOWS
            SetCursorPos(pt.X, pt.Y);
#elif MONOMAC
            var mousePt = NSEvent.CurrentMouseLocation;
            NSScreen currentScreen = null;
            foreach (var screen in NSScreen.Screens) {
                if (screen.Frame.Contains(mousePt)) {
                    currentScreen = screen;
                    break;
                }
            }

            var point = new PointF(x, Window.ClientBounds.Height-y);
            var windowPt = Window.ConvertPointToView(point, null);
            var screenPt = Window.Window.ConvertBaseToScreen(windowPt);
            var flippedPt = new PointF(screenPt.X, currentScreen.Frame.Size.Height-screenPt.Y);
            flippedPt.Y += currentScreen.Frame.Location.Y;
            
            
            CGSetLocalEventsSuppressionInterval(0.0);
            CGWarpMouseCursorPosition(flippedPt);
            CGSetLocalEventsSuppressionInterval(0.25);
#elif WEB
            PrimaryWindow.MouseState.X = x;
            PrimaryWindow.MouseState.Y = y;
#endif
        }

        #endregion Public methods
    
        private static void UpdateStatePosition(int x, int y)
        {
            //TODO: SAVE ME

            //PrimaryWindow.MouseState.X = x;
            //PrimaryWindow.MouseState.Y = y;
        }

#if WINDOWS

        [DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        internal static extern bool SetCursorPos(int X, int Y);

        /// <summary>
        /// Struct representing a point. 
        /// (Suggestion : Make another class for mouse extensions)
        /// </summary>
        [StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        internal struct POINT
        {
            public int X;
            public int Y;

            public System.Drawing.Point ToPoint()
            {
                return new System.Drawing.Point(X, Y);
            }

        }

#elif MONOMAC
#if PLATFORM_MACOS_LEGACY
        [DllImport (MonoMac.Constants.CoreGraphicsLibrary)]
        extern static void CGWarpMouseCursorPosition(PointF newCursorPosition);
        
        [DllImport (MonoMac.Constants.CoreGraphicsLibrary)]
        extern static void CGSetLocalEventsSuppressionInterval(double seconds);
#else
        [DllImport (ObjCRuntime.Constants.CoreGraphicsLibrary)]
        extern static void CGWarpMouseCursorPosition(CoreGraphics.CGPoint newCursorPosition);

        [DllImport (ObjCRuntime.Constants.CoreGraphicsLibrary)]
        extern static void CGSetLocalEventsSuppressionInterval(double seconds);
#endif
#endif

    }

    /// <summary>
    /// Represents a mouse state with cursor position and button press information.
    /// </summary>
	public struct MouseState
    {
        int _x, _y;
        int _scrollWheelValue;
        ButtonState _leftButton;
        ButtonState _rightButton;
        ButtonState _middleButton;
        ButtonState _xButton1;
        ButtonState _xButton2;

        /// <summary>
        /// Initializes a new instance of the MouseState.
        /// </summary>
        /// <param name="x">Horizontal position of the mouse.</param>
        /// <param name="y">Vertical position of the mouse.</param>
        /// <param name="scrollWheel">Mouse scroll wheel's value.</param>
        /// <param name="leftButton">Left mouse button's state.</param>
        /// <param name="middleButton">Middle mouse button's state.</param>
        /// <param name="rightButton">Right mouse button's state.</param>
        /// <param name="xButton1">XBUTTON1's state.</param>
        /// <param name="xButton2">XBUTTON2's state.</param>
        /// <remarks>Normally <see cref="Mouse.GetState()"/> should be used to get mouse current state. The constructor is provided for simulating mouse input.</remarks>
        public MouseState(
            int x,
            int y,
            int scrollWheel,
            ButtonState leftButton,
            ButtonState middleButton,
            ButtonState rightButton,
            ButtonState xButton1,
            ButtonState xButton2)
        {
            _x = x;
            _y = y;
            _scrollWheelValue = scrollWheel;
            _leftButton = leftButton;
            _middleButton = middleButton;
            _rightButton = rightButton;
            _xButton1 = xButton1;
            _xButton2 = xButton2;
        }

        /// <summary>
        /// Compares whether two MouseState instances are equal.
        /// </summary>
        /// <param name="left">MouseState instance on the left of the equal sign.</param>
        /// <param name="right">MouseState instance  on the right of the equal sign.</param>
        /// <returns>true if the instances are equal; false otherwise.</returns>
        public static bool operator ==(MouseState left, MouseState right)
        {
            return left._x == right._x &&
                   left._y == right._y &&
                   left._leftButton == right._leftButton &&
                   left._middleButton == right._middleButton &&
                   left._rightButton == right._rightButton &&
                   left._scrollWheelValue == right._scrollWheelValue;
        }

        /// <summary>
        /// Compares whether two MouseState instances are not equal.
        /// </summary>
        /// <param name="left">MouseState instance on the left of the equal sign.</param>
        /// <param name="right">MouseState instance  on the right of the equal sign.</param>
        /// <returns>true if the objects are not equal; false otherwise.</returns>
        public static bool operator !=(MouseState left, MouseState right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compares whether current instance is equal to specified object.
        /// </summary>
        /// <param name="obj">The MouseState to compare.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is MouseState)
                return this == (MouseState)obj;
            return false;
        }

        /// <summary>
        /// Gets the hash code for MouseState instance.
        /// </summary>
        /// <returns>Hash code of the object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets horizontal position of the cursor.
        /// </summary>
		public int X
        {
            get
            {
                return _x;
            }
            internal set
            {
                _x = value;
            }
        }

        /// <summary>
        /// Gets vertical position of the cursor.
        /// </summary>
		public int Y
        {
            get
            {
                return _y;
            }
            internal set
            {
                _y = value;
            }
        }

        /// <summary>
        /// Gets cursor position.
        /// </summary>
        public Point Position
        {
            get { return new Point(_x, _y); }
        }

        /// <summary>
        /// Gets state of the left mouse button.
        /// </summary>
		public ButtonState LeftButton
        {
            get
            {
                return _leftButton;
            }
            internal set { _leftButton = value; }
        }

        /// <summary>
        /// Gets state of the middle mouse button.
        /// </summary>
		public ButtonState MiddleButton
        {
            get
            {
                return _middleButton;
            }
            internal set { _middleButton = value; }
        }

        /// <summary>
        /// Gets state of the right mouse button.
        /// </summary>
		public ButtonState RightButton
        {
            get
            {
                return _rightButton;
            }
            internal set { _rightButton = value; }
        }

        /// <summary>
        /// Returns cumulative scroll wheel value since the game start.
        /// </summary>
		public int ScrollWheelValue
        {
            get
            {
                return _scrollWheelValue;
            }
            internal set { _scrollWheelValue = value; }
        }

        /// <summary>
        /// Gets state of the XButton1.
        /// </summary>
		public ButtonState XButton1
        {
            get
            {
                return _xButton1;
            }
            set
            {
                _xButton1 = value;
            }
        }

        /// <summary>
        /// Gets state of the XButton2.
        /// </summary>
		public ButtonState XButton2
        {
            get
            {
                return _xButton2;
            }
            set
            {
                _xButton2 = value;
            }
        }
    }
}
