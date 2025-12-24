
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace ShoelaceStudios.Input.Editor
{
    [System.Serializable]
    public class ActionContextSetting
    {
        private string actionName;

        public string Name
        {
            get => actionName;
            set => actionName = value.Replace(" ", string.Empty);
        }
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
}
