using UnityEngine;
using UnityEngine.InputSystem;

namespace Shoelace.InputSystem
{
	public abstract class BaseInputReaderCore : ScriptableObject
	{
		[SerializeField] protected InputActionAsset inputActionAsset;
		[SerializeField] protected string actionMapName;

		protected InputActionMap actionMap;
		protected bool isActive;
		protected int playerIndex = 0;

		public abstract void Initialize();
		public abstract void Enable();
		public abstract void Disable();
		public abstract void Cleanup();
		

		protected virtual void OnEnable()
		{
			if (inputActionAsset != null)
			{
				actionMap = inputActionAsset.FindActionMap(actionMapName);
			}
		}
        
		public virtual void SetPlayerIndex(int index)
		{
			playerIndex = index;
		}
        
		public virtual void SetActive(bool active)
		{
			if (active && !isActive)
			{
				Enable();
				isActive = true;
			}
			else if (!active && isActive)
			{
				Disable();
				isActive = false;
			}
		}
		
		/*public ControlScheme CurrentScheme { get; private set; }
		protected void DetectControlScheme(InputAction.CallbackContext context)
		{
			if (context.control.device is Gamepad)
			{
				if (CurrentScheme != ControlScheme.Gamepad)
				{
					CurrentScheme = ControlScheme.Gamepad;
				}
			}
			else if (context.control.device is Mouse or Keyboard)
			{
				if (CurrentScheme != ControlScheme.KeyboardMouse)
				{
					CurrentScheme = ControlScheme.KeyboardMouse;
				}
			}
		}
		public enum ControlScheme
		{
			KeyboardMouse,
			Gamepad
		}*/
	}
}