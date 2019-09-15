using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static CameraAspectSetuper;

[CustomEditor(typeof(CameraAspectSetuper))]
public class CameraAspectSetuperEditor : Editor
{
    private CameraAspectSetuper setuper;

    private void OnEnable()
    {
        setuper = target as CameraAspectSetuper;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying)
            return;

        if(GUILayout.Button("Save setup"))
        {
            AspectRatioSettings newSettings = new AspectRatioSettings();

            newSettings.ratioLabel = "new ratio";
            newSettings.aspectRatio = setuper.cameraRef.aspect;

            newSettings.cameraOffset = setuper.cameraRef.transform.position;

            int index = setuper.aspectRatiosSettings.FindIndex(0, a => a.aspectRatio == newSettings.aspectRatio);

            if (index == -1)
            {
                setuper.aspectRatiosSettings.Add(newSettings);
            }
            else
            {
                setuper.aspectRatiosSettings[index] = newSettings;
            }

            setuper.aspectRatiosSettings.OrderByDescending(x => x.aspectRatio);

            EditorUtility.SetDirty(setuper);
        }
    }

}