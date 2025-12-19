#if UNITY_EDITOR
namespace ShoelaceStudios.Input.Editor
{
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.UIElements;
    using Shoelace.InputSystem;
    using ShoelaceStudios.Utilities.Editor.ScriptGeneration;
    using ShoelaceStudios.Utilities;
    public class InputReaderCreatorEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset tree = default;
        private InputActionAsset targetInput;
        private string projectNamespace;
        private string filePath;

        private InputReaderSettings[] readerOptions;

        [MenuItem("Tools/Input Reader Creator")]
        public static void ShowExample()
        {
            InputReaderCreatorEditor wnd = GetWindow<InputReaderCreatorEditor>();
            wnd.titleContent = new GUIContent("Input Reader Creator");
        }

        public void CreateGUI()
        {
            tree.CloneTree(rootVisualElement);

            var input = rootVisualElement.Q<ObjectField>("TargetInput");
            input.RegisterValueChangedCallback(evt =>
            {
                targetInput = evt.newValue as InputActionAsset;
                RefreshEditor();
            });
            
            rootVisualElement.Q<TextField>("FilePath").RegisterValueChangedCallback(evt =>
            {
                filePath = evt.newValue;
                //Check for saved values
                RefreshEditor();
            });
            projectNamespace = rootVisualElement.Q<TextField>("NameSpace").text;

            rootVisualElement.Q<Button>("Generate").clicked += GenerateInputReaders;
        }

        private void OnDisable()
        {
            rootVisualElement.Q<Button>("Generate").clicked -= GenerateInputReaders;
        }

        private void RefreshEditor()
        {
            readerOptions = new InputReaderSettings[targetInput.actionMaps.Count];
            for (int i = 0; i < readerOptions.Length; i++)
            {
                readerOptions[i] = new InputReaderSettings
                {
                    Name = targetInput.actionMaps[i].name,
                    Generate = true
                };
                readerOptions[i].PopulateActionOptions(targetInput.actionMaps[i]);
            }
        }

        private void GenerateInputReaders()
        {
            if (targetInput == null)
            {
                Debug.LogError("No Input Action Map Selected");
                return;
            }

            ProjectSetup.Folders.Create(filePath, "Readers", "Generated");
            
            var so = new SerializedObject(targetInput);

            so.FindProperty("m_GenerateWrapperCode").boolValue = true;
            so.FindProperty("m_WrapperClassName").stringValue = "MyInputActions";
            so.FindProperty("m_WrapperNamespace").stringValue = "Game.Input";
            so.FindProperty("m_WrapperCodePath").stringValue = "Assets/Scripts/Input";

            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(targetInput);
            AssetDatabase.SaveAssets();
            foreach (InputReaderSettings reader in readerOptions)
            {
                GenerateReader(reader);
            }
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
        }

        private void GenerateReader(InputReaderSettings reader)
        {
            ScriptBuilder builder = new ScriptBuilder();
            
            ProceduralScriptSpec readerScript =
                builder.NameSpace(projectNamespace).Class(reader.Name + "ReaderSO", nameof(BaseInputReader))
                    .Interface($"{targetInput.name}I{reader.Name}Actions")
                    .Region("Initialization", regionBuilder =>
                    {
                        regionBuilder.Field(targetInput.name, "input");
                        regionBuilder.Method("override void", "Initialize", Access.Public, m =>
                        {
                            m.If("input == null", b =>
                            {
                                b.Line($"input = new {targetInput.name}();");
                                b.Line($"input.{reader.Name}.SetCallback(this);");
                            });
                        });
                        regionBuilder.Method("override void", "Enable", Access.Public,
                            m => { m.Line($"input?.{targetInput.name}.Enable()"); });
                        regionBuilder.Method("override void", "Disable", Access.Public,
                            m => { m.Line($"input?.{targetInput.name}.Disable()"); });
                        regionBuilder.Method("override void", "Cleanup", Access.Public, m =>
                        {
                            m.Line($"input?.{targetInput.name}.Disable()");
                            m.Line($"input?.Dispose()");
                        });
                    })
                    .Region("Inputs", regionBuilder =>
                    {
                        foreach (ActionContextSetting action in reader.Actions)
                        {
                            string type = action.GeneratedCSharpType;
                            switch (action.PassValue)
                            {
                                case PassValueType.Field:
                                    regionBuilder.Field(type, action.Name, Access.Public);
                                    break;
                                case PassValueType.Property:
                                    regionBuilder.Property(type, action.Name);
                                    break;
                                case PassValueType.None:
                                default: //noop
                                    break;
                            }

                            foreach (InputEventSetting actionEvent in action.Events)
                            {
                                if (actionEvent.PassValue)
                                {
                                    regionBuilder.UnityEvent(actionEvent.EventName(action.Name));
                                }
                                else
                                {
                                    regionBuilder.UnityEvent(actionEvent.EventName(action.Name), type);
                                }
                            }
                        }
                    })
                    .Region("Reader Actions", regionBuilder =>
                    {
                        foreach (ActionContextSetting action in reader.Actions)
                        {
                            string type = action.GeneratedCSharpType;

                            regionBuilder.Method("void", $"On{action.Name}", Access.Public, actionMethod =>
                            {
                                actionMethod.Param("InputAction.CallbackContext", "context");


                                actionMethod.If("context.performed", m =>
                                {
                                    m.Line($"{type} val = context.ReadValue<{type}>();");

                                    switch (action.PassValue)
                                    {
                                        case PassValueType.Property:
                                        case PassValueType.Field:
                                            m.Line($"{action.Name} = val;");
                                            break;
                                        case PassValueType.None:
                                        default: //noop
                                            break;
                                    }

                                    foreach (InputEventSetting actionEvent in action.Events)
                                    {
                                        m.Switch("context.phase", switchBuilder =>
                                        {
                                            switchBuilder.Case("InputActionPhase" + actionEvent.EventType,
                                                body =>
                                                {
                                                    if (actionEvent.PassValue)
                                                    {
                                                        body.Line(
                                                            $"{actionEvent.EventName(action.Name)}?.Invoke(val);");
                                                    }
                                                    else
                                                    {
                                                        body.Line($"{actionEvent.EventName(action.Name)}?.Invoke();");
                                                    }
                                                });
                                        });
                                    }
                                });
                            });
                        }
                    }).
                    Build();
            
            
            builder.WriteScript(filePath, readerScript, "Generated");
        }
    }
}
#endif