using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

/// <summary>
/// Controlls colors look of game. Use static SetRandomPreset or inst SetPreset() to change color theme.
/// </summary>
public class ColorsController : MonoBehaviour
{
    private static ColorsController instance;

    [Header("References")]
    public List<ColorsPreset> presetsList;

    [HideInInspector]
    public Material itemMaterial;
    [HideInInspector]
    public Material obstacleMaterial;
    [HideInInspector]
    public Material groundMaterial;
    [HideInInspector]
    public Material bordersMaterial;
    [HideInInspector]
    public Material backSeaMaterial;

    private int currentPresetIndex = -1;
    private int emissionColorID;


#if UNITY_EDITOR
    [HideInInspector]
    public ColorsPreset currentPresetEditor;
#endif

    private void Awake()
    {
        instance = this;
        emissionColorID = Shader.PropertyToID("_EmissionColor");

        presetsList.Shuffle();
    }

    public static void SetRandomPreset()
    {
        instance.currentPresetIndex++;

        if (instance.currentPresetIndex >= instance.presetsList.Count)
        {
            instance.currentPresetIndex = 0;
        }

        if (instance.presetsList.Count > 0)
        {
            instance.SetPreset(instance.presetsList[instance.currentPresetIndex]);
        }
    }

    public void SetPreset(ColorsPreset colorsPreset)
    {
        InitMaterial(itemMaterial, colorsPreset.item);
        InitMaterial(obstacleMaterial, colorsPreset.obstacle);
        InitMaterial(groundMaterial, colorsPreset.ground);
        InitMaterial(bordersMaterial, colorsPreset.borders);
        InitMaterial(backSeaMaterial, colorsPreset.backSea);

#if UNITY_EDITOR
        currentPresetEditor = colorsPreset;
#endif
    }

    private void InitMaterial(Material material, ColorsPreset.MaterialSave save)
    {
        material.color = save.albedo;

        if (save.emissionEnabled)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor(emissionColorID, save.emission);
        }
        else
        {
            material.DisableKeyword("_EMISSION");
        }

    }

    public static Material GetItemMaterial(LevelItemBehaviour.ItemType type)
    {
        if (type == LevelItemBehaviour.ItemType.Normal)
        {
            return instance.itemMaterial;
        }
        else
        {
            return instance.obstacleMaterial;
        }
    }

#if UNITY_EDITOR
    public static void EditorSetRandomPresetInverse()
    {
        instance.currentPresetIndex--;

        if (instance.currentPresetIndex < 0)
        {
            instance.currentPresetIndex = instance.presetsList.Count - 1;
        }

        instance.SetPreset(instance.presetsList[instance.currentPresetIndex]);
    }
#endif
}