using UnityEngine;

namespace Shoelace.InputSystem
{
	public class InputManager : MonoBehaviour
	{
		/*[Header("Input Readers")]
		[SerializeField] private UIInputReader uiInputReader;
		[SerializeField] private OnFootInputReader onFootInputReader;
        
		private BaseInputReader currentInputReader;
        
		public UIInputReader UIInput => uiInputReader;
		public OnFootInputReader OnFootInput => onFootInputReader;
        
		public void Initialize()
		{
			uiInputReader.Initialize();
			onFootInputReader.Initialize();
            
			SwitchToUI();
		}
        
		public void SwitchToUI()
		{
			currentInputReader?.SetActive(false);
			currentInputReader = uiInputReader;
			currentInputReader.SetActive(true);
		}

		public void SwitchToVehicle()
		{
			
		}
        
		public void SwitchToGameplay()
		{
			currentInputReader?.SetActive(false);
			currentInputReader = onFootInputReader;
			currentInputReader.SetActive(true);
		}
        
		private void OnDestroy()
		{
			currentInputReader?.SetActive(false);
		}*/
	}
}