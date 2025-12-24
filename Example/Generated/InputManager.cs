using UnityEngine;
using Shoelace.InputSystem;
namespace ShoelaceStudios.Input.Example
{
    public class InputManager : MonoBehaviour
    {
        #region Input Readers
            // Fields
            [SerializeField] private UIReaderSO UIReader;
            [SerializeField] private PlayerGameplayReaderSO PlayerGameplayReader;
            private BaseInputReader currentInputReader;
            // Properties
            public UIReaderSO UI => UIReader;
            public PlayerGameplayReaderSO PLAYERGAMEPLAY => PlayerGameplayReader;
        #endregion
        #region Initialization
            // Methods
            public void Initialize ()
            {
                UIReader.Initialize();
                PlayerGameplayReader.Initialize();
            }
            private void OnDestroy ()
            {
                currentInputReader?.SetActive(false);
            }
        #endregion
        #region Input Switch
            public enum ActionMapType {UI, PLAYERGAMEPLAY}
            public void SwitchInput (ActionMapType setActionMap)
            {
                switch (setActionMap)
                {
                    // Switch Input to Desired Action Map
                    case ActionMapType.UI:
                currentInputReader?.SetActive(false);
                currentInputReader = UIReader;
                currentInputReader?.SetActive(true);
                        break;
                    // Switch Input to Desired Action Map
                    case ActionMapType.PLAYERGAMEPLAY:
                currentInputReader?.SetActive(false);
                currentInputReader = PlayerGameplayReader;
                currentInputReader?.SetActive(true);
                        break;
                }
            }
        #endregion
        }
}
