using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorsPreset", menuName = "ColorsSystem/ColorsPreset")]
public class ColorsPreset : ScriptableObject
{
    public MaterialSave item = new MaterialSave(Color.white, Color.white, false);
    public MaterialSave obstacle = new MaterialSave(Color.white, Color.white, false);
    public MaterialSave ground = new MaterialSave(Color.white, Color.white, false);
    public MaterialSave borders = new MaterialSave(Color.white, Color.white, false);
    public MaterialSave backSea = new MaterialSave(Color.white, Color.white, false);

    [System.Serializable]
    public struct MaterialSave
    {
        public Color albedo;
        public Color emission;
        public bool emissionEnabled;

        public MaterialSave(Color albedo, Color emission, bool emissionEnabled)
        {
            this.albedo = albedo;
            this.emission = emission;
            this.emissionEnabled = emissionEnabled;
        }
    }
}