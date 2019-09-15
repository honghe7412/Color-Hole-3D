using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Watermelon;

[CustomEditor(typeof(ColorsController))]
public class ColorsControllerEditor : Editor
{
    //private ColorsPreset colorsPreset;
    private bool presetCreation = false;
    private bool hasUnsavedChanges = false;
    private int emissionColorID;
    private string newPresetName;

    GUIStyle boldSyle = new GUIStyle();
    ColorsController controller;

    private const string PRESETS_PATH = @"Assets/Color Hole 3D/Content/ColorPresets/";

    private void OnEnable()
    {
        emissionColorID = Shader.PropertyToID("_EmissionColor");

        boldSyle.fontStyle = FontStyle.Bold;
        presetCreation = false;
        newPresetName = string.Empty;

        hasUnsavedChanges = false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();
        GUILayout.Space(5);

        controller = target as ColorsController;

        controller.itemMaterial = DrawMaterialAndColorEditor("Item", controller.itemMaterial);
        GUILayout.Space(8);
        controller.obstacleMaterial = DrawMaterialAndColorEditor("Obstacle", controller.obstacleMaterial);
        GUILayout.Space(8);
        controller.groundMaterial = DrawMaterialAndColorEditor("Ground", controller.groundMaterial);
        GUILayout.Space(8);
        controller.bordersMaterial = DrawMaterialAndColorEditor("Borders", controller.bordersMaterial);
        GUILayout.Space(8);
        controller.backSeaMaterial = DrawMaterialAndColorEditor("Back Sea", controller.backSeaMaterial);

        GUILayout.Space(10);

        GUILayout.Label("Developement", boldSyle);

        if (!presetCreation)
        {
            EditorGUILayout.BeginHorizontal();
            controller.currentPresetEditor = (ColorsPreset)EditorGUILayout.ObjectField("Current Preset: ", controller.currentPresetEditor, typeof(ColorsPreset), true);

            if (GUILayout.Button("New"))
            {
                presetCreation = true;
            }

            GUILayout.EndHorizontal();

            //if (hasUnsavedChanges)
            //{
            //    GUILayout.Space(10);

            //    EditorGUILayout.HelpBox("There is unsaved changes", MessageType.Warning);
            //}

            GUILayout.Space(10);

            if (EditorApplication.isPlaying)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("◀", GUILayout.Height(20)))
                {
                    if (hasUnsavedChanges)
                    {
                        if (EditorUtility.DisplayDialog("There is unsaved changes", "All changes will be lost", "Discard changes", "Cancel"))
                        {
                            hasUnsavedChanges = false;
                            ColorsController.EditorSetRandomPresetInverse();
                        }
                    }
                    else
                    {
                        ColorsController.EditorSetRandomPresetInverse();
                    }
                }

                if (GUILayout.Button("▶", GUILayout.Height(20)))
                {
                    if (hasUnsavedChanges)
                    {
                        if (EditorUtility.DisplayDialog("There is unsaved changes", "All changes will be lost", "Discard changes", "Cancel"))
                        {
                            hasUnsavedChanges = false;
                            ColorsController.SetRandomPreset();
                        }
                    }
                    else
                    {
                        ColorsController.SetRandomPreset();
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Load current", GUILayout.Height(30), GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.463f)))
            {
                LoadCurrent(controller);
            }

            if (GUILayout.Button((hasUnsavedChanges ? "*" : "") + "Save to current", GUILayout.Height(30), GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.463f)))
            {
                SaveToCurrent(controller);
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            newPresetName = EditorGUILayout.TextField("Preset Name: ", newPresetName);

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Cancel", GUILayout.Height(30)))
            {
                presetCreation = false;
                newPresetName = string.Empty;
            }

            if (GUILayout.Button("Create", GUILayout.Height(30)))
            {
                controller.currentPresetEditor = ScriptableObject.CreateInstance<ColorsPreset>();
                AssetDatabase.CreateAsset(controller.currentPresetEditor, PRESETS_PATH + (newPresetName == string.Empty ? "NewPreset" : newPresetName) + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                presetCreation = false;
            }

            GUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private Material DrawMaterialAndColorEditor(string label, Material material)
    {
        EditorGUILayout.BeginVertical();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
        Material newMaterial = (Material)EditorGUILayout.ObjectField(material, typeof(Material), false);
        EditorGUILayout.EndHorizontal();
        if (newMaterial != null)
        {
            newMaterial.color = EditorGUILayout.ColorField("Albedo", newMaterial.color);
            EditorGUILayout.BeginHorizontal();

            bool emissionState = EditorGUILayout.Toggle("Emission", newMaterial.IsKeywordEnabled("_EMISSION"), GUILayout.Width(EditorGUIUtility.labelWidth + 12));
            if (emissionState)
            {
                newMaterial.EnableKeyword("_EMISSION");
                newMaterial.SetColor(emissionColorID, EditorGUILayout.ColorField(newMaterial.GetColor(emissionColorID)/*, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f)*/));
            }
            else
            {
                newMaterial.DisableKeyword("_EMISSION");
            }

            EditorGUILayout.EndHorizontal();
        }

        if (newMaterial != null && (GUI.changed || newMaterial.color != material.color))
        {
            hasUnsavedChanges = true;
            Undo.RecordObject(target, "Changed color preset");

            if (!EditorApplication.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.EndVertical();

        return newMaterial;
    }

    private Color DrawUIColorEditor(string label, Color color)
    {
        EditorGUI.BeginChangeCheck();

        Color newColor = EditorGUILayout.ColorField(label, color);

        if (GUI.changed || newColor != color)
        {
            Undo.RecordObject(target, "Changed color preset");

            EditorUtility.SetDirty(controller.currentPresetEditor);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        return newColor;
    }


    [Button("Load current")]
    public void LoadCurrent(ColorsController controller)
    {
        hasUnsavedChanges = false;
        if (controller.currentPresetEditor != null)
        {
            controller.SetPreset(controller.currentPresetEditor);
        }
        else
        {
            Debug.Log("Please, assign current preset");
        }
    }

    [Button("Save to current")]
    public void SaveToCurrent(ColorsController controller)
    {
        hasUnsavedChanges = false;
        if (controller.currentPresetEditor != null)
        {
            controller.currentPresetEditor.item = GetMaterialSave(controller.itemMaterial);
            controller.currentPresetEditor.obstacle = GetMaterialSave(controller.obstacleMaterial);
            controller.currentPresetEditor.ground = GetMaterialSave(controller.groundMaterial);
            controller.currentPresetEditor.borders = GetMaterialSave(controller.bordersMaterial);
            controller.currentPresetEditor.backSea = GetMaterialSave(controller.backSeaMaterial);

            EditorUtility.SetDirty(controller.currentPresetEditor);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log("Please, assign current preset");
        }
    }

    private ColorsPreset.MaterialSave GetMaterialSave(Material material)
    {
        return new ColorsPreset.MaterialSave(material.color, material.GetColor(emissionColorID), material.IsKeywordEnabled("_EMISSION"));
    }
}