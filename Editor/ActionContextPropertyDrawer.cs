using UnityEditor;
using UnityEngine.UIElements;

namespace ShoelaceStudios.Input.Editor
{
    [CustomPropertyDrawer(typeof(ActionContextSetting))]
    public class ActionContextPropertyDrawer : PropertyDrawer
    {
        public VisualTreeAsset visualTreeAsset;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = visualTreeAsset.CloneTree();
            return root;
        }
    }
}