
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace ShoelaceStudios.Input.Editor
{
    public enum InputEventType
    {
        Started,
        Waiting,
        Disabled,
        Canceled
    }

    public enum PassValueType
    {
        None,
        Field,
        Property
    }
    
    
    [System.Serializable]
    public class ActionContextSetting
    {
        public string Name;
        public InputActionType ActionType;
        public string ControlType;

        public PassValueType PassValue;

        // List of event configs
        public List<InputEventSetting> Events = new();

        // Optional: contextual helpers
        public string GeneratedCSharpType => ResolveCSharpType();
        private string ResolveCSharpType()
        {
            if (ActionType == InputActionType.Button) return "bool";
            return ControlType switch
            {
                "Vector2" => "Vector2",
                "Vector3" => "Vector3",
                "Quaternion" => "Quaternion",
                _ => "float"
            };
        }
        // Convenience method for adding/updating event
        public void SetEvent(InputEventType eventType, bool passValue)
        {
            var existing = Events.Find(e => e.EventType == eventType);
            if (existing != null)
            {
                existing.PassValue = passValue;
            }
            else
            {
                Events.Add(new InputEventSetting(eventType, passValue));
            }
        }
    }

    
    [System.Serializable]
    public class InputEventSetting
    {
        public InputEventType EventType;
        public bool PassValue;
        public InputEventSetting(InputEventType eventType, bool passValue = false)
        {
            EventType = eventType;
            PassValue = passValue;
        }

        public string EventName(string actionName) => $"On{EventType}{actionName}";
    }
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
