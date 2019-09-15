using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Watermelon
{
    public static class EditorGUILayoutCustom
    {
        public static string FileField(GUIContent content, string value, string directory = "", string extension = "")
        {
            string tempValue = value;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(content);
            EditorGUILayout.LabelField(new GUIContent(value, value), GUILayout.MaxWidth(40));
            if (GUILayout.Button("•", EditorStyles.miniButton, GUILayout.Width(14)))
            {
                tempValue = EditorUtility.OpenFilePanel("Select file path", directory, extension);
            }
            EditorGUILayout.EndHorizontal();

            return tempValue;
        }

        public static string FolderField(GUIContent content, string value, string folder = "", string defaultName = "")
        {
            string tempValue = value;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(content);
            EditorGUILayout.LabelField(new GUIContent(value, value), GUILayout.MaxWidth(40));
            if (GUILayout.Button("•", EditorStyles.miniButton, GUILayout.Width(14)))
            {
                tempValue = EditorUtility.OpenFolderPanel("Select folder path", folder, defaultName);
            }
            EditorGUILayout.EndHorizontal();

            return tempValue;
        }

        public static bool ChangedToggle(ref bool variable, GUIContent content)
        {
            bool value = EditorGUILayout.Toggle(content, variable);
            if (value != variable)
            {
                variable = value;

                return true;
            }

            return false;
        }

        public static bool ChangedFoldout(ref bool variable, GUIContent content)
        {
            bool value = EditorGUILayout.Foldout(variable, content, true);
            if (value != variable)
            {
                variable = value;

                return true;
            }

            return false;
        }

        public static Type TypeField(string content, Type type, Type assemblyType = null)
        {
            if (assemblyType == null)
                assemblyType = typeof(MonoBehaviour);

            Assembly assembly = Assembly.GetAssembly(assemblyType);
            Type[] types = assembly.GetTypes();
            string[] variableNames = types.Select(x => x.Name).ToArray();

            int selectedItem = Array.FindIndex(variableNames, x => x == type.Name);

            selectedItem = EditorGUILayout.Popup(content, selectedItem, variableNames);

            return types[selectedItem];
        }

        public static string FieldNameLayout(Type type, ref int popupIndex, string label)
        {
            string[] variableNames = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(x => x.Name).ToArray();

            popupIndex = EditorGUILayout.Popup(label, popupIndex, variableNames);

            return variableNames[popupIndex];
        }

        public static string FieldName(Rect rect, Type type, ref int popupIndex, string label)
        {
            string[] variableNames = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(x => x.Name).ToArray();

            popupIndex = EditorGUI.Popup(rect, label, popupIndex, variableNames);

            return variableNames[popupIndex];
        }

        public static void ShowList(SerializedProperty list, Action<SerializedProperty> action)
        {
            if (!list.isArray)
            {
                EditorGUILayout.HelpBox(list.name + " is neither an array nor a list!", MessageType.Error);
                return;
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));

            for (int i = 0; i < list.arraySize; i++)
            {
                action(list.GetArrayElementAtIndex(i));
            }

            if (GUILayout.Button("Add", EditorStyles.miniButton))
            {
                list.arraySize += 1;
            }
        }

        public static object UniversalField(object value, Type type, string title = "")
        {
            if (type == typeof(string))
            {
                return EditorGUILayout.TextField(new GUIContent(title), (string)value);
            }
            else if (type == typeof(bool))
            {
                return EditorGUILayout.Toggle(new GUIContent(title), (bool)value);
            }
            else if (type == typeof(int))
            {
                return EditorGUILayout.IntField(new GUIContent(title), (int)value);
            }
            else if (type == typeof(float))
            {
                return EditorGUILayout.FloatField(new GUIContent(title), (System.Single)value);
            }
            else if (type == typeof(Type))
            {
                return EditorGUILayoutCustom.TypeField(title, (Type)value);
            }
            else if (type == typeof(Vector2))
            {
                return EditorGUILayout.Vector2Field(new GUIContent(title), (Vector2)value);
            }
            else if (type == typeof(Vector3))
            {
                return EditorGUILayout.Vector3Field(new GUIContent(title), (Vector3)value);
            }
            else if (type.IsEnum)
            {
                return EditorGUILayout.EnumPopup(new GUIContent(title), (Enum)value);
            }
            else if (type.IsSubclassOf(typeof(Object)))
            {
                return EditorGUILayout.ObjectField(new GUIContent(title), (Object)value, type, true);
            }
            else if (type.IsSerializable && !type.IsArray && !type.IsGenericType && type != typeof(object))
            {
                foreach (var property in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(x => x.IsPublic || x.GetCustomAttributes(typeof(SerializeField), false).Length > 0))
                {
                    EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(property.Name) + ": ", GUILayout.ExpandWidth(true));

                    try
                    {
                        property.SetValue(value, Convert.ChangeType(UniversalField(property.GetValue(value), property.FieldType), property.FieldType));
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }

                return value;
            }

            return null;
        }

        public static void DrawScript<T>(T script) where T : MonoBehaviour
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(script), typeof(T), false);
            GUI.enabled = true;
        }
    }
}