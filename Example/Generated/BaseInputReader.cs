using UnityEngine;
using UnityEngine.InputSystem;
namespace Shoelace.InputSystem
{
    public abstract class BaseInputReader : BaseInputReaderCore
    {
        #region Control Schemes
            public enum ControlScheme {Gamepad, MouseAndKeyboard, Joystick}
            public ControlScheme CurrentScheme { get; private set; }
            protected void DetectControlScheme (InputAction.CallbackContext context)
            {
                // 
                if (context.control.device is Gamepad)
                {
                // 
                if (CurrentScheme != ControlScheme.Gamepad)
                {
                CurrentScheme = ControlScheme.Gamepad;
                }
                }
                // 
                if (context.control.device is Keyboard or Mouse)
                {
                // 
                if (CurrentScheme != ControlScheme.MouseAndKeyboard)
                {
                CurrentScheme = ControlScheme.MouseAndKeyboard;
                }
                }
                // 
                if (context.control.device is Joystick)
                {
                // 
                if (CurrentScheme != ControlScheme.Joystick)
                {
                CurrentScheme = ControlScheme.Joystick;
                }
                }
            }
        #endregion
        }
}
