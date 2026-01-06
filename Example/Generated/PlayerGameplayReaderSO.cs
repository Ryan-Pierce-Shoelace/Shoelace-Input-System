using Shoelace.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Shoelace.Input.Example
{
    [CreateAssetMenu(fileName ="PlayerGameplayReaderSO", menuName = "Shoelace Input/PlayerGameplay")]
    public class PlayerGameplayReaderSO : BaseInputReader, TestInput.IPlayerGameplayActions
    {
        #region Initialization
            private TestInput input;
            // Methods
            public override void Initialize ()
            {
                // 
                if (input == null)
                {
                input = new TestInput();
                input.PlayerGameplay.SetCallbacks(this);
                }
            }
            public override void Enable ()
            {
                input?.PlayerGameplay.Enable();
            }
            public override void Disable ()
            {
                input?.PlayerGameplay.Disable();
            }
            public override void Cleanup ()
            {
                input?.PlayerGameplay.Disable();
                input?.Dispose();
            }
        #endregion
        #region Inputs
            // Fields
            public bool Punch;
            public Vector2 Movement;
            public Quaternion NEWACTION;
        #endregion
        #region Reader Actions
            // Methods
            public void OnPunch (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                bool val = context.ReadValue<bool>();
                Punch = val;
                }
            }
            public void OnMovement (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                Vector2 val = context.ReadValue<Vector2>();
                Movement = val;
                }
            }
            public void OnNEWACTION (InputAction.CallbackContext context)
            {
                // 
                if (context.performed)
                {
                Quaternion val = context.ReadValue<Quaternion>();
                NEWACTION = val;
                }
            }
        #endregion
        }
}
