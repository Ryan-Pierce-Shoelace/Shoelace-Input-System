using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Shoelace.InputSystem
{
	[CreateAssetMenu(fileName = "UI InputReader", menuName = "Input/UI Reader")]
	public class UIInputReaderCore : BaseInputReaderCore//, WizardInput.IUIActions
	{
		public event UnityAction SubmitEvent = delegate { };
		public event UnityAction CancelEvent = delegate { };
		public event UnityAction<Vector2> NavigateEvent = delegate { };
		public event UnityAction<Vector2> PointEvent = delegate { };
		public event UnityAction ClickEvent = delegate { };

		//private WizardInput input;
		/*
		public override void Initialize()
		{
			if (input == null)
			{
				input = new WizardInput();
				input.UI.SetCallbacks(this);
			}
		}

		public override void Enable()
		{
			input?.UI.Enable();
		}

		public override void Disable()
		{
			input?.UI.Disable();
		}

		public override void Cleanup()
		{
			input?.UI.Disable();
			input?.Dispose();
		}
		*/

		public void OnNavigate(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				Vector2 navigation = context.ReadValue<Vector2>();
				NavigateEvent?.Invoke(navigation);
			}
		}

		public void OnSubmit(InputAction.CallbackContext context)
		{
			if (context.performed)
				SubmitEvent?.Invoke();
		}

		public void OnCancel(InputAction.CallbackContext context)
		{
			if (context.performed)
				CancelEvent?.Invoke();
		}
		public void OnPoint(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				Vector2 point = context.ReadValue<Vector2>();
				PointEvent?.Invoke(point);
			}
		}
		public void OnClick(InputAction.CallbackContext context)
		{
			if (context.performed)
				ClickEvent?.Invoke();
			
		}

		public void OnScrollWheel(InputAction.CallbackContext context)
		{
		}

		public void OnMiddleClick(InputAction.CallbackContext context)
		{
		}

		public void OnRightClick(InputAction.CallbackContext context)
		{
		}

		public override void Initialize()
		{
			
		}

		public override void Enable()
		{
			
		}

		public override void Disable()
		{
			
		}

		public override void Cleanup()
		{
			
		}
	}
}