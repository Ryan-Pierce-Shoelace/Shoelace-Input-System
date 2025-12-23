using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Shoelace.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using ShoelaceStudios.Utilities.Editor.ScriptGeneration;
using Utilities.Editor;

namespace ShoelaceStudios.Input.Editor
{
    public class InputReaderCreatorEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset rootTree;
        [SerializeField] private VisualTreeAsset inputActionAsset;

        [SerializeField] private InputActionAsset targetInput;
        [SerializeField] private string projectNamespace;
        [SerializeField] private string filePathString;
        [SerializeField] private InputReaderSettings[] readerOptions;

        private VisualElement readersContainer;

        [MenuItem("Tools/Input Reader Creator")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<InputReaderCreatorEditor>();
            wnd.titleContent = new GUIContent("Input Reader Creator");
        }

        public void CreateGUI()
        {
            rootTree.CloneTree(rootVisualElement);

            BindHeader();
            BindReaderList();
            BindGenerate();
        }

        private void BindGenerate()
        {
            var button = rootVisualElement.Q<Button>("Generate");
            button.clicked += GenerateInputReaders;
        }

        private void BindHeader()
        {
            var inputField = rootVisualElement.Q<ObjectField>("InputAsset");
            var namespaceField = rootVisualElement.Q<TextField>("Namespace");
            var pathField = rootVisualElement.Q<TextField>("FilePath");

            inputField.value = targetInput;
            namespaceField.value = projectNamespace;
            pathField.value = filePathString;

            inputField.RegisterValueChangedCallback(evt =>
            {
                targetInput = evt.newValue as InputActionAsset;
                RefreshReaders();
            });

            namespaceField.RegisterValueChangedCallback(e => projectNamespace = e.newValue);
            pathField.RegisterValueChangedCallback(e => filePathString = e.newValue);
        }

        private void BindReaderList()
        {
            readersContainer = rootVisualElement.Q<ScrollView>("Readers");
        }

        private void RefreshReaders()
        {
            readersContainer.Clear();

            if (targetInput == null)
                return;

            readerOptions = new InputReaderSettings[targetInput.actionMaps.Count];

            for (int i = 0; i < targetInput.actionMaps.Count; i++)
            {
                var map = targetInput.actionMaps[i];

                var settings = new InputReaderSettings
                {
                    Name = map.name,
                    Generate = true
                };
                settings.PopulateActionOptions(map);

                readerOptions[i] = settings;

                readersContainer.Add(CreateReaderFoldout(settings));
            }
        }

        private VisualElement CreateReaderFoldout(InputReaderSettings settings)
        {
            var foldout = new Foldout
            {
                text = settings.Name,
                value = true
            };

            var generateToggle = new Toggle("Generate Reader")
            {
                value = settings.Generate
            };

            generateToggle.RegisterValueChangedCallback(e =>
                settings.Generate = e.newValue
            );

            foldout.Add(generateToggle);

            foreach (var action in settings.Actions)
            {
                foldout.Add(CreateActionFoldout(action));
            }

            return foldout;
        }

        private VisualElement CreateActionFoldout(ActionContextSetting action)
        {
            var foldout = new Foldout
            {
                text = $"{action.Name}  ({action.GeneratedCSharpType})",
                value = false
            };

            foldout.style.marginLeft = 12;
            
            // Pass Value selector
            var passValueField = new EnumField("Pass Value", action.PassValue);
            passValueField.RegisterValueChangedCallback(e =>
                action.PassValue = (PassValueType)e.newValue
            );

            foldout.Add(passValueField);

            foldout.Add(CreateEventSection(action));

            return foldout;
        }

        private VisualElement CreateEventSection(ActionContextSetting action)
        {
            var container = new VisualElement();
            container.style.marginLeft = 12;

            container.Add(new Label("Events"));

            foreach (InputEventType type in Enum.GetValues(typeof(InputEventType)))
            {
                container.Add(CreateEventRow(action, type));
            }

            return container;
        }

