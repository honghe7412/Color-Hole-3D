using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Watermelon;

public class CameraAspectSetuper : MonoBehaviour
{
    [Header("Settings")]
    public List<AspectRatioSettings> aspectRatiosSettings = new List<AspectRatioSettings>();

    [Header("References")]
    public Camera cameraRef;

    public AspectRatioSettings GetSettings()
    {
        aspectRatiosSettings = aspectRatiosSettings.OrderByDescending(x => x.aspectRatio).ToList();

        float currentAspectRatio = cameraRef.aspect;

        for (int i = 0; i < aspectRatiosSettings.Count; i++)
        {
            // if our aspect ratio is spesified at settings - perfect - applying settings
            if (currentAspectRatio == aspectRatiosSettings[i].aspectRatio)
            {
                return aspectRatiosSettings[i];
            }
            // if current aspect is bigger  than current saved settings aspect - we have not this aspect saved - interpolating beetween current and previous settings
            else if (currentAspectRatio > aspectRatiosSettings[i].aspectRatio)
            {
                // if biggest saved setting aspect is smaller than we need - anyway applying this settings
                if (i == 0)
                {
                    return aspectRatiosSettings[i];
                }
                // otherwise interpolating settings
                else
                {
                    float interpolationCoef = (currentAspectRatio - aspectRatiosSettings[i].aspectRatio) / (aspectRatiosSettings[i - 1].aspectRatio - aspectRatiosSettings[i].aspectRatio);

                    AspectRatioSettings interpolatedSettings = AspectRatioSettings.GetInterpolatedSettings(aspectRatiosSettings[i], aspectRatiosSettings[i - 1], interpolationCoef);

                    return interpolatedSettings;
                }
            }
            // if current aspect is smaller then smallest saved settings - applying smallest settings
            else if (i == aspectRatiosSettings.Count - 1)
            {
                return aspectRatiosSettings[i];
            }
            // otherwise moving to next settings
        }

        return aspectRatiosSettings[0];
    }

    //// applying saved or calculated aspect settings
    //private void ApplyAspectRatioSettings(AspectRatioSettings settings)
    //{
    //    // [add here initializing of saved camera settings]

    //}


    [System.Serializable]
    public class AspectRatioSettings
    {
        // standart fields
        public string ratioLabel;
        public float aspectRatio;

        [Space(5)]
        // [add here custom settings depending on project]
        public Vector3 cameraOffset;

        public AspectRatioSettings()
        {
            ratioLabel = "new ratio";
            aspectRatio = 0f;

            cameraOffset = new Vector3();
        }

        public static AspectRatioSettings GetInterpolatedSettings(AspectRatioSettings smallerSettings, AspectRatioSettings biggerSettings, float interpolationCoef)
        {
            AspectRatioSettings newSettins = new AspectRatioSettings();
            newSettins.ratioLabel = "interpolated";

            // [initialize newSettings with interpolated values]
            // [for example: newSettins.cameraOffset = Vector3.Lerp(smallerSettings.cameraOffset, biggerSettings.cameraOffset, interpolationCoef);]
            newSettins.cameraOffset = Vector3.Lerp(smallerSettings.cameraOffset, biggerSettings.cameraOffset, interpolationCoef);

            return newSettins;
        }
    }


}