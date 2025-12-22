namespace ShoelaceStudios.Input.Editor
{
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
}