        private VisualElement CreateEventRow(ActionContextSetting action, InputEventType type)
        {
            var row = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    marginBottom = 2
                }
            };

            bool hasEvent = action.Events.Exists(e => e.EventType == type);

            var enableToggle = new Toggle(type.ToString())
            {
                value = hasEvent
            };

            var passValueToggle = new Toggle("Pass Value")
            {
                value = hasEvent && action.Events.Find(e => e.EventType == type)?.PassValue == true
            };

            enableToggle.RegisterValueChangedCallback(e =>
            {
                if (e.newValue)
                {
                    action.SetEvent(type, passValueToggle.value);
                }
                else
                {
                    action.Events.RemoveAll(ev => ev.EventType == type);
                }
            });

            passValueToggle.RegisterValueChangedCallback(e =>
            {
                if (enableToggle.value)
                    action.SetEvent(type, e.newValue);
            });

            row.Add(enableToggle);
            row.Add(passValueToggle);

            return row;
        }


        private void GenerateInputReaders()
        {
            if (targetInput == null)
            {
                Debug.LogError("No Input Action Map Selected");
                return;
            }

            ProjectSetup.Folders.Create(filePathString, "Readers", "Generated");

            /*
            var path = AssetDatabase.GetAssetPath(targetInput);

            var importer = AssetImporter.GetAtPath(path) as  InputActionAssetImporter;

            var so = new SerializedObject(targetInput);

            so.FindProperty("m_GenerateWrapperCode").boolValue = true;
            so.FindProperty("m_WrapperClassName").stringValue = "MyInputActions";
            so.FindProperty("m_WrapperNamespace").stringValue = "Game.Input";
            so.FindProperty("m_WrapperCodePath").stringValue = "Assets/Scripts/Input";

            so.ApplyModifiedProperties();
            */

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
                    .Interface($"{targetInput.name}.I{reader.Name}Actions")
                    .Using("Shoelace.InputSystem")
                    .Using("UnityEngine")
                    .Using("UnityEngine.InputSystem")
                    .Attribute(
                        $"CreateAssetMenu(fileName =\"{reader.Name}ReaderSO\", menuName = \"Shoelace Input/{reader.Name}\")")
                    .Region("Initialization", regionBuilder =>
                    {
                        regionBuilder.Field(targetInput.name, "input");
                        regionBuilder.Method("override void", "Initialize", Access.Public, m =>
                        {
                            m.If("input == null", b =>
                            {
                                b.Line($"input = new {targetInput.name}();");
                                b.Line($"input.{reader.Name}.SetCallbacks(this);");
                            });
                        });
                        regionBuilder.Method("override void", "Enable", Access.Public,
                            m => { m.Line($"input?.{reader.Name}.Enable();"); });
                        regionBuilder.Method("override void", "Disable", Access.Public,
                            m => { m.Line($"input?.{reader.Name}.Disable();"); });
                        regionBuilder.Method("override void", "Cleanup", Access.Public, m =>
                        {
                            m.Line($"input?.{reader.Name}.Disable();");
                            m.Line($"input?.Dispose();");
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

                                    if (action.Events.Count > 1)
                                    {


                                        m.Switch("context.phase", switchBuilder =>
                                        {
                                            foreach (InputEventSetting actionEvent in action.Events)
                                            {
                                                switchBuilder.Case("InputActionPhase." + actionEvent.EventType,
                                                    body =>
                                                    {
                                                        if (actionEvent.PassValue)
                                                        {
                                                            body.Line(
                                                                $"{actionEvent.EventName(action.Name)}?.Invoke(val);");
                                                        }
                                                        else
                                                        {
                                                            body.Line(
                                                                $"{actionEvent.EventName(action.Name)}?.Invoke();");
                                                        }
                                                    });
                                            }
                                        });
                                    }
                                });
                            });
                        }
                    }).Build();


            builder.WriteScript(filePathString, readerScript, "Generated");
        }
    }
}
#endif