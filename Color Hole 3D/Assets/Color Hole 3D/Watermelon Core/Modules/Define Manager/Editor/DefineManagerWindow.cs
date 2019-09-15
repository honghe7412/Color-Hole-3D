using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;

namespace Watermelon
{
    public class DefineManagerWindow : EditorWindow
    {
        private bool[] enabledDefines;

        private Vector2 definesScroll;

        private bool requireInit = true;
        private string defineLine;
        private bool definesSame;

        private Define[] projectDefines;

        private string unknownDefines;
        private bool showUnknownDefines = false;

        private static readonly Vector2 WINDOW_SIZE = new Vector2(320, 310);

        [MenuItem("Tools/Editor/Define Manager")]
        public static void Init()
        {
            DefineManagerWindow window = (DefineManagerWindow)GetWindow(typeof(DefineManagerWindow), true, "Define Manager");
            window.maxSize = WINDOW_SIZE;
            window.minSize = WINDOW_SIZE;
        }

        private void OnEnable()
        {
            List<Define> defines = new List<Define>();

            //Get assembly
            Assembly assembly = Assembly.GetAssembly(typeof(DefineAttribute));
            Assembly editorAssembly = Assembly.GetAssembly(this.GetType());

            //Get all types with InvocableMethod attribute
            Type[] gameTypes = assembly.GetTypes().Where(m => m.IsDefined(typeof(DefineAttribute), true)).ToArray();
            foreach (Type type in gameTypes)
            {
                //Get attribute
                DefineAttribute[] defineAttributes = (DefineAttribute[])Attribute.GetCustomAttributes(type, typeof(DefineAttribute));

                for (int i = 0; i < defineAttributes.Length; i++)
                {
                    int methodId = defines.FindIndex(x => x.define == defineAttributes[i].define);
                    if (methodId == -1)
                    {
                        defines.Add(new Define(defineAttributes[i].define, false));
                    }
                }
            }

            Type[] editorTypes = editorAssembly.GetTypes().Where(m => m.IsDefined(typeof(DefineAttribute), true)).ToArray();
            foreach (Type type in editorTypes)
            {
                //Get attribute
                DefineAttribute[] defineAttributes = (DefineAttribute[])Attribute.GetCustomAttributes(type, typeof(DefineAttribute));

                for (int i = 0; i < defineAttributes.Length; i++)
                {
                    int methodId = defines.FindIndex(x => x.define == defineAttributes[i].define);
                    if (methodId == -1)
                    {
                        defines.Add(new Define(defineAttributes[i].define, true));
                    }
                }
            }

            projectDefines = defines.OrderBy(x => x.define).ToArray();

            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

            ClearDefines(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup));

            LoadDefines(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup));
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            DefinesGUI();

            if (EditorGUI.EndChangeCheck())
            {
                requireInit = true;
            }

            if (requireInit)
            {
                defineLine = GetDefines();
                definesSame = CompareDefines(defineLine);

                requireInit = false;
            }

            if (showUnknownDefines)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Unknown defines:", EditorStyles.boldLabel);
                if (GUILayout.Button("Remove", EditorStyles.miniButton))
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), GetDefines());

                    showUnknownDefines = false;
                    unknownDefines = "";
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.HelpBox(unknownDefines, MessageType.Warning, true);
                EditorGUILayout.EndVertical();
            }

            if (definesSame)
                GUI.enabled = false;

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Apply Defines", EditorStylesExtended.button_04))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), defineLine);

                requireInit = true;

                return;
            }

            GUI.enabled = true;
        }

        private void DefinesGUI()
        {
            GUILayout.Space(5);

            EditorGUILayout.BeginVertical();
            Rect rect = EditorGUILayout.BeginHorizontal(GUI.skin.box); //Defines block

            definesScroll = EditorGUILayout.BeginScrollView(definesScroll, false, false); //Define scrollview
            for (int i = 0; i < projectDefines.Length; i++)
            {
                EditorGUILayout.BeginHorizontal(); //Define
                EditorGUILayout.LabelField(new GUIContent(projectDefines[i].define + (projectDefines[i].editor ? " (editor)" : ""), projectDefines[i].define), GUILayout.ExpandWidth(true));
                enabledDefines[i] = EditorGUILayout.Toggle(enabledDefines[i], GUILayout.Width(20));
                EditorGUILayout.EndHorizontal(); //End define
            }
            EditorGUILayout.EndScrollView(); //End define scrollview

            EditorGUILayout.EndHorizontal(); //End defines block
            EditorGUILayout.EndVertical();
        }

        private string GetDefines()
        {
            string definesLine = "";

            for (int i = 0; i < projectDefines.Length; i++)
            {
                if (enabledDefines[i])
                {
                    definesLine += projectDefines[i].define + ";";
                }
            }

            return definesLine;
        }

        private void ClearDefines(string defines)
        {
            unknownDefines = "\n";

            string definesLine = "";
            showUnknownDefines = false;

            if (!string.IsNullOrEmpty(defines))
            {
                string[] definesArray = defines.Split(';');

                for (int i = 0; i < definesArray.Length; i++)
                {
                    int defineId = System.Array.FindIndex(projectDefines, x => x.define == definesArray[i]);
                    if (defineId != -1)
                    {
                        definesLine += projectDefines[i].define + ";";
                    }
                    else
                    {
                        unknownDefines += definesArray[i] + "\n";

                        showUnknownDefines = true;

                        Debug.LogWarning("[Define Manager]: Unknown define (" + definesArray[i] + ") has been found! Please, add defines using DefineAttribute.");
                    }
                }
            }
        }

        private void LoadDefines(string defines)
        {
            enabledDefines = new bool[projectDefines.Length];

            if (!string.IsNullOrEmpty(defines))
            {
                string[] definesArray = defines.Split(';');

                for (int i = 0; i < definesArray.Length; i++)
                {
                    int defineId = System.Array.FindIndex(projectDefines, x => x.define == definesArray[i]);
                    if (defineId != -1)
                    {
                        enabledDefines[defineId] = true;
                    }
                }
            }
        }

        private bool CompareDefines(string defineLine)
        {
            string[] currentDefinesArray = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget)).Split(';');

            for (int i = 0; i < projectDefines.Length; i++)
            {
                int findIndex = System.Array.FindIndex(currentDefinesArray, x => x == projectDefines[i].define);

                if (enabledDefines[i])
                {
                    if (findIndex == -1)
                        return false;
                }
                else
                {
                    if (findIndex != -1)
                        return false;
                }
            }

            return true;
        }

        private struct Define
        {
            public string define;
            public bool editor;

            public Define(string define, bool editor)
            {
                this.define = define;
                this.editor = editor;
            }
        }
    }
}