using Shoelace.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
namespace ShoelaceStudios.Input.Example
{
    [CreateAssetMenu(fileName ="UIReaderSO", menuName = "Shoelace Input/UI")]
    public class UIReaderSO : BaseInputReader, TestInput.IUIActions
    {
        #region Initialization
            private TestInput input;
            // Methods

            /// 
            public override void Initialize ()
            {
                // 
                if (input == null)
                {
                input = new TestInput();
                input.UI.SetCallbacks(this);
                }
            }

            /// 
            public override void Enable ()
            {
                input?.UI.Enable();
            }

            /// 
            public override void Disable ()
            {
                input?.UI.Disable();
            }

            /// 
            public override void Cleanup ()
            {
                input?.UI.Disable();
                input?.Dispose();
            }
        #endregion
        #region Inputs
            // Fields
            public Vector2 Navigate;
            public bool Submit;
            public bool Cancel;
            public Vector2 Point;
            public float Click;
            public Vector2 ScrollWheel;
            public float MiddleClick;
            public float RightClick;
            public Vector3 TrackedDevicePosition;
            public Quaternion TrackedDeviceOrientation;
        #endregion
        #region Reader Actions
            // Methods

            /// 
            public void OnNavigate (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                Vector2 val = context.ReadValue<Vector2>();
                Navigate = val;
                }
            }

            /// 
            public void OnSubmit (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                bool val = context.ReadValue<bool>();
                Submit = val;
                }
            }

            /// 
            public void OnCancel (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                bool val = context.ReadValue<bool>();
                Cancel = val;
                }
            }

            /// 
            public void OnPoint (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                Vector2 val = context.ReadValue<Vector2>();
                Point = val;
                }
            }

            /// 
            public void OnClick (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                float val = context.ReadValue<float>();
                Click = val;
                }
            }

            /// 
            public void OnScrollWheel (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                Vector2 val = context.ReadValue<Vector2>();
                ScrollWheel = val;
                }
            }

            /// 
            public void OnMiddleClick (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                float val = context.ReadValue<float>();
                MiddleClick = val;
                }
            }

            /// 
            public void OnRightClick (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                float val = context.ReadValue<float>();
                RightClick = val;
                }
            }

            /// 
            public void OnTrackedDevicePosition (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                Vector3 val = context.ReadValue<Vector3>();
                TrackedDevicePosition = val;
                }
            }

            /// 
            public void OnTrackedDeviceOrientation (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                Quaternion val = context.ReadValue<Quaternion>();
                TrackedDeviceOrientation = val;
                }
            }
        #endregion
    }
}
