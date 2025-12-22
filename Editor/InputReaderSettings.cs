using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace ShoelaceStudios.Input.Editor
{
    [System.Serializable]
    public class InputReaderSettings
    {
        public string Name;
        public bool Generate = true;
        public List<ActionContextSetting> Actions = new();

        public void PopulateActionOptions(InputActionMap actionMap)
        {
            Actions.Clear();

            foreach (var action in actionMap.actions)
            {
                Actions.Add(new ActionContextSetting
                {
                    Name = action.name,
                    ActionType = action.type,
                    ControlType = action.expectedControlType,
                    PassValue = PassValueType.Field
                });
            }
        }
    }
}